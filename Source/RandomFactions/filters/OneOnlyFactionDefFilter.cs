using RimWorld;

namespace RandomFactions.filters;

public class OneOnlyFactionDefFilter(bool isOneOnly) : FactionDefFilter
{
    protected override bool Matches(FactionDef f)
    {
        return f.maxConfigurableAtWorldCreation == 1 == isOneOnly;
    }
}