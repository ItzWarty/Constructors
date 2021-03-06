﻿using AssemblyToProcess;
using System;
using System.Linq;
using Xunit;

namespace Tests {
   public class MultiConstructorTests_2 : WeaverTestsBase {
      public MultiConstructorTests_2() : base(typeof(MultiConstructorExampleClass2)) { }

      [Fact]
      public void VerifyMultiConstructor2_DefaultConstructor() {
         var addedCtor = type.GetConstructors().First(c => c.GetParameters().Length == 0);
         var instance = addedCtor.Invoke(null);

         Assert.Null(instance.GetType().GetProperty("F1").GetValue(instance, null));
         Assert.Null(instance.GetType().GetProperty("F2").GetValue(instance, null));
      }

      [Fact]
      public void VerifyMultiConstructor2_RequiredArgsConstructor() {
         var addedCtor = type.GetConstructors().First(c => c.GetParameters().Length > 0);
         var instance = addedCtor.Invoke(new[] { "test", "asdf" });

         Assert.Equal("test", instance.GetType().GetProperty("F1").GetValue(instance, null));
         Assert.Equal("asdf", instance.GetType().GetProperty("F2").GetValue(instance, null));
      }

      [Fact]
      public void VerifyMultiConstructor2_HasOnlyTwoConstructors() {
         foreach (var ctor in type.GetConstructors()) {
            Console.WriteLine(ctor);
         }
         Assert.Equal(2, type.GetConstructors().Length);
      }
   }
}