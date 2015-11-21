using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fody.Constructors;

namespace AssemblyToProcess {
    [NoArgsConstructor]
    [RequiredArgsConstructor]
    public class MultiConstructorExampleClass1 {
        private readonly object f1;
        private readonly string f2;

        public object F1 { get { return f1; } }
        public string F2 { get { return f2; } }
    }
}
