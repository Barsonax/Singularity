using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Collections;
using Singularity.Graph;

namespace Singularity
{
    [DebuggerDisplay("{Expression?.Type}")]
    internal class Binding
    {
        public BindingMetadata BindingMetadata { get; }
        public Expression Expression { get; }

        public Lifetime Lifetime { get; }
        public DisposeBehavior NeedsDispose { get; }
        public Action<object>? Finalizer { get; }


        public Expression? BaseExpression { get; set; }
        public Exception? ResolveError { get; set; }

        public ArrayList<InstanceFactory> Factories { get; } = new ArrayList<InstanceFactory>();

        public bool TryGetInstanceFactory(Type type, out InstanceFactory factory)
        {
            factory = Factories.Array.FirstOrDefault(x => x.DependencyType == type);
            return factory != null;
        }

        public Binding(BindingMetadata bindingMetadata, Expression expression, Lifetime lifetime = Lifetime.Transient, Action<object>? finalizer = null, DisposeBehavior needsDispose = DisposeBehavior.Default)
        {
            BindingMetadata = bindingMetadata ?? throw new ArgumentNullException(nameof(bindingMetadata));
            Lifetime = lifetime;
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            NeedsDispose = needsDispose;
            Finalizer = finalizer;
        }
    }
}
