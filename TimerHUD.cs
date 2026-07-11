using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;
using System;
using Pigeon.Movement;
using Sparroh.UI;
using TMPro;

public class BossTimerHUD
{
    private ConfigEntry<bool> enableBossTimerHUD;
    private ConfigEntry<float> bossTimerAnchorX;
    private ConfigEntry<float> bossTimerAnchorY;
    private HudHandle hud;
    private readonly ConfigFile configFile;
    private readonly Harmony harmony;

    private bool isTiming = false;
    private float startTime = 0f;
    private float finalTime = 0f;
    private bool hasFinalTime = false;

    public static BossTimerHUD Instance { get; private set; }

    public BossTimerHUD(ConfigFile configFile, Harmony harmony)
    {
        this.configFile = configFile;
        this.harmony = harmony;

        Instance = this;

        try
        {
            enableBossTimerHUD = configFile.Bind("General", "EnableBossTimerHUD", true, "Enables the Amalgamation boss timer HUD display.");
            enableBossTimerHUD.SettingChanged += OnEnableBossTimerHUDChanged;

            bossTimerAnchorX = configFile.Bind("HUD Positioning", "BossTimerAnchorX", 0.8229749f, "X anchor position for Boss Timer (0-1).");
            bossTimerAnchorY = configFile.Bind("HUD Positioning", "BossTimerAnchorY", 0.9050629f, "Y anchor position for Boss Timer (0-1).");
            bossTimerAnchorX.SettingChanged += OnAnchorChanged;
            bossTimerAnchorY.SettingChanged += OnAnchorChanged;
        }
        catch (Exception ex)
        {
            SparrohPlugin.Logger.LogError($"Failed to initialize BossTimerHUD: {ex.Message}");
        }
    }

    private bool IsHudAlive => hud != null && hud.GameObject != null && hud.Primary != null;

    public bool IsActive => IsHudAlive && hud.IsActive;
    public Vector2 GetSize => IsHudAlive ? hud.Size : Vector2.zero;

    public void UpdateHudVisibility()
    {
        if (!IsHudAlive)
        {
            ClearDestroyedHud();
            return;
        }

        hud.SetActive(enableBossTimerHUD.Value);
    }

    private void OnEnableBossTimerHUDChanged(object sender, EventArgs e)
    {
        if (enableBossTimerHUD.Value == false && hud != null)
            DestroyHud();
        UpdateHudVisibility();
    }

    private void OnAnchorChanged(object sender, EventArgs e)
    {
        if (IsHudAlive)
            hud.SetAnchor(bossTimerAnchorX.Value, bossTimerAnchorY.Value);
    }

    private void ClearDestroyedHud()
    {
        if (hud == null) return;
        try
        {
            if (hud.Rect != null)
                HudRepositionClient.Unregister(SparrohPlugin.PluginGUID);
        }
        catch { /* ignore */ }
        hud = null;
    }

    private void CreateTimerHUD()
    {
        if (IsHudAlive) return;
        ClearDestroyedHud();

        hud = HudBuilder.Create("BossTimerHUD")
            .ParentToReticle()
            .Anchor(bossTimerAnchorX.Value, bossTimerAnchorY.Value)
            .Pivot(new Vector2(0.5f, 0.5f))
            .Size(350f, 28f)
            .AddText("TimerText", fontSize: 20f, alignment: TextAlignmentOptions.Center)
            .Build();

        if (!IsHudAlive)
            return;

        HudRepositionClient.Register(
            SparrohPlugin.PluginGUID,
            "Boss Timer",
            hud.Rect,
            bossTimerAnchorX,
            bossTimerAnchorY);

        UpdateHudVisibility();
    }

    private void DestroyHud()
    {
        HudRepositionClient.Unregister(SparrohPlugin.PluginGUID);
        if (hud != null)
        {
            if (hud.GameObject != null)
                hud.Destroy();
            hud = null;
        }
    }

    public void StartTimer()
    {
        isTiming = true;
        startTime = Time.realtimeSinceStartup;
        finalTime = 0f;
        hasFinalTime = false;
        SparrohPlugin.Logger.LogInfo("Boss timer started");
    }

    public void StopTimer()
    {
        if (isTiming)
        {
            finalTime = Time.realtimeSinceStartup - startTime;
            isTiming = false;
            hasFinalTime = true;
            SparrohPlugin.Logger.LogInfo($"Boss timer stopped: {FormatTime(finalTime)}");
        }
    }

    public void ResetTimer()
    {
        isTiming = false;
        startTime = 0f;
        finalTime = 0f;
        hasFinalTime = false;
        SparrohPlugin.Logger.LogInfo("Boss timer reset");
    }

    public void Update()
    {
        try
        {
            if (enableBossTimerHUD == null || !enableBossTimerHUD.Value)
                return;

            if (hud != null && !IsHudAlive)
                ClearDestroyedHud();

            if (Player.LocalPlayer == null || Player.LocalPlayer.PlayerLook == null || Player.LocalPlayer.PlayerLook.Reticle == null)
                return;

            if (!IsHudAlive)
            {
                CreateTimerHUD();
                return;
            }

            if (hasFinalTime)
                hud.Primary.SetRich("Amalgamation", FormatTime(finalTime), UIColors.Amber);
            else if (isTiming)
                hud.Primary.SetRich("Amalgamation", FormatTime(Time.realtimeSinceStartup - startTime), UIColors.Amber);
            else
                hud.Primary.Text = "Amalgamation: Waiting...";
        }
        catch (Exception ex)
        {
            SparrohPlugin.Logger.LogError($"Error in BossTimerHUD.Update(): {ex.Message}");
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = (int)(timeInSeconds / 60f);
        int seconds = (int)(timeInSeconds % 60f);
        int milliseconds = (int)((timeInSeconds % 1f) * 1000f);
        return $"{minutes:D2}:{seconds:D2}.{milliseconds:D3}";
    }

    public void OnDestroy()
    {
        try
        {
            DestroyHud();
        }
        catch (Exception ex)
        {
            SparrohPlugin.Logger.LogError($"Error in BossTimerHUD.OnDestroy(): {ex.Message}");
        }
    }
}
