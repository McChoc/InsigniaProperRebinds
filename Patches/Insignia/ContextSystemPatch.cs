using HarmonyLib;
using UnityEngine;

namespace InsigniaProperKeybindsMod.Patches.Insignia;

[HarmonyPatch]
public static class ContextSystemPatch
{
    [HarmonyPatch(typeof(ContextSystem), nameof(ContextSystem.MenuUpdate))]
    [HarmonyPrefix]
    public static bool MenuUpdatePrefix(ref float ___autoFireInterval, ref float ___timeAtLastAutoFire)
    {
        bool flag = false;
        if (ContextSystem.activeMenu != null && !ContextSystem.activeMenu.IsLocked())
        {
            if (ContextSystem.activeMenu is KeyMapMenu keyMapMenu && keyMapMenu.IsBinding())
                return false;

            if (!Inpt.Get(ContextSystem.lastInputButton))
                ContextSystem.timeAtInputDown = Time.unscaledTime;

            if (Inpt.Get(ContextSystem.lastInputButton) && Time.unscaledTime - ContextSystem.timeAtInputDown > 0.5f && Time.unscaledTime - ___timeAtLastAutoFire >= ___autoFireInterval)
            {
                ___timeAtLastAutoFire = Time.unscaledTime;
                foreach (var btn in InptPatch.AllDirections)
                {
                    if (ContextSystem.lastInputButton == btn)
                        ContextSystem.activeMenu.focus.PressButton(Utils.AsOriginalButton((ProperButton)btn));
                }
            }

            if (Inpt.GetDown(Inpt.Btn.Start) && Engine.NotDead())
            {
                if ((bool)Engine.engine)
                {
                    if (ContextSystem.activeMenu.focus.m.GetPersistentEventCount() > 0 || (ContextSystem.activeMenu.focus.group != null && ContextSystem.activeMenu.focus.group.m.GetPersistentEventCount() > 0))
                    {
                        ContextSystem.lastInputButton = Inpt.Btn.Start;
                        ContextSystem.activeMenu.focus.PressButton(Inpt.Btn.Start);
                    }
                    else
                    {
                        flag = true;
                        if (ContextSystem.activeMenu.isClosable)
                            ContextSystem.activeMenu.CloseMenu();
                    }
                }
            }
            else
            {
                foreach (var btn in InptPatch.AllButtons)
                {
                    if (btn != Inpt.Btn.Start && Inpt.GetDown(btn))
                    {
                        ContextSystem.lastInputButton = btn;
                        ContextSystem.activeMenu.focus.PressButton(Utils.AsOriginalButton((ProperButton)btn));
                    }
                }
            }
        }

        if ((bool)Engine.engine && !flag)
            Engine.engine.UpdateMenuControls();

        return false;
    }
}