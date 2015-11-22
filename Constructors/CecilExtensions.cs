using System;
using System.CodeDom;
using System.Diagnostics;
using Mono.Cecil;
using System.Linq;
using System.Reflection;
using Mono.Cecil.Rocks;

public static class CecilExtensions {
    public static bool HasDefaultConstructor(this TypeDefinition type) {
        return type.GetConstructors().Any(t => t.Parameters.Count == 0);
    }

    public static bool TryFindDefaultConstructor(this TypeDefinition type, out MethodReference defaultConstructor) {
        foreach (var method in type.Methods) {
            if (method.Name == ".ctor" && method.Parameters.Count == 0) {
                defaultConstructor = method;
                return true;
            }
        }
        defaultConstructor = null;
        return false;
    }

    public static T FindAndActivateCustomAttributeOrNull<T>(this TypeDefinition value) where T : Attribute {
        var customAttribute = value.CustomAttributes.FirstOrDefault(a => a.AttributeType.Name == typeof(T).Name);
        if (customAttribute == null) {
            return null;
        } else {
            return ActivateInstance<T>(customAttribute);
        }
    }

    public static CustomAttribute FindCustomAttributeOrNull<T>(this TypeDefinition value) {
        return value.CustomAttributes.FirstOrDefault(a => a.AttributeType.Name == typeof(T).Name);
    }

    public static T ActivateInstance<T>(CustomAttribute attribute) {
        var constructorArguments = attribute.ConstructorArguments.Select(arg => arg.Value).ToArray();
        var instance = (T)Activator.CreateInstance(typeof(T), constructorArguments);
        foreach (var property in attribute.Properties) {
            typeof(T).GetProperty(property.Name).SetValue(instance, property.Argument.Value, null);
        }
        return instance;
    }
}
