using BepInEx;
using HarmonyLib;

namespace Archipelago.ARobotNamedFight
{
    [BepInPlugin("com.BetaSprite.Archipelago", "ARNFArch", "0.1.0")]
    [BepInProcess("ARobotNamedFight.exe")]
    public class ArchipelagoPlugin : BaseUnityPlugin
    {
        //internal static string apServerUri = "archipelago.gg";

        //internal static int apServerPort = 38281;

        //private bool willConnectToAP = true;

        //private bool isPlayingAP = false;

        //internal static string apSlotName = "";

        //internal static string apPassword;

        private void Awake()
        {
            Log.Init(this.Logger);

            //ArchipelagoClient.Instance.OnClientDisconnect += AP_OnClientDisconnect;

            // Plugin startup logic
            Log.Debug($"Plugin Archipelago.ARobotNamedFight is loaded!");

            Log.Debug($"Creating Harmony");
            Harmony.DEBUG = true;
            var harmony = new Harmony("Archipelago.ARobotNamedFight");

            Log.Info($"Patching Harmony");
            harmony.PatchAll();
        }

        private void AP_OnClientDisconnect(string reason)
        {
            Log.Warning("Archipelago client was disconnected from the server because `" + reason + "`");
        }
    }
}
