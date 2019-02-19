using System;
using System.Linq.Expressions;

namespace Singularity.Bindings
{
    public interface IDecoratorBinding
    {
        Type DependencyType { get; }
        Expression? Expression { get; }
    }
}