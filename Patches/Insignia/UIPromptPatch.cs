using HarmonyLib;

namespace InsigniaProperKeybindsMod.Patches.Insignia;

[HarmonyPatch]
public static class UIPromptPatch
{
    [HarmonyPatch(typeof(UIPrompt), nameof(UIPrompt.CheckInput))]
    [HarmonyPrefix]
    public static bool CheckInputPrefix(UIPrompt __instance)
    {
        if (Inpt.usingGamepad)
            return true;

        __instance.label.sprite = GameSystem.keyCodeToSpriteDictionary[Inpt.GetKeyCode(Inpt.playerOne.id, Utils.GetActionName(__instance.button))];
        __instance.background.enabled = true;

        return false;
    }
}