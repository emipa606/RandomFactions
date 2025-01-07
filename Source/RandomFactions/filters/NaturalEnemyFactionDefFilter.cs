using RimWorld;

namespace RandomFactions.filters;

public class NaturalEnemyFactionDefFilter(bool isNaturalEnemy) : FactionDefFilter
{
    protected override bool Matches(FactionDef f)
    {
        return f.naturalEnemy == isNaturalEnemy;
    }
}