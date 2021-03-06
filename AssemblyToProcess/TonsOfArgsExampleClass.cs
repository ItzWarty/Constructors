﻿using Fody.Constructors;
using System;

namespace AssemblyToProcess {
   [RequiredFieldsConstructor]
   public class TonsOfArgsExampleClass {
      private static readonly object s0 = new object();
      private static readonly object s1;
      private readonly string f1;
      private readonly int f2;
      private readonly float f3;
      private readonly object f4;
      private readonly DateTime f5;

      public string F1 { get { return f1; } }
      public int F2 { get { return f2; } }
      public float F3 { get { return f3; } }
      public object F4 { get { return f4; } }
      public DateTime F5 { get { return f5; } }
   }
}