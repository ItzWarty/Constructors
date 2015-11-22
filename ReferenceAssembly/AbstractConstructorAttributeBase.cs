﻿using System;
using System.Reflection;
using Fody.Constructors.Internal;
using FieldInfo = Fody.Constructors.Internal.FieldInfo;

namespace Fody.Constructors {
    public abstract class AbstractConstructorAttributeBase : Attribute {
        internal AbstractConstructorAttributeBase(Predicate<FieldInfo> condition) {
            Condition = condition;
        }

        public Predicate<FieldInfo> Condition { get; set; }
    }
}
