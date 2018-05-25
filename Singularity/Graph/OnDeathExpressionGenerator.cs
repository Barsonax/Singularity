using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Bindings;
using Singularity.Collections;

namespace Singularity.Graph
{
    public class OnDeathExpressionGenerator : IDependencyExpressionGenerator
    {
        private readonly ObjectActionContainer _objectActionContainer;
        private readonly MethodInfo _addMethod;

        public OnDeathExpressionGenerator(ObjectActionContainer objectActionContainer)
        {
            _objectActionContainer = objectActionContainer;
            _addMethod = typeof(ObjectActionContainer).GetRuntimeMethod(nameof(ObjectActionContainer.Add), new[] { typeof(object) });
        }

        public void Generate(IBinding binding, ParameterExpression instanceParameter, List<ParameterExpression> parameters, List<Expression> body)
        {
            if (binding.ConfiguredBinding.OnDeath != null)
            {
                _objectActionContainer.AddAction(binding.ConfiguredBinding.Expression.Type, binding.ConfiguredBinding.OnDeath);
                body.Add(Expression.Call(Expression.Constant(_objectActionContainer), _addMethod, instanceParameter));
            }
        }
    }
}