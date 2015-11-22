using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fody.Constructors;

namespace AssemblyToProcess {
    [RequiredFieldsConstructor]
    public class TrivialRequiredArgsExampleClass {
        private int f1 = 123;

        public int F1 {  get { return f1; } }
    }
}
