using System.Collections.Generic;
using Mono.Cecil;

namespace Constructors.Fody {
   public class ConstructorParameterEqualityComparer : IEqualityComparer<ParameterDefinition> {
      public bool Equals(ParameterDefinition x, ParameterDefinition y) {
         return x.ParameterType.Equals(y.ParameterType);
      }

      public int GetHashCode(ParameterDefinition obj) {
         return obj.ParameterType.GetHashCode();
      }
   }
}
