using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace InsigniaProperKeybindsMod.Patches.Insignia;

[HarmonyPatch]
public static class PlayerGroundControlPatch
{
    [HarmonyPatch(typeof(PlayerGroundControl), nameof(PlayerGroundControl.Do))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> DoTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);

        var rollHeldField = AccessTools.Field(
            typeof(InputIntent),
            nameof(InputIntent.rollHeld)
        );

        var getInputMethod = AccessTools.Method(
            typeof(BehaviourState),
            "get_input"
        );

        var getRunMethod = AccessTools.Method(
            typeof(PlayerGroundControlPatch),
            nameof(GetRun)
        );

        bool patched = false;

        for (int i = 0; i < codes.Count - 2; i++)
        {
            // Look for:
            //
            // ldarg.0
            // call BehaviourState::get_input()
            // ldfld InputIntent::rollHeld
            //
            // and replace all three with:
            //
            // call InptPatch::GetRun()

            if (codes[i].opcode == OpCodes.Ldarg_0 &&
                codes[i + 1].opcode == OpCodes.Call &&
                Equals(codes[i + 1].operand, getInputMethod) &&
                codes[i + 2].opcode == OpCodes.Ldfld &&
                Equals(codes[i + 2].operand, rollHeldField))
            {
                codes[i] = new CodeInstruction(
                    OpCodes.Call,
                    getRunMethod
                );

                codes.RemoveAt(i + 2);
                codes.RemoveAt(i + 1);

                patched = true;
                Plugin.Log.LogInfo("Patched PlayerGroundControl.Do()");
                break;
            }
        }

        if (!patched)
            Plugin.Log.LogError("Could not find the expected rollHeld access pattern in PlayerGroundControl.Do().");

        return codes;
    }

    [HarmonyPatch(typeof(PlayerGroundControl), nameof(PlayerGroundControl.FixedDo))]
    [HarmonyTranspiler]
    public static IEnumerable<CodeInstruction> FixedDoTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = new List<CodeInstruction>(instructions);

        var getMethod = AccessTools.Method(
            typeof(Inpt),
            nameof(Inpt.Get),
            [typeof(Inpt.Btn)]
        );

        var getRunMethod = AccessTools.Method(
            typeof(PlayerGroundControlPatch),
            nameof(GetRun)
        );

        bool patched = false;

        for (int i = 0; i < codes.Count; i++)
        {
            if (codes[i].opcode != System.Reflection.Emit.OpCodes.Call ||
                !Equals(codes[i].operand, getMethod))
            {
                continue;
            }

            // We are specifically looking for:
            //
            //     ldc.i4.2
            //     call Inpt.Get(Inpt.Btn)
            //
            // where 2 == Inpt.Btn.Roll.

            if (i > 0 && GetLoadedInt(codes[i - 1], out int value))
            {
                if (value == (int)Inpt.Btn.Roll)
                {
                    // Replace the integer + Inpt.Get call with:
                    //
                    //     call InptPatch.GetRun()

                    codes[i - 1] = new CodeInstruction(
                        OpCodes.Call,
                        getRunMethod
                    );

                    codes.RemoveAt(i);

                    patched = true;
                    Plugin.Log.LogInfo("Patched PlayerGroundControl.FixedDo()");
                    break;
                }
            }
        }

        if (!patched)
            Plugin.Log.LogError("Could not find Inpt.Get(Inpt.Btn.Roll) in PlayerGroundControl.FixedDo().");

        return codes;
    }

    private static bool GetRun()
    {
        return Inpt.Get((Inpt.Btn)ProperButton.Run);
    }

    private static bool GetLoadedInt(CodeInstruction instruction, out int value)
    {
        value = 0;

        if (instruction.opcode == OpCodes.Ldc_I4_M1)
        {
            value = -1;
            return true;
        }

        if (instruction.opcode == OpCodes.Ldc_I4_0)
        {
            value = 0;
            return true;
        }

        if (instruction.opcode == OpCodes.Ldc_I4_1)
        {
            value = 1;
            return true;
        }

        if (instruction.opcode == OpCodes.Ldc_I4_2)
        {
            value = 2;
            return true;
        }

        if (instruction.opcode == OpCodes.Ldc_I4_3)
        {
            value = 3;
            return true;
        }

        if (instruction.opcode == OpCodes.Ldc_I4_4)
        {
            value = 4;
            return true;
        }

        if (instruction.opcode == OpCodes.Ldc_I4_5)
        {
            value = 5;
            return true;
        }

        if (instruction.opcode == OpCodes.Ldc_I4_6)
        {
            value = 6;
            return true;
        }

        if (instruction.opcode == OpCodes.Ldc_I4_7)
        {
            value = 7;
            return true;
        }

        if (instruction.opcode == OpCodes.Ldc_I4_8)
        {
            value = 8;
            return true;
        }

        if (instruction.opcode == OpCodes.Ldc_I4)
        {
            value = (int)instruction.operand;
            return true;
        }

        if (instruction.opcode == OpCodes.Ldc_I4_S)
        {
            value = (sbyte)instruction.operand;
            return true;
        }

        return false;
    }
}