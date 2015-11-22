using Mono.Cecil;
using System;
using System.IO;
using System.Reflection;

namespace Tests {
   public abstract class WeaverTestsBase {
      protected readonly Assembly assembly;
      protected readonly string assemblyPath;
      protected readonly string newAssemblyPath;
      protected readonly Type type;

      protected WeaverTestsBase() {
         assembly = OnceWeaver.Assembly;
         assemblyPath = OnceWeaver.AssemblyPath;
         newAssemblyPath = OnceWeaver.NewAssemblyPath;
      }

      protected WeaverTestsBase(Type testType) : this() {
         type = assembly.GetType(testType.FullName);
      }

      public static class OnceWeaver {
         public static Assembly Assembly { get; private set; }
         public static string AssemblyPath { get; private set; }
         public static string NewAssemblyPath { get; private set; }

         static OnceWeaver() {
            var projectPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\AssemblyToProcess\AssemblyToProcess.csproj"));
            var assemblyPath = Path.Combine(Path.GetDirectoryName(projectPath), @"bin\Debug\AssemblyToProcess.dll");
#if (!DEBUG)
        assemblyPath = assemblyPath.Replace("Debug", "Release");
#endif

            var newAssemblyPath = assemblyPath.Replace(".dll", "2.dll");
            File.Copy(assemblyPath, newAssemblyPath, true);

            var moduleDefinition = ModuleDefinition.ReadModule(newAssemblyPath);
            var weavingTask = new ModuleWeaver {
               ModuleDefinition = moduleDefinition
            };

            weavingTask.Execute();
            moduleDefinition.Write(newAssemblyPath);

            Assembly = Assembly.LoadFile(newAssemblyPath);
            AssemblyPath = assemblyPath;
            NewAssemblyPath = newAssemblyPath;
         }
      }
   }
}
