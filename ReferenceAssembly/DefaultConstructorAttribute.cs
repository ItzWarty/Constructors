using Fody.Constructors.Internal;
using System;

namespace Fody.Constructors {
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultConstructorAttribute : AbstractConstructorAttributeBase {
        public DefaultConstructorAttribute() : base(FieldCondition) { }

        private static bool FieldCondition(FieldInfo field) {
            return false;
        }
    }
}