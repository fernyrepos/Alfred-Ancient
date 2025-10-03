using RimWorld;
using Verse;

namespace AlfredAncient
{
    public static class Utils
    {
        public static T TryGetComp<T>(this Storyteller storyteller) where T : StorytellerComp
        {
            return storyteller.storytellerComps.FirstOrFallback(comp => comp is T) as T;
        }
    }
}