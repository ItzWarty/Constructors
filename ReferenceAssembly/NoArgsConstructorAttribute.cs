using System;

namespace Fody.Constructors {
    [AttributeUsage(AttributeTargets.Class)]
    public class NoArgsConstructorAttribute : Attribute { }
}