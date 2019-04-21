using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Singularity.Bindings;
using Singularity.Collections;

namespace Singularity.Graph
{
    internal sealed class Dependency
    {
        public ReadonlyRegistration Registration { get; }
        public ArrayList<ResolvedDependency> ResolvedDependencies { get; }
        public ResolvedDependency Default { get; }

        public Dependency(ReadonlyRegistration registration)
        {
            Registration = registration ?? throw new ArgumentNullException(nameof(registration));
            ResolvedDependencies = new ArrayList<ResolvedDependency>();
            foreach (Binding binding in registration.Bindings)
            {
                ResolvedDependencies.Add(new ResolvedDependency(registration, binding));
            }

            Default = ResolvedDependencies.Array.LastOrDefault();
        }

        public Dependency(Type type, Lifetime lifetime) :
            this(new ReadonlyRegistration(type, new Binding(new BindingMetadata(type), type.AutoResolveConstructorExpression(), lifetime, null, DisposeBehavior.Default), new Expression[0]))
        {

        }

        public Dependency(Type type, Expression expression, Lifetime lifetime) :
            this(new ReadonlyRegistration(type, new Binding(new BindingMetadata(type), expression, lifetime, null, DisposeBehavior.Default), new Expression[0]))
        {

        }

        public Dependency(Type type, IEnumerable<Expression> expressions, Lifetime lifetime) :
            this(new ReadonlyRegistration(type, expressions.Select(expression => new Binding(new BindingMetadata(type), expression, lifetime, null, DisposeBehavior.Default)), new Expression[0]))
        {

        }
    }

    internal sealed class ResolvedDependency
    {
        public ReadonlyRegistration Registration { get; }
        public Binding Binding { get; }
        public Dependency[]? Children { get; set; }
        public Expression? Expression { get; set; }
        public Func<Scoped, object>? InstanceFactory { get; set; }
        public Exception? ResolveError { get; set; }

        public ResolvedDependency(ReadonlyRegistration registration, Binding binding)
        {
            Registration = registration;
            Binding = binding;
        }
    }
}