using Fody.Constructors;
using Fody.Constructors.Internal;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using Constructors.Fody;

// ReSharper disable once CheckNamespace
public class ModuleWeaver {
   // Will log an informational message to MSBuild
   public Action<string> LogInfo { get; set; }

   // An instance of Mono.Cecil.ModuleDefinition for processing
   public ModuleDefinition ModuleDefinition { get; set; }

   private TypeSystem TypeSystem => ModuleDefinition.TypeSystem;

   // Init logging delegates to make testing easier
   public ModuleWeaver() {
      LogInfo = m => { };
   }

   public void Execute() {
      foreach (var type in ModuleDefinition.Types) {
         var defaultConstructorAttribute = type.FindAndActivateCustomAttributeOrNull<DefaultConstructorAttribute>();
         var otherConstructorAttributes = new AbstractConstructorAttributeBase[] {
            type.FindAndActivateCustomAttributeOrNull<RequiredFieldsConstructorAttribute>(),
            type.FindAndActivateCustomAttributeOrNull<UninitializedFieldsConstructorAttribute>()
         }.Where(x => x != null).ToArray();

         if (defaultConstructorAttribute == null && otherConstructorAttributes.Length == 0) continue;

         var defaultConstructor = FindOrCreateAndAddDefaultConstructor(type).Resolve();
         defaultConstructor.IsPublic = defaultConstructorAttribute != null;

         foreach (var constructorAttribute in otherConstructorAttributes) {
            var fieldsToInitialize = FindMatchingFields(type, defaultConstructor, constructorAttribute);
            var injectionConstructor = CreateInjectionConstructor(type, defaultConstructor, fieldsToInitialize);
            MethodDefinition similarConstructor;
            if (!TryFindSimilarConstructors(type.Methods, injectionConstructor, out similarConstructor)) {
               type.Methods.Add(injectionConstructor);
            } else {
               similarConstructor.IsPublic = true;
            }
         }
      }
   }

   private IList<FieldDefinition> FindMatchingFields(TypeDefinition type, MethodDefinition defaultConstructor, AbstractConstructorAttributeBase constructorAttribute) {
      var matches = new List<FieldDefinition>();
      var initializedFields = EnumerateInitializedFields(defaultConstructor);
      foreach (var field in type.Fields) {
         var fieldInfo = new FieldInfo {
            IsInitOnly = field.IsInitOnly,
            HasNonDefaultFieldInitializer = initializedFields.Contains(field)
         };
         if (constructorAttribute.Condition(fieldInfo)) {
            matches.Add(field);
         }
      }
      return matches;
   }

   private MethodReference FindOrCreateAndAddDefaultConstructor(TypeDefinition type) {
      MethodReference defaultConstructor;
      if (type.TryFindDefaultConstructor(out defaultConstructor)) {
         return defaultConstructor;
      } else {
         var superConstructor = ModuleDefinition.ImportReference(TypeSystem.Object.Resolve().GetConstructors().First());
         var parameterlessConstructor = CreateParameterlessConstructor(type, superConstructor);
         type.Methods.Add(parameterlessConstructor);
         return parameterlessConstructor;
      }
   }

   private MethodDefinition CreateParameterlessConstructor(TypeDefinition type, MethodReference parentConstructor) {
      var constructor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, TypeSystem.Void);
      var processor = constructor.Body.GetILProcessor();

      // Select referencedConstructor whose field initializer code we're cloning
      // Note: Field initializers aren't optimized away, even if the field is overwritten by ctor!
      var referencedConstructor = type.GetConstructors().FirstOrDefault();
      if (referencedConstructor != null) {
         // Copy all instructions to last field initializer set-field.
         // If we don't run into a .ctor somehow, all instructions are copied.
         var referencedBody = referencedConstructor.Body;
         var referencedInstructions = referencedBody.Instructions;
         int copiedInstructionCount = referencedInstructions.Count;
         var superCall = referencedInstructions.FirstOrDefault(
            i => i.OpCode == OpCodes.Call && ((MethodReference)i.Operand).Name == ".ctor");
         if (superCall != null) {
            copiedInstructionCount = referencedInstructions.IndexOf(superCall);
            while (copiedInstructionCount > 0 &&
                   referencedInstructions[copiedInstructionCount - 1].OpCode != OpCodes.Stfld) {
               copiedInstructionCount--;
            }
         }
         for (var i = 0; i < copiedInstructionCount; i++) {
            processor.Append(referencedInstructions[i]);
         }
      }

      // Call parent constructor
      processor.Emit(OpCodes.Ldarg_0);
      processor.Emit(OpCodes.Call, parentConstructor);
      processor.Emit(OpCodes.Ret);
      return constructor;
   }

   private MethodDefinition CreateInjectionConstructor(TypeDefinition type, MethodReference parentConstructor, IEnumerable<FieldDefinition> fieldsToInitialize) {
      var constructor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, TypeSystem.Void);
      var processor = constructor.Body.GetILProcessor();
      processor.Emit(OpCodes.Ldarg_0);
      processor.Emit(OpCodes.Call, parentConstructor);
      int argumentCount = 1; // `this` is arg 0
      foreach (var field in fieldsToInitialize) {
         var argumentId = argumentCount;
         argumentCount++;

         AddFieldConstructorArgument(constructor, field, argumentId, processor);
      }
      processor.Emit(OpCodes.Ret);
      return constructor;
   }

   private ISet<FieldDefinition> EnumerateInitializedFields(MethodDefinition method) {
      var initializedFields = new HashSet<FieldDefinition>();
      var body = method.Body;
      bool isOptimizableLoad = true;
      foreach (var instruction in body.Instructions) {
         if (instruction.OpCode == OpCodes.Call && ((MethodReference)instruction.Operand).Name == ".ctor") {
            break;
         } else if (instruction.OpCode == OpCodes.Ldarg_0) {
            // push `this` to stack
         } else if (instruction.OpCode == OpCodes.Ldnull) {
            // push `null` to stack
         } else if (instruction.OpCode == OpCodes.Stfld) {
            var field = (FieldReference)instruction.Operand;
            if (!isOptimizableLoad) {
               initializedFields.Add(field.Resolve());
            }

            // reset state for next field
            isOptimizableLoad = true;
         } else {
            isOptimizableLoad = false;
         }
      }
      return initializedFields;
   }

   private void AddFieldConstructorArgument(MethodDefinition constructor, FieldDefinition field, int argumentId, ILProcessor processor) {
      constructor.Parameters.Add(new ParameterDefinition(field.Name, ParameterAttributes.None, field.FieldType));
      processor.Emit(OpCodes.Ldarg_0);
      if (argumentId == 1) {
         processor.Emit(OpCodes.Ldarg_1);
      } else if (argumentId == 2) {
         processor.Emit(OpCodes.Ldarg_2);
      } else if (argumentId == 3) {
         processor.Emit(OpCodes.Ldarg_3);
      } else {
         processor.Emit(OpCodes.Ldarg, argumentId);
      }
      processor.Emit(OpCodes.Stfld, field);
   }

   private bool TryFindSimilarConstructors(Collection<MethodDefinition> methods, MethodDefinition testConstructor, out MethodDefinition similarConstructor) {
      var constructorParameterComparer = new ConstructorParameterEqualityComparer();
      foreach (var otherConstructor in methods.Where(m => m.Name == ".ctor")) {
         if (otherConstructor.Parameters.SequenceEqual(testConstructor.Parameters, constructorParameterComparer)) {
            similarConstructor = otherConstructor;
            return true;
         }
      }
      similarConstructor = null;
      return false;
   }
}