using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

using Singularity.Collections;

namespace Singularity.Graph
{
    internal sealed class OnDeathExpressionGenerator : IDependencyExpressionGenerator
    {
        private readonly ObjectActionContainer _objectActionContainer;
        private static readonly MethodInfo _addMethod = typeof(ObjectActionContainer).GetRuntimeMethod(nameof(ObjectActionContainer.Add), new[] { typeof(object) });

        public OnDeathExpressionGenerator(ObjectActionContainer objectActionContainer)
        {
            _objectActionContainer = objectActionContainer;
        }

        public void Generate(UnresolvedDependency binding, ParameterExpression instanceParameter, List<ParameterExpression> parameters, List<Expression> body)
        {
            if (binding.OnDeath != null)
            {
                _objectActionContainer.RegisterAction(binding.Expression.Type, binding.OnDeath);
                body.Add(Expression.Call(Expression.Constant(_objectActionContainer), _addMethod, instanceParameter));
            }
        }
    }
}