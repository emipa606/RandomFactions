using RimWorld;

namespace RandomFactions.filters;

public class HiddenFactionFilter(bool isHidden) : FactionFilter
{
    protected override bool Matches(Faction f)
    {
        return f.Hidden == isHidden;
    }
}