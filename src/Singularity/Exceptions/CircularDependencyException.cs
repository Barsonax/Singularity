using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Singularity.Exceptions
{
    /// <summary>
    /// Thrown when a circular dependency has been found.
    /// </summary>
    [Serializable]
    public sealed class CircularDependencyException : SingularityException
    {
        /// <summary>
        /// The assembly qualified names of the types that are in the cycle.
        /// </summary>
		public string[] Cycle { get; }

        internal CircularDependencyException(IReadOnlyList<Type> cycle, Exception? inner = null) : base(GetMessage(cycle), inner)
        {
            Cycle = cycle.Select(x => x.AssemblyQualifiedName).ToArray();
        }

        private static string GetMessage(IReadOnlyList<Type> cycle)
        {
            return $"{cycle.First()} has circular dependencies! ({string.Join("->", cycle.Select(x => x.Name))})";
        }

        /// <summary>
        /// Deserialization constructor
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private CircularDependencyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Cycle = (string[])info.GetValue(nameof(Cycle), typeof(string[]));
        }

        /// <summary>
        /// Needed for serialization
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null) throw new ArgumentNullException(nameof(info));
            info.AddValue(nameof(Cycle), Cycle);
            base.GetObjectData(info, context);
        }
    }
}