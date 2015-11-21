using Mono.Cecil;
using System.Linq;

public static class CecilExtensions {
    const string RequiredArgsConstructorAttributeTypeName = "RequiredArgsConstructorAttribute";
    const string AllArgsConstructorAttributeTypeName = "AllArgsConstructorAttribute";
    const string NoArgsConstructorAttributeTypeName = "NoArgsConstructorAttribute"; 

    public static CustomAttribute GetRequiredArgsConstructorAttributeOrNull(this TypeDefinition value) {
        return value.CustomAttributes.FirstOrDefault(a => a.AttributeType.Name == RequiredArgsConstructorAttributeTypeName);
    }

    public static CustomAttribute GetAllArgsConstructorAttributeOrNull(this TypeDefinition value) {
        return value.CustomAttributes.FirstOrDefault(a => a.AttributeType.Name == AllArgsConstructorAttributeTypeName);
    }

    public static CustomAttribute GetNoArgsConstructorAttributeOrNull(this TypeDefinition value) {
        return value.CustomAttributes.FirstOrDefault(a => a.AttributeType.Name == NoArgsConstructorAttributeTypeName);
    }

    public static bool RequestsRequiredArgsConstructor(this TypeDefinition value) {
        return value.GetRequiredArgsConstructorAttributeOrNull() != null;
    }

    public static bool RequestsAllArgsConstructor(this TypeDefinition value) {
        return value.GetAllArgsConstructorAttributeOrNull() != null;
    }

    public static bool RequestsNoArgsConstructor(this TypeDefinition value) {
        return value.GetNoArgsConstructorAttributeOrNull() != null;
    }
}
