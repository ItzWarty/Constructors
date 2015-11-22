using AssemblyToProcess;
using System.Linq;
using Xunit;

namespace Tests {
   public class RequiredArgsConstructorTests : WeaverTestsBase {
      public RequiredArgsConstructorTests() : base(typeof(RequiredArgsExampleClass)) { }

      [Fact]
      public void ValidateRequiredArgsConstructor_Interface() {
         var type = assembly.GetType("AssemblyToProcess.RequiredArgsExampleClass");
         var addedCtor = type.GetConstructors().First(c => c.GetParameters().Length > 0);
         var parameters = addedCtor.GetParameters();

         Assert.Equal(1, parameters.Length);
         Assert.Equal("f2", parameters[0].Name);
         Assert.Equal(typeof(object), parameters[0].ParameterType);
      }

      [Fact]
      public void ValidateRequiredArgsConstructor_Properties() {
         var type = assembly.GetType("AssemblyToProcess.RequiredArgsExampleClass");
         var addedCtor = type.GetConstructors().First(c => c.GetParameters().Length > 0);
         var instance = addedCtor.Invoke(new[] { "test" });

         Assert.Equal(123, instance.GetType().GetProperty("F1").GetValue(instance, null));
         Assert.Equal("test", instance.GetType().GetProperty("F2").GetValue(instance, null));
         Assert.Null(instance.GetType().GetProperty("F3").GetValue(instance, null));
      }
   }
}