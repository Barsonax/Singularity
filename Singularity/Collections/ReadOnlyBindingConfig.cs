using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Singularity.Bindings;

namespace Singularity.Collections
{
    internal class ReadOnlyBindingConfig
    {
        public Dictionary<Type, Registration> Registrations { get; }
        public ReadOnlyDictionary<Type, ReadOnlyCollection<Expression>> Decorators { get; }

        public ReadOnlyBindingConfig(Dictionary<Type, Registration> registrations, ReadOnlyDictionary<Type, ReadOnlyCollection<Expression>> decorators)
        {
            Registrations = registrations ?? throw new ArgumentNullException(nameof(registrations));
            Decorators = decorators ?? throw new ArgumentNullException(nameof(decorators));
        }
    }
}