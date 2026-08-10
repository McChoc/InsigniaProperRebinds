using Rewired;
using System;
using System.Linq;
using UnityEngine;

namespace InsigniaProperKeybindsMod;

public static class Utils
{
    public static void PrintTree(Transform currentTransform, int depth)
    {
        string indentation = new('-', depth);
        string name = currentTransform.name;
        string components = string.Join(", ", currentTransform.GetComponents<Component>().Select(x => x.GetType().Name));

        Plugin.Log.LogInfo($"{indentation}{name} ({components})");

        foreach (Transform child in currentTransform)
            PrintTree(child, depth + 1);
    }

    public static string GetActionName(Inpt.Btn button)
    {
        return TryGetProperButton(button, out var properButton)
            ? properButton.ToString()
            : button.ToString();
    }

    public static bool TryGetProperButton(Inpt.Btn button, out ProperButton properButton)
    {
        int value = (int)button;

        if (value < 100)
        {
            properButton = default;
            return false;
        }
        else
        {
            properButton = (ProperButton)value;
            return Enum.IsDefined(typeof(ProperButton), properButton);
        }
    }

    public static Inpt.Btn AsOriginalButton(ProperButton properButton)
    {
        return properButton switch
        {
            ProperButton.Run => Inpt.Btn.Roll,
            ProperButton.MenuUp => Inpt.Btn.Up,
            ProperButton.MenuDown => Inpt.Btn.Down,
            ProperButton.MenuLeft => Inpt.Btn.Left,
            ProperButton.MenuRight => Inpt.Btn.Right,
            ProperButton.MenuNextPage => Inpt.Btn.Rations,
            ProperButton.MenuPrevPage => Inpt.Btn.Abilities,
            ProperButton.MenuZoomIn => Inpt.Btn.Rations,
            ProperButton.MenuZoomOut => Inpt.Btn.Abilities,
            _ => (Inpt.Btn)properButton,
        };
    }

    public static void BindActionsToDefaultKeys(KeyboardMap keyboardMap)
    {
        BindActionToDefaultKey("Run", "Roll", keyboardMap);
        BindActionToDefaultKey("MenuUp", "Up", keyboardMap);
        BindActionToDefaultKey("MenuDown", "Down", keyboardMap);
        BindActionToDefaultKey("MenuLeft", "Left", keyboardMap);
        BindActionToDefaultKey("MenuRight", "Right", keyboardMap);
        BindActionToDefaultKey("MenuNextPage", "Rations", keyboardMap);
        BindActionToDefaultKey("MenuPrevPage", "Abilities", keyboardMap);
        BindActionToDefaultKey("MenuZoomIn", "Rations", keyboardMap);
        BindActionToDefaultKey("MenuZoomOut", "Abilities", keyboardMap);
    }

    public static void BindActionsToJoystick(ControllerMap controllerMap)
    {
        BindActionToJoystick("Run", "Roll", controllerMap);
        BindActionToJoystick("MenuUp", "Up", controllerMap);
        BindActionToJoystick("MenuDown", "Down", controllerMap);
        BindActionToJoystick("MenuLeft", "Left", controllerMap);
        BindActionToJoystick("MenuRight", "Right", controllerMap);
        BindActionToJoystick("MenuNextPage", "Rations", controllerMap);
        BindActionToJoystick("MenuPrevPage", "Abilities", controllerMap);
        BindActionToJoystick("MenuZoomIn", "Rations", controllerMap);
        BindActionToJoystick("MenuZoomOut", "Abilities", controllerMap);
    }

    private static void BindActionToDefaultKey(string action, string baseAction, KeyboardMap keyboardMap)
    {
        int actionId = ReInput.mapping.GetActionId(action);

        if (actionId < 0)
        {
            Plugin.Log.LogError($"Could not find Rewired action '{action}'.");
            return;
        }

        if (keyboardMap.ElementMapsWithAction(actionId).Any())
            return;

        int baseActionId = ReInput.mapping.GetActionId(baseAction);

        if (baseActionId < 0)
        {
            Plugin.Log.LogError($"Could not find Rewired action '{baseAction}'.");
            return;
        }

        var elementMaps = keyboardMap.ElementMapsWithAction(baseActionId).ToList();

        foreach (var elementMap in elementMaps)
        {
            bool success = keyboardMap.CreateElementMap(
                actionId,
                Pole.Positive,
                elementMap.keyCode,
                ModifierKeyFlags.None,
                out ActionElementMap newMap);

            if (success)
                Plugin.Log.LogInfo($"Bound {action} to {elementMap.keyCode} on layout {keyboardMap.layoutId} (ActionElementMap ID = {newMap.id})");
            else
                Plugin.Log.LogInfo($"Failed to bind {action} to {elementMap.keyCode} on layout {keyboardMap.layoutId}");
        }
    }

    private static void BindActionToJoystick(string action, string baseAction, ControllerMap controllerMap)
    {
        int actionId = ReInput.mapping.GetActionId(action);

        if (actionId < 0)
        {
            Plugin.Log.LogError($"Could not find Rewired action '{action}'.");
            return;
        }

        if (controllerMap.ElementMapsWithAction(actionId).Any())
            return;

        int baseActionId = ReInput.mapping.GetActionId(baseAction);

        if (baseActionId < 0)
        {
            Plugin.Log.LogError($"Could not find Rewired action '{baseAction}'.");
            return;
        }

        var elementMaps = controllerMap.ElementMapsWithAction(baseActionId).ToList();

        foreach (var elementMap in elementMaps)
        {
            var assignment = new ElementAssignment(
                ControllerType.Joystick,
                elementMap.elementType,
                elementMap.elementIdentifierId,
                elementMap.axisRange,
                elementMap.keyCode,
                elementMap.modifierKeyFlags,
                actionId,
                elementMap.axisContribution,
                elementMap.invert
            );

            bool success = controllerMap.CreateElementMap(assignment);

            if (success)
                Plugin.Log.LogInfo($"Bound {action} to joystick element {elementMap.elementIdentifierId} on layout {controllerMap.layoutId}");
            else
                Plugin.Log.LogWarning($"Failed to bind {action} to joystick element {elementMap.elementIdentifierId}");
        }
    }
}