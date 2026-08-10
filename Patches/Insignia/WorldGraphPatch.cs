using HarmonyLib;

namespace InsigniaProperKeybindsMod.Patches.Insignia;

[HarmonyPatch]
public static class WorldGraphPatch
{
    [HarmonyPatch(typeof(WorldGraph), "SetupPrompts")]
    [HarmonyPrefix]
    public static bool SetupPromptsPrefix()
    {
        MenuPromptRow.Clear();
        MenuPromptRow.Set((Inpt.Btn)ProperButton.MenuZoomOut, "Zoom out");
        MenuPromptRow.Set(WorldGraph.moveButton, "Move");
        MenuPromptRow.Set((Inpt.Btn)ProperButton.MenuZoomIn, "Zoom in");

        return false;
    }

    [HarmonyPatch(typeof(WorldGraph), nameof(WorldGraph.HandleZoomInput))]
    [HarmonyPrefix]
    public static bool HandleZoomInputPrefix(WorldGraph __instance, float[] ___zoomScales)
    {
        if (Inpt.GetDown((Inpt.Btn)ProperButton.MenuZoomOut) && __instance.zoomIndex > 0)
        {
            GlobalAudio.PlayClip(__instance.zoomOutSFX);
            __instance.SetZoom(__instance.zoomIndex - 1);
        }
        if (Inpt.GetDown((Inpt.Btn)ProperButton.MenuZoomIn) && __instance.zoomIndex < ___zoomScales.Length - 1)
        {
            GlobalAudio.PlayClip(__instance.zoomInSFX);
            __instance.SetZoom(__instance.zoomIndex + 1);
        }

        return false;
    }
}