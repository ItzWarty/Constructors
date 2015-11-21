using Fody.Constructors;

namespace AssemblyToProcess {
    [RequiredArgsConstructor]
    public class RequiredArgsExampleClass {
        private readonly int f1 = 123;
        private readonly object f2;
        private string f3;

        public int F1 { get { return f1; } }
        public object F2 { get { return f2; } }
        public string F3 { get { return f3; } }
    }
}