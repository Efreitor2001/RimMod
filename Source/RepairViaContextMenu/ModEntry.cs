using HarmonyLib;
using Verse;

namespace RepairViaContextMenu;

[StaticConstructorOnStartup]
public static class ModEntry
{
    static ModEntry()
    {
        var harmony = new Harmony("rimmod.repaircontextmenu");
        var postfix = new HarmonyMethod(typeof(FloatMenuPatch), nameof(FloatMenuPatch.Postfix));
        var patched = 0;

        foreach (var method in FloatMenuPatch.FindTargetMethods())
        {
            harmony.Patch(method, postfix: postfix);
            patched++;
        }

        if (patched == 0)
            Log.Warning("[RepairViaContextMenu] Не найдены методы FloatMenuMakerMap.ChoicesAtFor для патча.");
    }
}
