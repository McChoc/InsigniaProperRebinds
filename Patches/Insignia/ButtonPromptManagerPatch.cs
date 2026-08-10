using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace InsigniaProperKeybindsMod.Patches.Insignia;

[HarmonyPatch]
public static class ButtonPromptManagerPatch
{
    [HarmonyPatch(typeof(ButtonPromptManager), nameof(ButtonPromptManager.GetButtonSpriteDictionary))]
    [HarmonyPrefix]
    public static bool GetButtonSpriteDictionaryPrefix(ref Dictionary<Inpt.Btn, Sprite> __result)
    {
        __result = new Dictionary<Inpt.Btn, Sprite>
        {
            [(Inpt.Btn)ProperButton.Jump] = GameSystem.buttonSpritesStatic[0],
            [(Inpt.Btn)ProperButton.Roll] = GameSystem.buttonSpritesStatic[1],
            [(Inpt.Btn)ProperButton.Run] = GameSystem.buttonSpritesStatic[1],
            [(Inpt.Btn)ProperButton.Attack] = GameSystem.buttonSpritesStatic[2],
            [(Inpt.Btn)ProperButton.Interact] = GameSystem.buttonSpritesStatic[3],
            [(Inpt.Btn)ProperButton.Target] = GameSystem.buttonSpritesStatic[4],
            [(Inpt.Btn)ProperButton.Rations] = GameSystem.buttonSpritesStatic[5],
            [(Inpt.Btn)ProperButton.Up] = GameSystem.buttonSpritesStatic[7],
            [(Inpt.Btn)ProperButton.Down] = GameSystem.buttonSpritesStatic[8],
            [(Inpt.Btn)ProperButton.Left] = GameSystem.buttonSpritesStatic[9],
            [(Inpt.Btn)ProperButton.Right] = GameSystem.buttonSpritesStatic[10],
            [(Inpt.Btn)ProperButton.Menu] = GameSystem.buttonSpritesStatic[6],
            [(Inpt.Btn)ProperButton.Map] = GameSystem.buttonSpritesStatic[14],
            [(Inpt.Btn)ProperButton.MenuSubmit] = GameSystem.buttonSpritesStatic[0],
            [(Inpt.Btn)ProperButton.MenuBack] = GameSystem.buttonSpritesStatic[1],
            [(Inpt.Btn)ProperButton.MenuActionA] = GameSystem.buttonSpritesStatic[2],
            [(Inpt.Btn)ProperButton.MenuActionB] = GameSystem.buttonSpritesStatic[3],
            [(Inpt.Btn)ProperButton.MenuUp] = GameSystem.buttonSpritesStatic[7],
            [(Inpt.Btn)ProperButton.MenuDown] = GameSystem.buttonSpritesStatic[8],
            [(Inpt.Btn)ProperButton.MenuLeft] = GameSystem.buttonSpritesStatic[9],
            [(Inpt.Btn)ProperButton.MenuRight] = GameSystem.buttonSpritesStatic[10],
            [(Inpt.Btn)ProperButton.MenuNextPage] = GameSystem.buttonSpritesStatic[5],
            [(Inpt.Btn)ProperButton.MenuPrevPage] = GameSystem.buttonSpritesStatic[4],
            [(Inpt.Btn)ProperButton.MenuZoomIn] = GameSystem.buttonSpritesStatic[5],
            [(Inpt.Btn)ProperButton.MenuZoomOut] = GameSystem.buttonSpritesStatic[4],
            [(Inpt.Btn)ProperButton.Vertical] = GameSystem.buttonSpritesStatic[11],
            [(Inpt.Btn)ProperButton.Horizontal] = GameSystem.buttonSpritesStatic[12],
            [(Inpt.Btn)ProperButton.Directional] = GameSystem.buttonSpritesStatic[13],
        };

        return false;
    }
}