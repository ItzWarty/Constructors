using Fody.Constructors;

namespace AssemblyToProcess {
   [DefaultConstructor]
   public class NoArgsExampleClass {
      private readonly object f1;
      private readonly string f2 = "asdf"; // Note: noargsctor will not set this to "asdf"; it will be null!

      public NoArgsExampleClass(object f1, string f2) {
         this.f1 = f1;
         this.f2 = f2;
      }

      public object F1 { get { return f1; } }
      public string F2 { get { return f2; } }
   }
}