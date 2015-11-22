using Fody.Constructors.Internal;
using System;

namespace Fody.Constructors {
   [AttributeUsage(AttributeTargets.Class)]
   public class AllFieldsConstructorAttribute : AbstractConstructorAttributeBase {
      public AllFieldsConstructorAttribute() : base(FieldCondition) { }

      private static bool FieldCondition(FieldInfo field) {
         return true;
      }
   }
}