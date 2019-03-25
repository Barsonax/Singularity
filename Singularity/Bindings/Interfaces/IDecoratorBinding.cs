using System.Linq.Expressions;

namespace Singularity.Bindings
{
    public interface IDecoratorBinding
    {
        Expression? Expression { get; }
    }
}