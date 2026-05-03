using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RepairViaContextMenu;

public static class FloatMenuPatch
{
    private const int RequiredCrafting = 4;

    public static IEnumerable<MethodBase> FindTargetMethods()
    {
        return AccessTools.GetDeclaredMethods(typeof(FloatMenuMakerMap))
            .Where(m => m.Name == "ChoicesAtFor")
            .Where(m => typeof(List<FloatMenuOption>).IsAssignableFrom(m.ReturnType));
    }

    public static void Postfix(object[] __args, ref List<FloatMenuOption> __result)
    {
        if (__args == null || __args.Length < 2)
            return;

        if (__args[0] is not Vector3 clickPos || __args[1] is not Pawn pawn)
            return;

        if (pawn.Map == null || pawn.Dead || pawn.Downed)
            return;

        __result ??= new List<FloatMenuOption>();

        var c = IntVec3.FromVector3(clickPos);
        var things = c.GetThingList(pawn.Map);

        foreach (var thing in things)
        {
            if (IsRepairableItem(thing))
                __result.Add(BuildItemOption(pawn, thing));

            if (thing is Pawn targetPawn)
            {
                foreach (var gear in GetDamagedGear(targetPawn))
                    __result.Add(BuildItemOption(pawn, gear, targetPawn));
            }
        }
    }

    private static FloatMenuOption BuildItemOption(Pawn pawn, Thing item, Pawn owner = null)
    {
        var label = owner == null
            ? "RVCM_RepairAction".Translate(item.LabelCap)
            : "RVCM_RepairActionPawn".Translate(item.LabelCap + " (" + owner.LabelShortCap + ")");

        if (pawn.skills?.GetSkill(SkillDefOf.Crafting)?.Level < RequiredCrafting)
            return new FloatMenuOption(label + " (" + "RVCM_NoSkill".Translate(RequiredCrafting) + ")", null);

        if (!pawn.CanReserve(item))
            return new FloatMenuOption(label + " (" + "RVCM_Reserved".Translate() + ")", null);

        if (!pawn.CanReach(item, PathEndMode.Touch, Danger.Some))
            return new FloatMenuOption(label + " (" + "RVCM_CannotReach".Translate() + ")", null);

        return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, () =>
        {
            var job = JobMaker.MakeJob(RepairJobDefOf.RVCM_RepairItem, item);
            pawn.jobs.TryTakeOrderedJob(job);
        }), pawn, item);
    }

    private static IEnumerable<Thing> GetDamagedGear(Pawn targetPawn)
    {
        if (targetPawn.apparel != null)
        {
            foreach (var apparel in targetPawn.apparel.WornApparel.Where(IsRepairableItem))
                yield return apparel;
        }

        if (IsRepairableItem(targetPawn.equipment?.Primary))
            yield return targetPawn.equipment.Primary;
    }

    private static bool IsRepairableItem(Thing thing)
    {
        return thing != null
               && thing.def.useHitPoints
               && thing.HitPoints < thing.MaxHitPoints
               && (thing.def.IsWeapon || thing.def.IsApparel);
    }
}
