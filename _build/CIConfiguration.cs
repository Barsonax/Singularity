using Nuke.Common;

public class CiConfiguration : Configuration
{
    public static Configuration CiConfig = new CiConfiguration();

    private CiConfiguration()
    {
        Value = "z_CI_config";
    }
}