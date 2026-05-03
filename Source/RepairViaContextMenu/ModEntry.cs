using HarmonyLib;
using Verse;

namespace RepairViaContextMenu;

[StaticConstructorOnStartup]
public static class ModEntry
{
    static ModEntry()
    {
        new Harmony("rimmod.repaircontextmenu").PatchAll();
    }
}
