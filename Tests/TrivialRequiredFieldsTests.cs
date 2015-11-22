using System;
using System.Linq;
using AssemblyToProcess;
using Xunit;

namespace Tests {
    public class TrivialRequiredFieldsTests : WeaverTestsBase {
        public TrivialRequiredFieldsTests() : base(typeof(TrivialRequiredArgsExampleClass)) { }

        [Fact]
        public void ValidateTrivialRequiredArgsConstructor_HasOnlyDefaultConstructor() {
            var ctors = type.GetConstructors();
            var defaultConstructor = ctors.First();
            var defaultConstructorParameters = defaultConstructor.GetParameters();

            Assert.Equal(1, ctors.Length);
            Assert.Equal(0, defaultConstructorParameters.Length);
        }

        [Fact]
        public void ValidateTrivialRequiredArgsConstructor_Properties() {
            var addedCtor = type.GetConstructors().First();
            var instance = addedCtor.Invoke(null);

            Assert.Equal(123, instance.GetType().GetProperty("F1").GetValue(instance, null));
        }

        [Fact]
        public void ValidateRequiredArgsConstructor_Count() {
            Assert.Equal(1, type.GetConstructors().Length);
        }
    }
}