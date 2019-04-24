namespace Singularity
{
    public class SingularitySettings
    {
        internal static readonly SingularitySettings Default = new SingularitySettings();
        internal static readonly SingularitySettings Microsoft = new SingularitySettings { AutoDispose = true };

        public bool AutoDispose { get; set; }
    }
}
