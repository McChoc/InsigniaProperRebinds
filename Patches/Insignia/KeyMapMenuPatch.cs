using HarmonyLib;
using Rewired;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace InsigniaProperKeybindsMod.Patches.Insignia;

[HarmonyPatch]
public static class KeyMapMenuPatch
{
    [HarmonyPatch(typeof(KeyMapMenu), "Start")]
    [HarmonyPostfix]
    public static void StartPostfix(KeyMapMenu __instance, ref UIFocus ___firstFocus)
    {
        var footer = __instance.transform.Find("canvas").Find("Form Footer");
        var actionGroup = __instance.transform.Find("canvas").Find("Left Side").Find("Action Group");
        var utilityGroup = __instance.transform.Find("canvas").Find("Left Side").Find("Utility Group");
        var movementGroup = __instance.transform.Find("canvas").Find("Right Side").Find("Movement Group");
        var menuGroup = __instance.transform.Find("canvas").Find("Right Side").Find("Menu Group");

        var actionGroupRectTransform = actionGroup.GetComponent<RectTransform>();
        var menuGroupRectTransform = menuGroup.GetComponent<RectTransform>();

        actionGroupRectTransform.anchoredPosition = actionGroupRectTransform.anchoredPosition with { y = actionGroupRectTransform.anchoredPosition.y + 17 };
        menuGroupRectTransform.anchoredPosition = menuGroupRectTransform.anchoredPosition with { y = menuGroupRectTransform.anchoredPosition.y + 161 };

        Object.Destroy(utilityGroup.gameObject);
        Object.Destroy(movementGroup.gameObject);

        foreach (var keyBinding in actionGroup.GetComponentsInChildren<KeyBindingUI>())
            Object.Destroy(keyBinding.gameObject);

        foreach (var keyBinding in menuGroup.GetComponentsInChildren<KeyBindingUI>())
            Object.Destroy(keyBinding.gameObject);

        var keyBindingUIs = __instance.GetComponentsInChildren<KeyBindingUI>();

        var TopBindingTemplate = keyBindingUIs.Single(x => x.title == "Up");
        var MiddleBindingTemplate = keyBindingUIs.Single(x => x.title == "Down");
        var BottomBindingTemplate = keyBindingUIs.Single(x => x.title == "Menu");
        Traverse.Create(BottomBindingTemplate).Field("locked").SetValue(false);

        var jumpBinding             = CreateKeyBindingUIFromBase(TopBindingTemplate,    actionGroup, (Inpt.Btn)ProperButton.Jump,     "Jump");
        var attackBinding           = CreateKeyBindingUIFromBase(MiddleBindingTemplate, actionGroup, (Inpt.Btn)ProperButton.Attack,   "Attack");
        var rollBinding             = CreateKeyBindingUIFromBase(MiddleBindingTemplate, actionGroup, (Inpt.Btn)ProperButton.Roll,     "Roll");
        var runBinding              = CreateKeyBindingUIFromBase(MiddleBindingTemplate, actionGroup, (Inpt.Btn)ProperButton.Run,      "Run");
        var interactBinding         = CreateKeyBindingUIFromBase(MiddleBindingTemplate, actionGroup, (Inpt.Btn)ProperButton.Interact, "Interact");
        var targetBinding           = CreateKeyBindingUIFromBase(MiddleBindingTemplate, actionGroup, (Inpt.Btn)ProperButton.Target,   "Target");
        var rationsBinding          = CreateKeyBindingUIFromBase(MiddleBindingTemplate, actionGroup, (Inpt.Btn)ProperButton.Rations,  "Rations");
        var upBinding               = CreateKeyBindingUIFromBase(MiddleBindingTemplate, actionGroup, (Inpt.Btn)ProperButton.Up,       "Up");
        var downBinding             = CreateKeyBindingUIFromBase(MiddleBindingTemplate, actionGroup, (Inpt.Btn)ProperButton.Down,     "Down");
        var leftBinding             = CreateKeyBindingUIFromBase(MiddleBindingTemplate, actionGroup, (Inpt.Btn)ProperButton.Left,     "Left");
        var rightBinding            = CreateKeyBindingUIFromBase(BottomBindingTemplate, actionGroup, (Inpt.Btn)ProperButton.Right,    "Right");

        var menuBinding             = CreateKeyBindingUIFromBase(TopBindingTemplate,    menuGroup, (Inpt.Btn)ProperButton.Menu,           "Menu");
        var mapBinding              = CreateKeyBindingUIFromBase(MiddleBindingTemplate, menuGroup, (Inpt.Btn)ProperButton.Map,            "Map");
        var menuSubmitBinding       = CreateKeyBindingUIFromBase(MiddleBindingTemplate, menuGroup, (Inpt.Btn)ProperButton.MenuSubmit,     "M-Submit");
        var menuBackBinding         = CreateKeyBindingUIFromBase(MiddleBindingTemplate, menuGroup, (Inpt.Btn)ProperButton.MenuBack,       "M-Back");
        var menuActionABinding      = CreateKeyBindingUIFromBase(MiddleBindingTemplate, menuGroup, (Inpt.Btn)ProperButton.MenuActionA,    "M-Action A");
        var menuActionBBinding      = CreateKeyBindingUIFromBase(MiddleBindingTemplate, menuGroup, (Inpt.Btn)ProperButton.MenuActionB,    "M-Action B");
        var menuUpBinding           = CreateKeyBindingUIFromBase(MiddleBindingTemplate, menuGroup, (Inpt.Btn)ProperButton.MenuUp,         "M-Up");
        var menuDownBinding         = CreateKeyBindingUIFromBase(MiddleBindingTemplate, menuGroup, (Inpt.Btn)ProperButton.MenuDown,       "M-Down");
        var menuLeftBinding         = CreateKeyBindingUIFromBase(MiddleBindingTemplate, menuGroup, (Inpt.Btn)ProperButton.MenuLeft,       "M-Left");
        var menuRightBinding        = CreateKeyBindingUIFromBase(MiddleBindingTemplate, menuGroup, (Inpt.Btn)ProperButton.MenuRight,      "M-Right");
        var menuNextTabBinding      = CreateKeyBindingUIFromBase(MiddleBindingTemplate, menuGroup, (Inpt.Btn)ProperButton.MenuNextPage,   "M-Next Page");
        var menuPreviousTabBinding  = CreateKeyBindingUIFromBase(MiddleBindingTemplate, menuGroup, (Inpt.Btn)ProperButton.MenuPrevPage,   "M-Prev Page");
        var menuZoomInBinding       = CreateKeyBindingUIFromBase(MiddleBindingTemplate, menuGroup, (Inpt.Btn)ProperButton.MenuZoomIn,     "M-Zoom In");
        var menuZoomOutBinding      = CreateKeyBindingUIFromBase(BottomBindingTemplate, menuGroup, (Inpt.Btn)ProperButton.MenuZoomOut,    "M-Zoom Out");

        __instance.focus = jumpBinding.GetComponent<UIFocus>();
        __instance.keyBindings = __instance.GetComponentsInChildren<KeyBindingUI>().ToList();

        foreach (var item in __instance.GetComponentsInChildren<KeyBindingUI>())
            item.GetComponent<UIFocus>().enabled = true;

        ReplaceArgument(footer.GetComponent<UIFocusGroup>().u, menuZoomOutBinding.GetComponent<UIFocus>());
    }

