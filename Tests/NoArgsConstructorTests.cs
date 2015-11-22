using AssemblyToProcess;
using System.Linq;
using Xunit;

namespace Tests {
   public class NoArgsConstructorTests : WeaverTestsBase {
      public NoArgsConstructorTests() : base(typeof(NoArgsExampleClass)) { }

      [Fact]
      public void ValidateNoArgsConstructor() {
         var addedCtor = type.GetConstructors().First(c => c.GetParameters().Length == 0);
         var instance = addedCtor.Invoke(null);

         Assert.Null(instance.GetType().GetProperty("F1").GetValue(instance, null));
         Assert.Equal("asdf", instance.GetType().GetProperty("F2").GetValue(instance, null));
      }
   }
}