using HarmonyLib;
using Rewired;
using Rewired.Data;
using System;
using System.Collections.Generic;

namespace InsigniaProperKeybindsMod.Patches.Rewired;

[HarmonyPatch]
public static class UserDataPatch
{
    [HarmonyPatch(typeof(UserData), "MGjLxofcUBaEsKcUjvQAeGHDwfur")]
    [HarmonyPrefix]
    public static void InitializePrefix(UserData __instance, List<InputAction> ___actions)
    {
        if (___actions == null)
        {
            Plugin.Log.LogError("Could not access Rewired UserData.actions.");
            return;
        }

        CreateNewActionFromBase(__instance, ___actions, "Roll", "Run");
        CreateNewActionFromBase(__instance, ___actions, "Up", "MenuUp");
        CreateNewActionFromBase(__instance, ___actions, "Down", "MenuDown");
        CreateNewActionFromBase(__instance, ___actions, "Left", "MenuLeft");
        CreateNewActionFromBase(__instance, ___actions, "Right", "MenuRight");
        CreateNewActionFromBase(__instance, ___actions, "Rations", "MenuNextPage");
        CreateNewActionFromBase(__instance, ___actions, "Abilities", "MenuPrevPage");
        CreateNewActionFromBase(__instance, ___actions, "Rations", "MenuZoomIn");
        CreateNewActionFromBase(__instance, ___actions, "Abilities", "MenuZoomOut");
    }

    private static InputAction? FindAction(IEnumerable<InputAction> actions, string name)
    {
        foreach (var action in actions)
        {
            if (action != null && string.Equals(action.name, name, StringComparison.OrdinalIgnoreCase))
                return action;
        }

        return null;
    }

    private static void CreateNewActionFromBase(UserData userData, IList<InputAction> actions, string originalName, string newName)
    {
        if (FindAction(actions, newName) is not null)
        {
            Plugin.Log.LogInfo($"Rewired action '{newName}' already exists.");
            return;
        }

        var originalAction = FindAction(actions, originalName);

        if (originalAction is null)
        {
            Plugin.Log.LogError($"Could not find Rewired action '{originalName}'.");
            return;
        }

        int newId = userData.GetNewActionId();
        var newAction = originalAction.Clone();
        Traverse.Create(newAction).Property("id").SetValue(newId);
        Traverse.Create(newAction).Property("name").SetValue(newName);

        actions.Add(newAction);

        Plugin.Log.LogInfo($"Added Rewired action '{newName}' with ID {newId}.");
    }
}