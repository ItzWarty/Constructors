using Mono.Cecil;
using System.Linq;

public static class CecilExtensions {
    const string RequiredArgsConstructorAttributeTypeName = "RequiredArgsConstructorAttribute";
    const string AllArgsConstructorAttributeTypeName = "AllArgsConstructorAttribute";

    public static CustomAttribute GetRequiredArgsConstructorAttributeOrNull(this TypeDefinition value) {
        return value.CustomAttributes.FirstOrDefault(a => a.AttributeType.Name == RequiredArgsConstructorAttributeTypeName);
    }

    public static CustomAttribute GetAllArgsConstructorAttributeOrNull(this TypeDefinition value) {
        return value.CustomAttributes.FirstOrDefault(a => a.AttributeType.Name == AllArgsConstructorAttributeTypeName);
    }

    public static bool RequestsRequiredArgsConstructor(this TypeDefinition value) {
        return value.GetRequiredArgsConstructorAttributeOrNull() != null;
    }

    public static bool RequestsAllArgsConstructor(this TypeDefinition value) {
        return value.GetAllArgsConstructorAttributeOrNull() != null;
    }
}
