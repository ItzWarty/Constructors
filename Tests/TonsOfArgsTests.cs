using System;
using System.Linq;
using AssemblyToProcess;
using Xunit;

namespace Tests {
    public class TonsOfArgsTests : WeaverTestsBase {
        public TonsOfArgsTests() : base(typeof(TonsOfArgsExampleClass)) { }

        [Fact]
        public void TonsOfAllArgsConstructor_Properties() {
            var injectedObject = new object();
            var injectedTime = DateTime.Now;

            var type = assembly.GetType("AssemblyToProcess.TonsOfArgsExampleClass");
            var addedCtor = type.GetConstructors().First(c => c.GetParameters().Length > 0);
            var instance = addedCtor.Invoke(new[] { "test", 1337, 1.337f, injectedObject, injectedTime });

            Assert.Equal("test", instance.GetType().GetProperty("F1").GetValue(instance, null));
            Assert.Equal(1337, instance.GetType().GetProperty("F2").GetValue(instance, null));
            Assert.Equal(1.337f, instance.GetType().GetProperty("F3").GetValue(instance, null));
            Assert.Equal(injectedObject, instance.GetType().GetProperty("F4").GetValue(instance, null));
            Assert.Equal(injectedTime, instance.GetType().GetProperty("F5").GetValue(instance, null));
        }
    }
}