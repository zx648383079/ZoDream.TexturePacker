using ZoDream.Shared.Interfaces;

namespace ZoDream.Plugin.Unity
{
    public static class Extension
    {

        public static void AddUnity(this IPluginCollection service)
        {
            service.Add<AssetReader>("import", "Unity Aseet");
            service.Add<AtlasReader>("import", "Unity Atlas");
            service.Add<SheetReader>("import", "Unity Atlas Sheet");
        }
    }
}
