using RimWorld;
using Verse;

namespace RandomFactions.filters;

public class CategoryTagFactionFilter(string tag) : FactionFilter
{
    // eg "Outlander"

    protected override bool Matches(Faction f)
    {
        return f.def.categoryTag.EqualsIgnoreCase(tag);
    }
}