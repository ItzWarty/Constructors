using Fody.Constructors.Internal;
using System;

namespace Fody.Constructors {
   public abstract class AbstractConstructorAttributeBase : Attribute {
      internal AbstractConstructorAttributeBase(Predicate<FieldInfo> condition) {
         Condition = condition;
      }

      public Predicate<FieldInfo> Condition { get; set; }
   }
}
