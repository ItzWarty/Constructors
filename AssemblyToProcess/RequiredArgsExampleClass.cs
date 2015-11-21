using Fody.Constructors;

namespace AssemblyToProcess {
    [RequiredArgsConstructor]
    public class RequiredArgsExampleClass {
        private readonly object f1;
        private string f2;

        public object F1 { get { return f1; } }
        public string F2 { get { return f2; } }
    }
}