/*
# Random Factions Rimworld Mod
Author: Dr. Plantabyte (aka Christopher C. Hall)
## CC BY 4.0

This work is licensed on the [Attribution 4.0 International (CC BY 4.0)](https://creativecommons.org/licenses/by/4.0/) Creative Commons License.


### You are free to:

* **Share** � copy and redistribute the material in any medium or format
* **Adapt** � remix, transform, and build upon the material
    for any purpose, even commercially.


### Under the following terms:

* **Attribution** � You must give appropriate credit, provide a link to the license, and indicate if changes were made. You may do so in any reasonable manner, but not in any way that suggests the licensor endorses you or your use.

* **No additional restrictions** � You may not apply legal terms or technological measures that legally restrict others from doing anything the license permits.

### Guarentees:

The licensor cannot revoke these freedoms as long as you follow the license terms.
 */

using System.Collections.Generic;
using RimWorld;

public abstract class FactionFilter
{
    protected abstract bool Matches(Faction f);

    public static List<Faction> FilterFactions(IEnumerable<Faction> allFactions, params FactionFilter[] filters)
    {
        var output = new List<Faction>();
        foreach (var fac in allFactions)
        {
            var matches = true;
            foreach (var filter in filters)
            {
                if (filter.Matches(fac))
                {
                    continue;
                }

                matches = false;
                break;
            }

            if (matches)
            {
                output.Add(fac);
            }
        }

        return output;
    }
}