using System;
using Fody.Constructors.Internal;

namespace Fody.Constructors {
    [AttributeUsage(AttributeTargets.Class)]
    public class AllFieldsConstructorAttribute : AbstractConstructorAttributeBase {
        public AllFieldsConstructorAttribute() : base(FieldCondition) { }

        private static bool FieldCondition(FieldInfo field) {
            return true;
        }
    }
}