using System;

namespace Fody.Constructors
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequiredArgsConstructorAttribute : Attribute { }
}
