using Fody.Constructors.Internal;
using System;

namespace Fody.Constructors {
    [AttributeUsage(AttributeTargets.Class)]
    public class UninitializedFieldsConstructorAttribute : AbstractConstructorAttributeBase {
        public UninitializedFieldsConstructorAttribute() : base(FieldCondition) { }

        private static bool FieldCondition(FieldInfo field) {
            return !field.HasNonDefaultFieldInitializer;
        }
    }
}