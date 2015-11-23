using Fody.Constructors;

namespace AssemblyToProcess {
   [UninitializedFieldsConstructor]
   public class UninitializedFieldsExampleClass {
      private readonly object f1 = "derp";
      private readonly object f2;
      private string f3;

      public object F1 { get { return f1; } }
      public object F2 { get { return f2; } }
      public string F3 { get { return f3; } }
   }
}