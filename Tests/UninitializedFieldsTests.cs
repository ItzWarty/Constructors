using AssemblyToProcess;
using System.Linq;
using Xunit;

namespace Tests {
   public class UninitializedFieldsTests : WeaverTestsBase {
      public UninitializedFieldsTests() : base(typeof(UninitializedFieldsExampleClass)) { }

      [Fact]
      public void UninitializedFieldsConstructor_Interface() {
         var addedCtor = type.GetConstructors().First(c => c.GetParameters().Length > 0);
         var parameters = addedCtor.GetParameters();

         Assert.Equal(2, parameters.Length);
         Assert.Equal("f1", parameters[0].Name);
         Assert.Equal(typeof(object), parameters[0].ParameterType);
         Assert.Equal("f2", parameters[1].Name);
         Assert.Equal(typeof(string), parameters[1].ParameterType);
      }

      [Fact]
      public void UninitializedFieldsConstructor_Properties() {
         var addedCtor = type.GetConstructors().First(c => c.GetParameters().Length > 0);
         var instance = addedCtor.Invoke(new[] { "test", "asdf" });

         Assert.Equal("test", instance.GetType().GetProperty("F1").GetValue(instance, null));
         Assert.Equal("asdf", instance.GetType().GetProperty("F2").GetValue(instance, null));
      }
   }
}