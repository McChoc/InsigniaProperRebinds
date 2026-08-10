using HarmonyLib;

namespace InsigniaProperKeybindsMod.Patches.Insignia;

[HarmonyPatch]
public static class InputButtonImagePatch
{
    [HarmonyPatch(typeof(InputButtonImage), nameof(InputButtonImage.CheckInput))]
    [HarmonyPrefix]
    public static bool CheckInputPrefix(InputButtonImage __instance)
    {
        if (__instance.button == Inpt.Btn.None)
            return true;

        if (Inpt.usingGamepad)
            return true;

        __instance.label.sprite = GameSystem.keyCodeToSpriteDictionary[Inpt.GetKeyCode(Inpt.playerOne.id, Utils.GetActionName(__instance.button))];
        __instance.background.enabled = true;
        __instance.background.sprite = (__instance.label.sprite.rect.width > 12f) ? __instance.largeBG : __instance.smallBG;

        return false;
    }
}