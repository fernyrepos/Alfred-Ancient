using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace AlfredAncient
{
    [HarmonyPatch(typeof(InteractionWorker_RecruitAttempt), nameof(InteractionWorker_RecruitAttempt.Interacted))]
    public static class InteractionWorker_RecruitAttempt_Interacted_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var instructionsList = instructions.ToList();

            var fGuest = AccessTools.Field(
                typeof(Pawn),
                nameof(Pawn.guest)
            );
            var fResistance = AccessTools.Field(
                typeof(Pawn_GuestTracker),
                nameof(Pawn_GuestTracker.resistance)
            );

            var mMin = AccessTools.Method(
                typeof(Mathf),
                nameof(Mathf.Min),
                new[] {typeof(float), typeof(float)}
            );

            var pStoryTeller = AccessTools.PropertyGetter(
                typeof(Find),
                nameof(Find.Storyteller)
            );

            var mTryGetStorytellerComp = AccessTools.Method(
                typeof(Utils),
                nameof(Utils.TryGetComp),
                new[] {typeof(Storyteller)}, new[] {typeof(StorytellerComp_IncreaseRecruitDifficulty)}
            );

            for (var i = 0; i < instructionsList.Count; i++)
            {
                var curInstr = instructionsList[i];
                
                if (
                    curInstr.IsLdloc() &&
                    instructionsList[i + 1].IsLdarg() &&
                    instructionsList[i + 2].LoadsField(fGuest) &&
                    instructionsList[i + 3].LoadsField(fResistance) &&
                    instructionsList[i + 4].Calls(mMin)
                )
                {
                    var doesNotHaveIncreasedRecruitDifficulty = generator.DefineLabel();
                    curInstr.labels.Add(doesNotHaveIncreasedRecruitDifficulty);

                    yield return new CodeInstruction(
                        OpCodes.Call,
                        pStoryTeller
                    );
                    yield return new CodeInstruction(
                        OpCodes.Call,
                        mTryGetStorytellerComp
                    );
                    yield return new CodeInstruction(
                        OpCodes.Brfalse_S,
                        doesNotHaveIncreasedRecruitDifficulty
                    );
                    yield return new CodeInstruction(
                        OpCodes.Ldloc,
                        curInstr.operand
                    );
                    yield return new CodeInstruction(
                        OpCodes.Ldc_R4,
                        5f
                    );
                    yield return new CodeInstruction(OpCodes.Div);
                    yield return new CodeInstruction(
                        OpCodes.Stloc,
                        curInstr.operand
                    );
                }

                yield return curInstr;
            }
        }
    }

    [HarmonyPatch(typeof(SkillRecord), nameof(SkillRecord.Interval))]
    public static class SkillRecord_Interval_Patch
    {
        public static bool Prefix()
        {
            if (Find.Storyteller?.def == DefsOf.AlfredAncient_AlfredAncient)
            {
                return false;
            }
            return true;
        }
    }
    
    [DefOf]
    public static class DefsOf
    {
        public static StorytellerDef AlfredAncient_AlfredAncient;
    }
}
