using RimWorld;

namespace RandomFactions.filters;

public class OneOnlyFactionDefFilter(bool isOneOnly) : FactionDefFilter
{
    protected override bool Matches(FactionDef def)
    {
        //return def.maxCountAtGameStart == 1 == isOneOnly;
        return def.maxConfigurableAtWorldCreation == 1 == isOneOnly;
    }
}