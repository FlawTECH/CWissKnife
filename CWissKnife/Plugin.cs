using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace CWissKnife
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;
        private static readonly Harmony Harmony = new(PluginInfo.PLUGIN_GUID);
        public static ConfigEntry<bool> configToggleInfiniteBattery;
        public static ConfigEntry<bool> configToggleInfiniteCamera;
        public static ConfigEntry<bool> configToggleInfiniteHealth;
        public static ConfigEntry<bool> configToggleInfiniteOxygen;
        public static ConfigEntry<bool> configToggleInfiniteStamina;
        public static ConfigEntry<bool> configToggleSpeedMultiplier;
        public static ConfigEntry<float> configSpeedMultiplier;
        public static ConfigEntry<bool> configToggleNoclip;
        public static bool isRandomLobby = false;
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
            // Fix ConfigurationManager not showing up
            this.gameObject.hideFlags = HideFlags.HideAndDontSave;
            Log = Logger;

            // Configuration
            configToggleInfiniteBattery =
                Config.Bind(
                    "General",
                    "ToggleInfiniteBattery",
                    false,
                    "Toggle infinite battery in items that have one"
                );

            configToggleInfiniteCamera =
                Config.Bind(
                    "General",
                    "ToggleInfiniteCamera",
                    false,
                    "Toggle infinite camera video length. Use with caution"
                );

            configToggleInfiniteHealth =
                Config.Bind(
                    "General",
                    "ToggleInfiniteHealth",
                    false,
                    "Toggle infinite health"
                );
            
            configToggleInfiniteOxygen =
                Config.Bind(
                    "General",
                    "ToggleInfiniteOxygen",
                    false,
                    "Toggle infinite oxygen"
                );

            configToggleInfiniteStamina =
                Config.Bind(
                    "General",
                    "ToggleInfiniteStamina",
                    false,
                    "Toggle infinite stamina"
                );
            
            configToggleSpeedMultiplier =
                Config.Bind(
                    "Speed",
                    "ToggleSpeedMultiplier",
                    false,
                    "Toggle movement speed multiplier"
                );

            configSpeedMultiplier =
                Config.Bind(
                    "Speed",
                    "SpeedMultiplier",
                    2f,
                    new ConfigDescription("Speed multiplier", new AcceptableValueRange<float>(0, 10))
                );

            configToggleNoclip =
                Config.Bind(
                    "Noclip",
                    "ToggleNoclip",
                    false,
                    "Toggle Noclip. Use regular movement keys to move. Crouch to go down / jump to go up. Use movement speed multiplier to move faster"
                );

            // -----------------------------------------
            // Uncomment to dump original vs. patched IL
            // -----------------------------------------
            // MethodBase mb = typeof(PlayerController).GetMethod("Movement", BindingFlags.NonPublic | BindingFlags.Instance);

            // var instructions = PatchProcessor.GetOriginalInstructions(mb);
            // foreach(var instruction in instructions) {
            //     Logger.LogWarning(instruction);
            // }
            
            // Logger.LogError("=====================");

            // var instructions2 = SpeedPatch.Transpiler(instructions, PatchProcessor.CreateILGenerator());
            // foreach(var instruction in instructions2) {
            //     Logger.LogWarning(instruction);
            // }

            Harmony.PatchAll();
        }
    }
}
