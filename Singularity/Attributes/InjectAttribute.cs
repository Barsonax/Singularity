using System;

namespace Singularity.Attributes
{
    /// <summary>
    /// Marks a method so that it can be used for method injection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class InjectAttribute : Attribute { }
}