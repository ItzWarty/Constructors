using Fody.Constructors;

namespace AssemblyToProcess {
   [AllFieldsConstructor]
   public class AllFieldsExampleClass {
      private readonly object f1 = "qwerty";
      private string f2;

      public object F1 { get { return f1; } }
      public string F2 { get { return f2; } }
   }
}