using System;
using System.Linq.Expressions;

namespace Singularity
{
    public interface IDecoratorBinding
    {
        Type DependencyType { get; }
        Expression Expression { get; set; }
    }
}