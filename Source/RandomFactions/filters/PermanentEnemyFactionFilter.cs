using RimWorld;

namespace RandomFactions.filters;

public class PermanentEnemyFactionFilter(bool isPermanentEnemy) : FactionFilter
{
    protected override bool Matches(Faction f)
    {
        return f.def.permanentEnemy == isPermanentEnemy;
    }
}