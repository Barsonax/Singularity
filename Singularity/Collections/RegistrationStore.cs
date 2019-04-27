using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Singularity.Bindings;

namespace Singularity.Collections
{
    internal class RegistrationStore
    {
        public Dictionary<Type, Registration> Registrations { get; } = new Dictionary<Type, Registration>();
        public Dictionary<Type, ArrayList<Expression>> Decorators { get; } = new Dictionary<Type, ArrayList<Expression>>();
        internal IModule? CurrentModule;

        public void AddDecorator(Type dependencyType, Expression expression)
        {
            ArrayList<Expression> list = GetOrCreateDecorator(dependencyType);
            list.Add(expression);
        }

        public void AddBinding(Binding binding)
        {
            SinglyLinkedListNode<Type>? currentNode = binding.BindingMetadata.DependencyTypes;
            while (currentNode != null)
            {
                Type type = currentNode.Value;
                if (!Registrations.TryGetValue(type, out Registration registration))
                {
                    registration = new Registration(type, binding);
                    Registrations.Add(type, registration);
                }
                else
                {
                    registration.AddBinding(binding);
                }

                currentNode = currentNode.Next;
            }
        }

        private ArrayList<Expression> GetOrCreateDecorator(Type type)
        {
            if (!Decorators.TryGetValue(type, out ArrayList<Expression> list))
            {
                list = new ArrayList<Expression>();
                Decorators.Add(type, list);
            }

            return list;
        }
    }
}