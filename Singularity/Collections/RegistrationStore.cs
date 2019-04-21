using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Bindings;
using Singularity.Exceptions;

namespace Singularity.Collections
{
    internal class RegistrationStore : IReadOnlyCollection<Registration>
    {
        public bool Locked => _readonlyBindings != null;
        private ReadOnlyCollection<ReadonlyRegistration>? _readonlyBindings;
        public Dictionary<Type, Registration> Registrations { get; } = new Dictionary<Type, Registration>();
        public int Count => Registrations.Count;
        internal IModule? CurrentModule;

        public WeaklyTypedDecoratorBinding CreateDecorator(Type dependencyType, Type decoratorType)
        {
            if (Locked) throw new BindingConfigException("This config is locked and cannot be modified anymore!");
            TypeInfo typeInfo = decoratorType.GetTypeInfo();
            if (!dependencyType.GetTypeInfo().IsAssignableFrom(typeInfo)) throw new TypeNotAssignableException($"{dependencyType} is not implemented by {decoratorType}");
            var decorator = new WeaklyTypedDecoratorBinding(dependencyType, decoratorType.AutoResolveConstructorExpression());

            ParameterExpression[] parameters = decorator.Expression.GetParameterExpressions();
            if (parameters.All(x => x.Type != dependencyType)) throw new InvalidExpressionArgumentsException($"Cannot decorate {dependencyType} since the expression to create {decoratorType} does not have a parameter for {dependencyType}");
            Registration registration = GetOrCreateRegistration(dependencyType);
            registration.DecoratorBindings.Add(decorator);
            return decorator;
        }

        public WeaklyTypedBinding CreateBinding(Type dependencyType, string callerFilePath, int callerLineNumber)
        {
            if (Locked) throw new BindingConfigException("This config is locked and cannot be modified anymore!");
            Registration registration = GetOrCreateRegistration(dependencyType);
            var binding = new WeaklyTypedBinding(dependencyType, callerFilePath, callerLineNumber, CurrentModule);
            registration.Bindings.Add(binding);
            return binding;
        }

        public ReadOnlyCollection<ReadonlyRegistration> GetDependencies()
        {
            if (_readonlyBindings == null)
            {
                var readonlyRegistrations = new ReadonlyRegistration[Registrations.Count];
                var count = 0;
                foreach (Registration registration in Registrations.Values)
                {
                    registration.Verify();

                    readonlyRegistrations[count] = new ReadonlyRegistration(registration.DependencyType, registration.Bindings.Select(x => new Binding(x)), registration.DecoratorBindings.Select(x => x.Expression));
                    count++;
                }

                _readonlyBindings = new ReadOnlyCollection<ReadonlyRegistration>(readonlyRegistrations);
            }
            return _readonlyBindings!;
        }

        public IEnumerator<Registration> GetEnumerator()
        {
            return Registrations.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private Registration GetOrCreateRegistration(Type type)
        {
            if (!Registrations.TryGetValue(type, out Registration registration))
            {
                registration = new Registration(type);
                Registrations.Add(type, registration);
            }

            return registration;
        }
    }
}