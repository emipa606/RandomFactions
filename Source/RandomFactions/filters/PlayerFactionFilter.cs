using RimWorld;

namespace RandomFactions.filters;

public class PlayerFactionFilter(bool isPlayer) : FactionFilter
{
    protected override bool Matches(Faction f)
    {
        return f.IsPlayer == isPlayer;
    }
}