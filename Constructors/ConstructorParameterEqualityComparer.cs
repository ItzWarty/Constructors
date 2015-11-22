using Mono.Cecil;
using System.Collections.Generic;

public class ConstructorParameterEqualityComparer : IEqualityComparer<ParameterDefinition> {
   public bool Equals(ParameterDefinition x, ParameterDefinition y) {
      return x.ParameterType.Equals(y.ParameterType);
   }

   public int GetHashCode(ParameterDefinition obj) {
      return obj.ParameterType.GetHashCode();
   }
}
