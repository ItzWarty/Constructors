using Fody.Constructors.Internal;
using System;

namespace Fody.Constructors {
   [AttributeUsage(AttributeTargets.Class)]
   public class RequiredFieldsConstructorAttribute : AbstractConstructorAttributeBase {
      public RequiredFieldsConstructorAttribute() : base(FieldCondition) { }

      private static bool FieldCondition(FieldInfo field) {
         return !field.HasNonDefaultFieldInitializer && field.IsInitOnly;
      }
   }
}
