using Fody.Constructors;

namespace AssemblyToProcess {
    [NoArgsConstructor]
    [RequiredArgsConstructor]
    public class MultiConstructorExampleClass2 {
        private readonly object f1;
        private readonly string f2;

        public MultiConstructorExampleClass2(object f1, string f2) {
            this.f1 = f1;
            this.f2 = f2;
        }

        public object F1 { get { return f1; } }
        public string F2 { get { return f2; } }
    }
}