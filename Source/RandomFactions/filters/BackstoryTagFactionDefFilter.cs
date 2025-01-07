using System.Linq;
using RimWorld;

namespace RandomFactions.filters;

public class BackstoryTagFactionDefFilter(BackstoryCategoryFilter tag) : FactionDefFilter
{
    // eg "Offworld" or "Pirate"

    protected override bool Matches(FactionDef f)
    {
        return f.backstoryFilters.Any(backstoryCategoryFilter =>
            backstoryCategoryFilter.categories.Any(cat => cat == tag.ToString()));
    }
}