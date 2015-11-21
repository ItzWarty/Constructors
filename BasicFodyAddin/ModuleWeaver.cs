using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using System;
using System.Linq;
using System.Windows.Forms;

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
        }
    }
    
    void AddConstructor(TypeDefinition type, Predicate<FieldDefinition> fieldCondition) {
        var constructor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, typeSystem.Void);
        var objectConstructor = ModuleDefinition.ImportReference(typeSystem.Object.Resolve().GetConstructors().First());
        var processor = constructor.Body.GetILProcessor();
        processor.Emit(OpCodes.Ldarg_0);
        processor.Emit(OpCodes.Call, objectConstructor);
        int argumentCount = 1; // `this` is arg 0
        foreach (var field in type.Fields) {
            if (fieldCondition(field)) {
                var argumentId = argumentCount;
                argumentCount++;

                AddFieldConstructorArgument(constructor, field, argumentId, processor);
            }
        }
        processor.Emit(OpCodes.Ret);
        type.Methods.Add(constructor);
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
}