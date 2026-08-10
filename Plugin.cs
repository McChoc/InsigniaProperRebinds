using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Linq;
using UnityEngine.SceneManagement;

namespace InsigniaProperKeybindsMod;

[BepInPlugin(GUID, NAME, VERSION)]
public class Plugin : BaseUnityPlugin
{
    private const string GUID = "com.insignia.properrebinds";
    private const string NAME = "Insignia Proper Rebinds";
    private const string VERSION = "0.9.3";

    private static readonly Harmony _harmony = new(GUID);

    public static ManualLogSource Log { get; } = BepInEx.Logging.Logger.CreateLogSource(NAME);

    public void Awake()
    {
        _harmony.PatchAll();
        SceneManager.sceneLoaded += SceneManager_SceneLoaded;
    }

    private void SceneManager_SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var sprintPrompt = scene
            .GetRootGameObjects()
            .FirstOrDefault(x => x.name == "Sprint Prompts")
            ?.GetComponentInChildren<InputButtonImage>();

        sprintPrompt?.button = (Inpt.Btn)ProperButton.Run;
    }
}