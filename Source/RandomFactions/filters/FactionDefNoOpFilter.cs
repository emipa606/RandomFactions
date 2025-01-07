using RimWorld;

namespace RandomFactions.filters;

public class FactionDefNoOpFilter : FactionDefFilter
{
    protected override bool Matches(FactionDef f)
    {
        // do nothing
        return true;
    }
}