    [HarmonyPatch(typeof(KeyMapMenu), "DoRebind")]
    [HarmonyPostfix]
    public static void DoRebindPostfix(KeyMapMenu __instance)
    {
        if (__instance.state is KeyMapMenu.MenuState.None)
            return;

        var keyboard = ReInput.controllers.Keyboard;

        if (keyboard.GetKeyDown(KeyCode.Escape))
        {
            __instance.FinishRebind(KeyCode.Escape);
            Traverse.Create(__instance).Method("Set", [KeyMapMenu.MenuState.None]).GetValue();
        }
        else if (keyboard.GetKeyDown(KeyCode.Return))
        {
            __instance.FinishRebind(KeyCode.Return);
            Traverse.Create(__instance).Method("Set", [KeyMapMenu.MenuState.None]).GetValue();
        }
        else if (keyboard.GetKeyDown(KeyCode.KeypadEnter))
        {
            __instance.FinishRebind(KeyCode.KeypadEnter);
            Traverse.Create(__instance).Method("Set", [KeyMapMenu.MenuState.None]).GetValue();
        }
    }

    [HarmonyPatch(typeof(KeyMapMenu), nameof(KeyMapMenu.FinishRebind))]
    [HarmonyPrefix]
    public static bool FinishRebindPrefix(KeyMapMenu __instance, ref KeyBindingUI? ___keyToBind, KeyCode code)
    {
        if (___keyToBind == null || __instance.keyboardMap == null || !ReInput.isReady)
            return false;

        string actionName = Utils.GetActionName(___keyToBind.btn);

        Debug.Log($"Rebinding {actionName} to {code}");

        int actionId = ReInput.mapping.GetActionId(actionName);
        if (actionId < 0)
            return false;

        Traverse.Create(__instance).Method("DeleteExistingBindings", [actionId]).GetValue();

        var elementAssignment = Traverse.Create(__instance).Method("GetElementAssignment", [code, actionId]).GetValue<ElementAssignment>();
        __instance.keyboardMap.CreateElementMap(elementAssignment);
        __instance.keyboardMap.isModified = true;

        foreach (var keyBinding in __instance.GetComponentsInChildren<KeyBindingUI>())
            keyBinding.GetKeyFromBinding();

        ___keyToBind.keyLabel.color = Helpers.GetAlphaColour(___keyToBind.keyLabel.color, 1f);
        ___keyToBind = null;
        return false;
    }

