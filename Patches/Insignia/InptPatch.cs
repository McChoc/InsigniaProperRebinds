using HarmonyLib;
using Rewired;
using UnityEngine;

namespace InsigniaProperKeybindsMod.Patches.Insignia;

[HarmonyPatch]
public static class InptPatch
{
    public static Inpt.Btn[] AllButtons { get; } =
    [
        (Inpt.Btn)ProperButton.Jump,
        (Inpt.Btn)ProperButton.Roll,
        (Inpt.Btn)ProperButton.Run,
        (Inpt.Btn)ProperButton.Attack,
        (Inpt.Btn)ProperButton.Interact,
        (Inpt.Btn)ProperButton.Rations,
        (Inpt.Btn)ProperButton.Target,
        (Inpt.Btn)ProperButton.Menu,
        (Inpt.Btn)ProperButton.Map,
        (Inpt.Btn)ProperButton.MenuSubmit,
        (Inpt.Btn)ProperButton.MenuBack,
        (Inpt.Btn)ProperButton.MenuActionA,
        (Inpt.Btn)ProperButton.MenuActionB,
        (Inpt.Btn)ProperButton.MenuUp,
        (Inpt.Btn)ProperButton.MenuDown,
        (Inpt.Btn)ProperButton.MenuLeft,
        (Inpt.Btn)ProperButton.MenuRight,
        (Inpt.Btn)ProperButton.MenuNextPage,
        (Inpt.Btn)ProperButton.MenuPrevPage,
        (Inpt.Btn)ProperButton.MenuZoomIn,
        (Inpt.Btn)ProperButton.MenuZoomOut,
    ];

    public static Inpt.Btn[] AllActionButtons { get; } =
    [
        (Inpt.Btn)ProperButton.Jump,
        (Inpt.Btn)ProperButton.Roll,
        (Inpt.Btn)ProperButton.Run,
        (Inpt.Btn)ProperButton.Attack,
        (Inpt.Btn)ProperButton.Interact,
        (Inpt.Btn)ProperButton.Rations,
        (Inpt.Btn)ProperButton.Target,
        (Inpt.Btn)ProperButton.Menu,
        (Inpt.Btn)ProperButton.Map,
        (Inpt.Btn)ProperButton.MenuSubmit,
        (Inpt.Btn)ProperButton.MenuBack,
        (Inpt.Btn)ProperButton.MenuActionA,
        (Inpt.Btn)ProperButton.MenuActionB,
        (Inpt.Btn)ProperButton.MenuNextPage,
        (Inpt.Btn)ProperButton.MenuPrevPage,
        (Inpt.Btn)ProperButton.MenuZoomIn,
        (Inpt.Btn)ProperButton.MenuZoomOut,
    ];

    public static Inpt.Btn[] AllDirections { get; } =
    [
        (Inpt.Btn)ProperButton.MenuUp,
        (Inpt.Btn)ProperButton.MenuDown,
        (Inpt.Btn)ProperButton.MenuLeft,
        (Inpt.Btn)ProperButton.MenuRight,
    ];

    [HarmonyPatch(typeof(Inpt), nameof(Inpt.GetAnyDown))]
    [HarmonyPrefix]
    public static bool GetAnyDownPrefix(bool includeDirections, ref bool __result)
    {
        var array = includeDirections ? AllButtons : AllActionButtons;

        for (int i = 0; i < array.Length; i++)
        {
            if (Inpt.GetDown(array[i]))
            {
                __result = true;
                return false;
            }
        }

        __result = false;
        return false;
    }

    [HarmonyPatch(typeof(Inpt), nameof(Inpt.Get))]
    [HarmonyPrefix]
    public static bool GetPrefix(Inpt.Btn button, ref bool __result)
    {
        if (!(ContextSystem.activeMenu != null) || !ContextSystem.activeMenu.IsLocked())
        {
            __result = GameSystem.playerOne.GetButton(Utils.GetActionName(button));
            return false;
        }

        __result = false;
        return false;
    }

    [HarmonyPatch(typeof(Inpt), nameof(Inpt.GetDown))]
    [HarmonyPrefix]
    public static bool GetDownPrefix(Inpt.Btn button, bool ignoreMenu, ref bool __result)
    {
        bool num = ContextSystem.activeMenu != null;
        bool flag = num && ContextSystem.activeMenu!.IsLocked();

        if (!(num & flag) | ignoreMenu)
        {
            __result = GameSystem.playerOne.GetButtonDown(Utils.GetActionName(button));
            return false;
        }

        __result = false;
        return false;
    }

    [HarmonyPatch(typeof(Inpt), nameof(Inpt.GetUp))]
    [HarmonyPrefix]
    public static bool GetUpPrefix(Inpt.Btn button, ref bool __result)
    {
        if (!(ContextSystem.activeMenu != null) || !ContextSystem.activeMenu.IsLocked())
        {
            __result = GameSystem.playerOne.GetButtonUp(Utils.GetActionName(button));
            return false;
        }

        __result = false;
        return false;
    }

    [HarmonyPatch(typeof(Inpt), nameof(Inpt.SetMapAsActive))]
    [HarmonyPostfix]
    public static void SetMapAsActivePostfix(KeyboardMap keyboardMap)
    {
        if (keyboardMap == null)
            return;

        if (!ReInput.isReady)
            return;

        Utils.BindActionsToDefaultKeys(keyboardMap);

        var player = Inpt.playerOne;

        if (player == null)
            return;

        var joystickMaps = player.controllers.maps.GetAllMaps(ControllerType.Joystick);

        foreach (var joystickMap in joystickMaps)
            Utils.BindActionsToJoystick(joystickMap);
    }

    [HarmonyPatch(typeof(Inpt), nameof(Inpt.GetInputVector))]
    [HarmonyPrefix]
    public static bool GetInputVectorPrefix(ref Vector2 __result)
    {
        var vector = Vector2.zero;

        if (Inpt.Get((Inpt.Btn)ProperButton.MenuUp))
            vector += Vector2.up;

        if (Inpt.Get((Inpt.Btn)ProperButton.MenuDown))
            vector += Vector2.down;

        if (Inpt.Get((Inpt.Btn)ProperButton.MenuLeft))
            vector += Vector2.left;

        if (Inpt.Get((Inpt.Btn)ProperButton.MenuRight))
            vector += Vector2.right;

        __result = vector;
        return false;
    }
}