using RimWorld;

namespace RandomFactions.filters;

public class HiddenFactionDefFilter(bool isHidden) : FactionDefFilter
{
    protected override bool Matches(FactionDef f)
    {
        return f.hidden == isHidden;
    }
}