    [HarmonyPatch(typeof(KeyMapMenu), nameof(KeyMapMenu.ResetToDefault))]
    [HarmonyPostfix]
    public static void ResetToDefaultPostfix(KeyMapMenu __instance)
    {
        Utils.BindActionsToDefaultKeys(__instance.keyboardMap);

        foreach (KeyBindingUI keyBinding in __instance.keyBindings)
            keyBinding.GetKeyFromBinding();
    }

    private static KeyBindingUI CreateKeyBindingUIFromBase(KeyBindingUI original, Transform parent, Inpt.Btn button, string label)
    {
        var clone = Object.Instantiate(original.gameObject, parent);

        var uiFocus = clone.GetComponent<UIFocus>();
        uiFocus.group = parent.GetComponentInParent<UIFocusGroup>();

        var keyBinding = clone.GetComponent<KeyBindingUI>();
        keyBinding.btn = button;
        keyBinding.actionLabel.text = keyBinding.title = label;
        keyBinding.GetKeyFromBinding();

        return keyBinding;
    }

    private static void ReplaceArgument(UnityEventBase evt, object argument)
    {
        var persistentCallsField = typeof(UnityEventBase)
            .GetField("m_PersistentCalls", BindingFlags.Instance | BindingFlags.NonPublic);

        if (persistentCallsField == null)
        {
            Plugin.Log.LogError("Could not find UnityEventBase.m_PersistentCalls");
            return;
        }

        object persistentCalls = persistentCallsField.GetValue(evt);

        var callsField = persistentCalls.GetType()
            .GetField("m_Calls", BindingFlags.Instance | BindingFlags.NonPublic);

        var calls = (IList)callsField.GetValue(persistentCalls);

        for (int i = 0; i < calls.Count; i++)
        {
            object call = calls[i];

            var argumentsField = call.GetType()
                .GetField("m_Arguments", BindingFlags.Instance | BindingFlags.NonPublic);

            object? arguments = argumentsField?.GetValue(call);

            if (arguments == null)
                continue;

            var objectArgumentField = arguments.GetType()
                .GetField("m_ObjectArgument", BindingFlags.Instance | BindingFlags.NonPublic);

            objectArgumentField?.SetValue(arguments, argument);
        }
    }
}