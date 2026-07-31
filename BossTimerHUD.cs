using System;
using Pigeon.Movement;
using Sparroh.UI;
using TMPro;
using UnityEngine;

public class BossTimerHUD
{
    private float finalTime;
    private bool hasFinalTime;
    private HudHandle hud;

    private bool isTiming;
    private float startTime;

    public BossTimerHUD()
    {
        Instance = this;
    }

    public static BossTimerHUD Instance { get; private set; }

    public bool IsActive => HudHandle.IsValid(hud) && hud.IsActive;
    public Vector2 GetSize => HudHandle.IsValid(hud) ? hud.Size : Vector2.zero;

    public void OnConfigChanged()
    {
        if (!ConfigManager.EnableBossTimerHUD.Value && HudHandle.IsValid(hud))
            DestroyHud();
        UpdateHudVisibility();
    }

    public void UpdateHudVisibility()
    {
        if (HudHandle.IsValid(hud))
            hud.SetActive(ConfigManager.EnableBossTimerHUD.Value);
    }

    private void CreateTimerHUD()
    {
        if (hud != null && !hud.IsAlive)
            hud = null;

        if (HudHandle.IsValid(hud)) return;

        hud = HudBuilder.Create("AmalgamationTimerHUD")
            .ParentToReticle()
            .Anchor(ConfigManager.Anchors.XValue, ConfigManager.Anchors.YValue)
            .Pivot(new Vector2(0.5f, 0.5f))
            .Size(350f, 28f)
            .AddText("TimerText", 20f, TextAlignmentOptions.Center)
            .Build();

        if (!HudHandle.IsValid(hud))
            return;

        hud.EnableReposition(SparrohPlugin.PluginGUID, "Amalgamation Timer", ConfigManager.Anchors);
        UpdateHudVisibility();
    }

    private void DestroyHud()
    {
        if (hud != null)
        {
            if (hud.IsAlive)
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
        SparrohPlugin.Logger.LogInfo("Amalgamation timer started");
    }

    public void StopTimer()
    {
        if (isTiming)
        {
            finalTime = Time.realtimeSinceStartup - startTime;
            isTiming = false;
            hasFinalTime = true;
            SparrohPlugin.Logger.LogInfo($"Amalgamation timer stopped: {FormatTime(finalTime)}");
        }
    }

    public void ResetTimer()
    {
        isTiming = false;
        startTime = 0f;
        finalTime = 0f;
        hasFinalTime = false;
        SparrohPlugin.Logger.LogInfo("Amalgamation timer reset");
    }

    public void Update()
    {
        try
        {
            if (!ConfigManager.EnableBossTimerHUD.Value)
                return;

            if (!HudHandle.IsValid(hud))
            {
                CreateTimerHUD();
                return;
            }

            if (hud.Primary == null || Player.LocalPlayer == null) return;

            if (hasFinalTime)
                hud.Primary.SetRich("Amalgamation", FormatTime(finalTime), ConfigManager.ValueColor.Value);
            else if (isTiming)
                hud.Primary.SetRich("Amalgamation", FormatTime(Time.realtimeSinceStartup - startTime),
                    ConfigManager.ValueColor.Value);
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
        var minutes = (int)(timeInSeconds / 60f);
        var seconds = (int)(timeInSeconds % 60f);
        var milliseconds = (int)(timeInSeconds % 1f * 1000f);
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