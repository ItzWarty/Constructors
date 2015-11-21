using Mono.Cecil;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

[TestFixture]
public class WeaverTests
{
    Assembly assembly;
    string newAssemblyPath;
    string assemblyPath;

    [TestFixtureSetUp]
    public void Setup()
    {
        var projectPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\AssemblyToProcess\AssemblyToProcess.csproj"));
        assemblyPath = Path.Combine(Path.GetDirectoryName(projectPath), @"bin\Debug\AssemblyToProcess.dll");
#if (!DEBUG)
        assemblyPath = assemblyPath.Replace("Debug", "Release");
#endif

        newAssemblyPath = assemblyPath.Replace(".dll", "2.dll");
        File.Copy(assemblyPath, newAssemblyPath, true);

        var moduleDefinition = ModuleDefinition.ReadModule(newAssemblyPath);
        var weavingTask = new ModuleWeaver
        {
            ModuleDefinition = moduleDefinition
        };

        weavingTask.Execute();
        moduleDefinition.Write(newAssemblyPath);

        assembly = Assembly.LoadFile(newAssemblyPath);
    }

    [Test]
    public void ValidateRequiredArgsConstructor_Interface() {
        var type = assembly.GetType("AssemblyToProcess.RequiredArgsExampleClass");
        var addedCtor = type.GetConstructors().First(c => c.GetParameters().Length > 0);
        var parameters = addedCtor.GetParameters();

        Assert.AreEqual(1, parameters.Length);
        Assert.AreEqual("f1", parameters[0].Name);
        Assert.AreEqual(typeof(object), parameters[0].ParameterType);
    }

    [Test]
    public void ValidateRequiredArgsConstructor_Properties() {
        var type = assembly.GetType("AssemblyToProcess.RequiredArgsExampleClass");
        var addedCtor = type.GetConstructors().First(c => c.GetParameters().Length > 0);
        var instance = addedCtor.Invoke(new[] { "test" });

        Assert.AreEqual("test", instance.GetType().GetProperty("F1").GetValue(instance, null));
        Assert.IsNull(instance.GetType().GetProperty("F2").GetValue(instance, null));
    }

    [Test]
    public void ValidateAllArgsConstructor_Interface() {
        var type = assembly.GetType("AssemblyToProcess.AllArgsExampleClass");
        var addedCtor = type.GetConstructors().First(c => c.GetParameters().Length > 0);
        var parameters = addedCtor.GetParameters();

        Assert.AreEqual(2, parameters.Length);
        Assert.AreEqual("f1", parameters[0].Name);
        Assert.AreEqual(typeof(object), parameters[0].ParameterType);
        Assert.AreEqual("f2", parameters[1].Name);
        Assert.AreEqual(typeof(string), parameters[1].ParameterType);
    }

    [Test]
    public void ValidateAllArgsConstructor_Properties() {
        var type = assembly.GetType("AssemblyToProcess.AllArgsExampleClass");
        var addedCtor = type.GetConstructors().First(c => c.GetParameters().Length > 0);
        var instance = addedCtor.Invoke(new[] { "test", "asdf" });

        Assert.AreEqual("test", instance.GetType().GetProperty("F1").GetValue(instance, null));
        Assert.AreEqual("asdf", instance.GetType().GetProperty("F2").GetValue(instance, null));
    }

    [Test]
    public void ValidateNoArgsConstructor() {
        var type = assembly.GetType("AssemblyToProcess.NoArgsExampleClass");
        var addedCtor = type.GetConstructors().First(c => c.GetParameters().Length == 0);
        var instance = addedCtor.Invoke(null);

        Assert.IsNull(instance.GetType().GetProperty("F1").GetValue(instance, null));
        Assert.IsNull(instance.GetType().GetProperty("F2").GetValue(instance, null));
    }

    [Test]
    public void TonsOfAllArgsConstructor_Properties() {
        var injectedObject = new object();
        var injectedTime = DateTime.Now;

        var type = assembly.GetType("AssemblyToProcess.TonsOfArgsExampleClass");
        var addedCtor = type.GetConstructors().First(c => c.GetParameters().Length > 0);
        var instance = addedCtor.Invoke(new[] { "test", 1337, 1.337f, injectedObject, injectedTime });

        Assert.AreEqual("test", instance.GetType().GetProperty("F1").GetValue(instance, null));
        Assert.AreEqual(1337, instance.GetType().GetProperty("F2").GetValue(instance, null));
        Assert.AreEqual(1.337f, instance.GetType().GetProperty("F3").GetValue(instance, null));
        Assert.AreEqual(injectedObject, instance.GetType().GetProperty("F4").GetValue(instance, null));
        Assert.AreEqual(injectedTime, instance.GetType().GetProperty("F5").GetValue(instance, null));
    }

#if(DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(assemblyPath,newAssemblyPath);
    }
#endif
}