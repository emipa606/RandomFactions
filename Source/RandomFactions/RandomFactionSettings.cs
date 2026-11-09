using Verse;

namespace RandomFactions;

public class RandomFactionsSettings : ModSettings
{
    public bool allowDuplicates;
    public int randomFactionCount = 4;
    public int randomPirateFactionCount = 1;
    public int randomRoughFactionCount = 2;
    public int randomTradeFactionCount = 2;
    public bool removeOtherFactions = true;
    public bool verboseLogging;
    public int xenoPercent = 15;

    public override void ExposeData()
    {
        Scribe_Values.Look(ref removeOtherFactions, "removeOtherFactions", true);
        Scribe_Values.Look(ref allowDuplicates, "allowDuplicates");
        Scribe_Values.Look(ref xenoPercent, "xenoPercent", 15);
        Scribe_Values.Look(ref verboseLogging, "verboseLogging");
        Scribe_Values.Look(ref randomFactionCount, "randomFactionCount", 4);
        Scribe_Values.Look(ref randomTradeFactionCount, "randomTradeFactionCount", 2);
        Scribe_Values.Look(ref randomRoughFactionCount, "randomRoughFactionCount", 2);
        Scribe_Values.Look(ref randomPirateFactionCount, "randomPirateFactionCount", 1);
    }
}