using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Singularity.Bindings;
using Singularity.Exceptions;

namespace Singularity.Collections
{
    internal class RegistrationStore
    {
        public bool Locked => _readonlyBindings != null;
        private ReadOnlyBindingConfig? _readonlyBindings;
        public Dictionary<Type, Registration> Registrations { get; } = new Dictionary<Type, Registration>();
        public Dictionary<Type, List<WeaklyTypedDecoratorBinding>> Decorators { get; } = new Dictionary<Type, List<WeaklyTypedDecoratorBinding>>();
        internal IModule? CurrentModule;

        public WeaklyTypedDecoratorBinding CreateDecorator(Type dependencyType, Type decoratorType)
        {
            if (Locked) throw new BindingConfigException("This config is locked and cannot be modified anymore!");
            TypeInfo typeInfo = decoratorType.GetTypeInfo();
            if (!dependencyType.GetTypeInfo().IsAssignableFrom(typeInfo)) throw new TypeNotAssignableException($"{dependencyType} is not implemented by {decoratorType}");
            var decorator = new WeaklyTypedDecoratorBinding(dependencyType, decoratorType.AutoResolveConstructorExpression());

            ParameterExpression[] parameters = decorator.Expression.GetParameterExpressions();
            if (parameters.All(x => x.Type != dependencyType)) throw new InvalidExpressionArgumentsException($"Cannot decorate {dependencyType} since the expression to create {decoratorType} does not have a parameter for {dependencyType}");
            List<WeaklyTypedDecoratorBinding> decorators = GetOrCreateDecorator(dependencyType);
            decorators.Add(decorator);
            return decorator;
        }

        public WeaklyTypedBinding CreateBinding(Type[] dependencyTypes, string callerFilePath, int callerLineNumber)
        {
            if (Locked) throw new BindingConfigException("This config is locked and cannot be modified anymore!");
            Registration registration = GetOrCreateRegistration(dependencyTypes);

            var binding = new WeaklyTypedBinding(dependencyTypes, callerFilePath, callerLineNumber, CurrentModule);
            registration.Bindings = new SinglyLinkedListNode<WeaklyTypedBinding>(registration.Bindings, binding);
            return binding;
        }

        public ReadOnlyBindingConfig GetDependencies()
        {
            if (_readonlyBindings == null)
            {
                Registration[] uniqueRegistrations = Registrations.Values.Distinct().ToArray();
                var registrations = new ReadonlyRegistration[uniqueRegistrations.Length];
                var count = 0;
                foreach (Registration registration in uniqueRegistrations)
                {
                    foreach (Type registrationDependencyType in registration.DependencyTypes)
                    {
                        if (Decorators.TryGetValue(registrationDependencyType, out List<WeaklyTypedDecoratorBinding> decorators))
                        {
                            registration.Verify(decorators);
                        }
                        else
                        {
                            registration.Verify(null);
                        }
                    }

                    registrations[count] = new ReadonlyRegistration(registration.DependencyTypes, new ReadOnlyBindingCollection(registration.Bindings.Select(x => new Binding(x))));
                    count++;
                }

                var readonlyRegistrations = new ReadOnlyCollection<ReadonlyRegistration>(registrations);
                var readonlyDecorators = new Dictionary<Type, ReadOnlyCollection<Expression>>(Decorators.Count);
                foreach (KeyValuePair<Type, List<WeaklyTypedDecoratorBinding>> keyValuePair in Decorators)
                {
                    readonlyDecorators.Add(keyValuePair.Key, new ReadOnlyCollection<Expression>(keyValuePair.Value.Select(x => x.Expression).ToArray()));
                }
                _readonlyBindings = new ReadOnlyBindingConfig(readonlyRegistrations, new ReadOnlyDictionary<Type, ReadOnlyCollection<Expression>>(readonlyDecorators));
            }
            return _readonlyBindings!;
        }

        private Registration GetOrCreateRegistration(Type[] types)
        {
            Registration registration;
            foreach (Type type in types)
            {
                if (Registrations.TryGetValue(type, out registration))
                {
                    if (!types.CollectionsAreEqual(registration.DependencyTypes))
                    {
                        throw new RegistrationCollisionException(registration.DependencyTypes, types);
                    }

                    return registration;
                }
            }
            registration = new Registration(types);
            foreach (Type type in types)
            {
                Registrations.Add(type, registration);
            }

            return registration;
        }

        private List<WeaklyTypedDecoratorBinding> GetOrCreateDecorator(Type type)
        {
            if (!Decorators.TryGetValue(type, out List<WeaklyTypedDecoratorBinding> list))
            {
                list = new List<WeaklyTypedDecoratorBinding>();
                Decorators.Add(type, list);
            }

            return list;
        }
    }
}