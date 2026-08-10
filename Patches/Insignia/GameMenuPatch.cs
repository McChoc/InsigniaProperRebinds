using HarmonyLib;
using System.Linq;

namespace InsigniaProperKeybindsMod.Patches.Insignia;

[HarmonyPatch]
public static class GameMenuPatch
{
    [HarmonyPatch(typeof(GameMenu), "Start")]
    [HarmonyPrefix]
    public static void StartPrefix(GameMenu __instance)
    {
        var prompts = __instance.GetComponentsInChildren<UIPrompt>();

        prompts[0].button = (Inpt.Btn)ProperButton.MenuPrevPage;
        prompts[1].button = (Inpt.Btn)ProperButton.MenuNextPage;
    }

    [HarmonyPatch(typeof(GameMenu), "CheckPageTurn")]
    [HarmonyPrefix]
    public static bool CheckPageTurnPrefix(GameMenu __instance)
    {
        if (__instance.pausePage.menuPages.Any(x => x.gameObject.activeInHierarchy && !x.menu.IsLocked()) && ContextSystem.activeLayer == __instance.menuController)
        {
            __instance.pausePage.menuPages.IndexOf(__instance.pausePage.menuPages.First((GameMenuPage x) => x.gameObject.activeInHierarchy));

            if (Inpt.GetDown((Inpt.Btn)ProperButton.MenuPrevPage))
                __instance.pausePage.PrevPage();

            if (Inpt.GetDown((Inpt.Btn)ProperButton.MenuNextPage))
                __instance.pausePage.NextPage();
        }

        return false;
    }
}