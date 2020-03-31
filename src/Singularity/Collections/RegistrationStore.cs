using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Singularity.Collections
{
    internal sealed class RegistrationStore
    {
        public Dictionary<Type, Registration> Registrations { get; } = new Dictionary<Type, Registration>();
        public Dictionary<Type, ArrayList<Expression>> Decorators { get; } = new Dictionary<Type, ArrayList<Expression>>();
        public Dictionary<Type, ArrayList<LateInjectorBinding>> LateInjectorBindings { get; } = new Dictionary<Type, ArrayList<LateInjectorBinding>>();

        internal IModule? CurrentModule;

        public void AddDecorator(Type serviceType, Expression expression)
        {
            if (!Decorators.TryGetValue(serviceType, out ArrayList<Expression> list))
            {
                list = new ArrayList<Expression>();
                Decorators.Add(serviceType, list);
            }
            list.Add(expression);
        }

        public void AddBinding(ServiceBinding serviceBinding)
        {
            SinglyLinkedListNode<Type>? currentNode = serviceBinding.ServiceTypes;
            while (currentNode != null)
            {
                Type type = currentNode.Value;
                if (!Registrations.TryGetValue(type, out Registration registration))
                {
                    registration = new Registration(serviceBinding);
                    Registrations.Add(type, registration);
                }
                else
                {
                    registration.AddBinding(serviceBinding);
                }

                currentNode = currentNode.Next;
            }
        }

        public void AddLateInjectorBinding(LateInjectorBinding lateInjectorBinding)
        {
            if (!LateInjectorBindings.TryGetValue(lateInjectorBinding.InstanceType, out ArrayList<LateInjectorBinding> list))
            {
                list = new ArrayList<LateInjectorBinding>();
                LateInjectorBindings.Add(lateInjectorBinding.InstanceType, list);
            }
            list.Add(lateInjectorBinding);
        }
    }
}