using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using Singularity.Bindings;

namespace Singularity.Collections
{
    internal class ReadOnlyBindingConfig
    {
        public ReadOnlyCollection<ReadonlyRegistration> Registrations { get; }
        public ReadOnlyDictionary<Type, ReadOnlyCollection<Expression>> Decorators { get; }

        public ReadOnlyBindingConfig(ReadOnlyCollection<ReadonlyRegistration> registrations, ReadOnlyDictionary<Type, ReadOnlyCollection<Expression>> decorators)
        {
            Registrations = registrations ?? throw new ArgumentNullException(nameof(registrations));
            Decorators = decorators ?? throw new ArgumentNullException(nameof(decorators));
        }
    }
}