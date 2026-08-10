using HarmonyLib;

namespace InsigniaProperKeybindsMod.Patches.Insignia;

[HarmonyPatch]
public static class ChronicleViewPatch
{
    [HarmonyPatch(typeof(ChronicleView), "Update")]
    [HarmonyPrefix]
    private static bool UpdatePrefix(ChronicleView __instance)
    {
        if (Inpt.GetDown((Inpt.Btn)ProperButton.MenuRight))
            Traverse.Create(__instance).Method("SetForward").GetValue();
        else if (Inpt.GetDown((Inpt.Btn)ProperButton.MenuLeft))
            Traverse.Create(__instance).Method("SetBack").GetValue();

        return false;
    }

    [HarmonyPatch(typeof(ChronicleView), "AssignPrompts")]
    [HarmonyPrefix]
    private static bool AssignPromptsPrefix(ChronicleView __instance, int ___pageIndex)
    {
        MenuPromptRow.Clear();
        MenuPromptRow.Set(Inpt.Btn.MenuB, "Back to Quests");

        if (___pageIndex > 0)
            MenuPromptRow.Set((Inpt.Btn)ProperButton.MenuPrevPage, "Previous page");

        if (___pageIndex + 1 < __instance.GetMaxPage())
            MenuPromptRow.Set((Inpt.Btn)ProperButton.MenuNextPage, "Next page");

        return false;
    }
}