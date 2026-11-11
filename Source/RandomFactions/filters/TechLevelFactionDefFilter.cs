using RimWorld;

namespace RandomFactions.filters;

public class TechLevelFactionDefFilter(TechLevel minTl, TechLevel maxTl) : FactionDefFilter
{
    /* Tech levels in order:
    Undefined,
    Animal,
    Neolithic,
    Medieval,
    Industrial,
    Spacer,
    Ultra,
    Archotech
    */

    protected override bool Matches(FactionDef f)
    {
        var t = toInt(f.techLevel);
        var low = toInt(minTl);
        var high = toInt(maxTl);
        return t >= low && t <= high;
    }

    private static int toInt(TechLevel tl)
    {
        return (int)tl;
    }
}