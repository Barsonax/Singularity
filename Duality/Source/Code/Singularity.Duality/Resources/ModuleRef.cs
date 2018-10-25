using System;
using Duality.Editor;

namespace Singularity.Duality.Resources
{
	public class ModuleRef
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

		public override string ToString()
		{
			return $"{NameSpace}.{Name},{Assembly}";
		}
	}
}