using Xunit;

namespace Tests {
   public class PEVerifierTests : WeaverTestsBase {
#if(DEBUG)
      [Fact]
      public void PeVerify() {
         Verifier.Verify(assemblyPath, newAssemblyPath);
      }
#endif
   }
}