using RimWorld;

namespace RandomFactions.filters;

public class DefeatedFactionFilter(bool isDefeated) : FactionFilter
{
    protected override bool Matches(Faction f)
    {
        return f.defeated == isDefeated;
    }
}