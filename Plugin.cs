using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInDependency("sparroh.uilibrary")]
[MycoMod(null, ModFlags.IsClientSide)]
public class SparrohPlugin : BaseUnityPlugin

{
    public const string PluginGUID = "sparroh.amalgamationtracker";
    public const string PluginName = "AmalgamationTracker";
    public const string PluginVersion = "1.0.0";

    internal static new ManualLogSource Logger;

    private Harmony harmony;
    private BossTimer bossTimer;

    private void Awake()
    {
        Logger = base.Logger;

        try
        {
            harmony = new Harmony(PluginGUID);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to create Harmony instance: {ex.Message}");
            return;
        }

        var configFile = Config;
        try
        {
            var watcher = new FileSystemWatcher(Paths.ConfigPath, "sparroh.amalgamationtracker.cfg");
            watcher.Changed += (s, e) =>
            {
                configFile.Reload();
            };
            watcher.EnableRaisingEvents = true;
        }
        catch (Exception ex)
        {
            Logger.LogWarning($"Failed to set up config watcher: {ex.Message}");
        }

        try
        {
            bossTimer = new BossTimer(configFile, harmony);
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to initialize BossTimer: {ex.Message}");
        }

        try
        {
            harmony.PatchAll();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to apply Harmony patches: {ex.Message}");
        }

        Logger.LogInfo($"{PluginName} loaded successfully.");
    }

    private void Update()
    {
        try
        {
            if (bossTimer != null) bossTimer.UpdateHudVisibility();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error in BossTimer.UpdateHudVisibility(): {ex.Message}");
        }

        try
        {
            if (bossTimer != null) bossTimer.Update();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error in BossTimer.Update(): {ex.Message}");
        }
    }

    private void OnDestroy()
    {
        try
        {
            if (bossTimer != null) bossTimer.OnDestroy();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error in BossTimer.OnDestroy(): {ex.Message}");
        }

        try
        {
            if (harmony != null) harmony.UnpatchSelf();
        }
        catch (Exception ex)
        {
            Logger.LogError($"Error unpatching Harmony: {ex.Message}");
        }
    }
}
