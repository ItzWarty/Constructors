using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Mono.Collections.Generic;

public class ModuleWeaver
{
    // Will log an informational message to MSBuild
    public Action<string> LogInfo { get; set; }

    // An instance of Mono.Cecil.ModuleDefinition for processing
    public ModuleDefinition ModuleDefinition { get; set; }

    TypeSystem typeSystem;

    // Init logging delegates to make testing easier
    public ModuleWeaver()
    {
        LogInfo = m => { };
    }

    public void Execute()
    {
        typeSystem = ModuleDefinition.TypeSystem;

        foreach (var type in ModuleDefinition.Types) {
            if (type.RequestsRequiredArgsConstructor()) {
                type.CustomAttributes.Remove(type.GetRequiredArgsConstructorAttributeOrNull());
                AddConstructor(type, field => field.IsInitOnly);
                LogInfo("Added required arguments constructor to '" + type.Name + "'.");
            }
            if (type.RequestsAllArgsConstructor()) {
                type.CustomAttributes.Remove(type.GetAllArgsConstructorAttributeOrNull());
                AddConstructor(type, field => true);
                LogInfo("Added all arguments constructor to '" + type.Name + "'.");
            }
            if (type.RequestsNoArgsConstructor()) {
                type.CustomAttributes.Remove(type.GetNoArgsConstructorAttributeOrNull());
                AddConstructor(type, field => false);
                LogInfo("Added no arguments constructor to '" + type.Name + "'.");
            }
        }
    }
    
    void AddConstructor(TypeDefinition type, Func<FieldDefinition, bool> fieldCondition) {
        var constructor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, typeSystem.Void);
        var bannedFields = new HashSet<FieldDefinition>();
        MethodReference parentConstructor;
        if (!TryFindDefaultConstructor(type, out parentConstructor)) {
            parentConstructor = ModuleDefinition.ImportReference(typeSystem.Object.Resolve().GetConstructors().First());
        } else {
            MethodDefinition parentConstructorDefinition = parentConstructor.Resolve();
            foreach (var instruction in parentConstructorDefinition.Body.Instructions) {
                if (instruction.OpCode == OpCodes.Stfld) {
                    var fieldReference = (FieldReference)instruction.Operand;
                    bannedFields.Add(fieldReference.Resolve());
                }
            }
        }
        var processor = constructor.Body.GetILProcessor();
        processor.Emit(OpCodes.Ldarg_0);
        processor.Emit(OpCodes.Call, parentConstructor);
        int argumentCount = 1; // `this` is arg 0
        foreach (var field in type.Fields.Where(fieldCondition)) {
            if (bannedFields.Contains(field)) continue;
            var argumentId = argumentCount;
            argumentCount++;

            AddFieldConstructorArgument(constructor, field, argumentId, processor);
        }
        processor.Emit(OpCodes.Ret);

        if (VerifyNoSimilarConstructors(type.Methods, constructor)) {
            type.Methods.Add(constructor);
        }
    }

    public bool TryFindDefaultConstructor(TypeDefinition type, out MethodReference super) {
        foreach (var method in type.Methods.Where(m => m.Name == ".ctor")) {
            if (method.Parameters.Count == 0) {
                super = method;
                return true;
            }
        }
        super = null;
        return false;
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

    private bool VerifyNoSimilarConstructors(Collection<MethodDefinition> methods, MethodDefinition testConstructor) {
        var constructorParameterComparer = new ConstructorParameterEqualityComparer();
        foreach (var otherConstructor in methods.Where(m => m.Name == ".ctor")) {
            if (otherConstructor.Parameters.SequenceEqual(testConstructor.Parameters, constructorParameterComparer)) {
                return false;
            }
        }
        return true;
    }
}