using System;
using System.Reflection;
using Duality.Editor;

namespace Singularity.Duality.Resources
{
    /// <summary>
    /// Represents a weakly typed reference to a <see cref="Type"/>
    /// </summary>
	public sealed class ModuleRef
	{
		/// <summary>
		/// The assembly name. This is the filename of the dll without .dll.
		/// </summary>
		[EditorHintFlags(MemberFlags.AffectsOthers)]
		public string Assembly { get; set; }

		/// <summary>
		/// The namespace where the type resides.
		/// </summary>
		[EditorHintFlags(MemberFlags.AffectsOthers)]
		public string NameSpace { get; set; }

		/// <summary>
		/// The name of the type.
		/// </summary>
		[EditorHintFlags(MemberFlags.AffectsOthers)]
		public string Name { get; set; }

		public Type Type => Type.GetType(ToString());

        public ModuleRef() { }

        public ModuleRef(Type type)
        {
            Assembly = type.GetTypeInfo().Assembly.ManifestModule.Name.Replace(".dll", "");
            NameSpace = type.Namespace;
            Name = type.Name;
        }

		public override string ToString()
		{
			return $"{NameSpace}.{Name},{Assembly}";
		}
	}
}