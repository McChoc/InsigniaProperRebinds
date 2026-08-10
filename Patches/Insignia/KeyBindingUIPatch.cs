using HarmonyLib;
using Rewired;
using System.Collections.Generic;
using UnityEngine;

namespace InsigniaProperKeybindsMod.Patches.Insignia;

[HarmonyPatch]
public static class KeyBindingUIPatch
{
    [HarmonyPatch(typeof(KeyBindingUI), nameof(KeyBindingUI.GetKeyFromBinding))]
    [HarmonyPrefix]
    public static bool GetKeyFromBindingPrefix(KeyBindingUI __instance)
    {
        if (!Utils.TryGetProperButton(__instance.btn, out ProperButton customButton))
            return true;

        int actionId = ReInput.mapping.GetActionId(customButton.ToString());

        if (actionId < 0)
            return false;

        var map = Inpt.config.keyboardMap;

        if (map == null)
            return false;

        var keys = new List<string>();

        foreach (var elementMap in map.ElementMapsWithAction(actionId))
        {
            if (elementMap.elementType == ControllerElementType.Button &&
                elementMap.keyCode != KeyCode.None)
            {
                keys.Add(elementMap.keyCode.ToString());
            }
        }

        string display = string.Join(", ", keys);
        __instance.SetKey(display);
        return false;
    }
}