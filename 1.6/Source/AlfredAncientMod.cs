using HarmonyLib;
using Verse;

namespace AlfredAncient
{
    public class AlfredAncientMod : Mod
    {
        public AlfredAncientMod(ModContentPack pack) : base(pack)
        {
            new Harmony("AlfredAncientMod").PatchAll();
        }
    }
}