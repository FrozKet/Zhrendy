using System;
using HarmonyLib;
using RimWorld;
using Verse;

public class StorytellerTraitGameComponent : GameComponent
{
    private bool traitsApplied;

    public StorytellerTraitGameComponent(Game game)
    {
    }

    public override void FinalizeInit()
    {
        base.FinalizeInit();
        ApplyToAllMaps();
    }

    public override void LoadedGame()
    {
        base.LoadedGame();
        ApplyToAllMaps();
    }

    private void ApplyToAllMaps()
    {
        if (traitsApplied) return;

        if (Find.Storyteller?.def?.defName != "RamenRendy")
            return;

        TraitDef traitDef = DefDatabase<TraitDef>.GetNamed("Gourmand");

        foreach (Map map in Find.Maps)
        {
            foreach (Pawn pawn in map.mapPawns.AllPawns)
            {
                TryGiveTrait(pawn, traitDef);
            }
        }

        traitsApplied = true;
    }

    private void TryGiveTrait(Pawn pawn, TraitDef traitDef)
    {
        if (pawn == null) return;
        if (!pawn.RaceProps.Humanlike) return;
        if (pawn.story == null) return;

        // Черта
        if (pawn.story.traits != null && !pawn.story.traits.HasTrait(traitDef))
        {
            pawn.story.traits.GainTrait(new Trait(traitDef));
        }

        // Внешность
        pawn.story.bodyType = BodyTypeDefOf.Fat;
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref traitsApplied, "traitsApplied", false);
    }
}

[HarmonyPatch(typeof(Pawn), nameof(Pawn.SpawnSetup))]
public static class Pawn_SpawnSetup_Patch
{
    public static void Postfix(Pawn __instance)
    {
        if (__instance == null) return;
        if (!__instance.RaceProps.Humanlike) return;
        if (__instance.story == null) return;

        if (Find.Storyteller?.def?.defName != "RamenRendy")
            return;

        TraitDef traitDef = DefDatabase<TraitDef>.GetNamed("Gourmand");

        if (__instance.story.traits != null && !__instance.story.traits.HasTrait(traitDef))
        {
            __instance.story.traits.GainTrait(new Trait(traitDef));
        }

        // Внешность
        __instance.story.bodyType = BodyTypeDefOf.Fat;
    }
}

[StaticConstructorOnStartup]
public static class ModStartup
{
    static ModStartup()
    {
        var harmony = new Harmony("FrozKet.foodrendi.RamenRendy");
        harmony.PatchAll();
    }
}
