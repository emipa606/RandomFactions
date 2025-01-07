using RimWorld;

namespace RandomFactions.filters;

public class NaturalEnemyFactionFilter(bool isNaturalEnemy) : FactionFilter
{
    protected override bool Matches(Faction f)
    {
        return f.def.naturalEnemy == isNaturalEnemy;
    }
}