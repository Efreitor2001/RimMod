using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RepairViaContextMenu;

public class JobDriver_RepairItem : JobDriver
{
    private const int TicksToRepair = 240;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDestroyedOrNull(TargetIndex.A);

        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);

        var repair = Toils_General.Wait(TicksToRepair);
        repair.WithProgressBarToilDelay(TargetIndex.A);
        repair.defaultCompleteMode = ToilCompleteMode.Delay;
        repair.initAction = () => pawn.skills?.Learn(SkillDefOf.Crafting, 150f);
        yield return repair;

        yield return Toils_General.Do(() =>
        {
            if (job.targetA.Thing is Thing thing && thing.def.useHitPoints)
            {
                thing.HitPoints = thing.MaxHitPoints;
            }
        });
    }
}
