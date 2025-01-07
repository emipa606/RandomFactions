using RimWorld;

namespace RandomFactions.filters;

public class PlayerFactionDefFilter(bool isPlayer) : FactionDefFilter
{
    protected override bool Matches(FactionDef f)
    {
        return f.isPlayer == isPlayer;
    }
}