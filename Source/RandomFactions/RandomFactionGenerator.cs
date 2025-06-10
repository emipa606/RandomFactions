/*
# Random Factions Rimworld Mod
Author: Dr. Plantabyte (aka Christopher C. Hall)
## CC BY 4.0

This work is licensed on the [Attribution 4.0 International (CC BY 4.0)](https://creativecommons.org/licenses/by/4.0/) Creative Commons License.


### You are free to:

* **Share** — copy and redistribute the material in any medium or format
* **Adapt** — remix, transform, and build upon the material
    for any purpose, even commercially.


### Under the following terms:

* **Attribution** — You must give appropriate credit, provide a link to the license, and indicate if changes were made. You may do so in any reasonable manner, but not in any way that suggests the licensor endorses you or your use.

* **No additional restrictions** — You may not apply legal terms or technological measures that legally restrict others from doing anything the license permits.

### Guarentees:

The licensor cannot revoke these freedoms as long as you follow the license terms.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using HugsLib.Utils;
using RandomFactions;
using RandomFactions.filters;
using RimWorld;
using Verse;

public class RandomFactionGenerator
{
    private readonly List<FactionDef> definedFactionDefs = [];

    private readonly bool hasBiotech;
    //private RandFacDataStore dataStore;

    private readonly ModLogger modLogger;
    private readonly string[] modOffBooksFactionDefNames;
    private readonly int percentXeno;
    private readonly Random prng;

    public RandomFactionGenerator(int percentXenoFaction, IEnumerable<FactionDef> allFactionDefs,
        string[] offBooksFactionDefNames, bool hasBiotechExpansion, ModLogger logger)
    {
        // init globals
        modLogger = logger;
        percentXeno = percentXenoFaction;
        hasBiotech = hasBiotechExpansion;
        modOffBooksFactionDefNames = offBooksFactionDefNames;
        var seeder = new Random(Find.World.ConstantRandSeed);
        var seedBuffer = new byte[4];
        seeder.NextBytes(seedBuffer);
        var seed = BitConverter.ToInt32(seedBuffer, 0);
        prng = new Random(seed);
        // load existing faction definitions
        foreach (var def in allFactionDefs)
        {
            if (def.categoryTag.EqualsIgnoreCase(RandomFactionsMod.RANDOM_CATEGORY_NAME))
            {
                continue;
            } // skip factions from this mod

            definedFactionDefs.Add(def);
        }

        logger.Trace($"RandomFactionGenerator constructed with random number seed {Find.World.ConstantRandSeed}");
    }

    public void ReplaceWithRandomNonHiddenFaction(Faction faction, bool allowDuplicates)
    {
        var priorFactions = Find.World.factionManager.AllFactions;
        var existingFactions = priorFactions as Faction[] ?? priorFactions.ToArray();
        var newFaction = randomNpcFaction(existingFactions, allowDuplicates);
        if (newFaction == null)
        {
            modLogger.Message($"Failed to generate a new faction to replace {faction}. Retaining the old faction.");
            return;
        }

        replaceFaction(faction, newFaction, existingFactions);
    }

    public void ReplaceWithRandomNonHiddenEnemyFaction(Faction faction, bool allowDuplicates)
    {
        var priorFactions = Find.World.factionManager.AllFactions;
        var existingFactions = priorFactions as Faction[] ?? priorFactions.ToArray();
        var newFaction = randomEnemyFaction(existingFactions, allowDuplicates);
        if (newFaction == null)
        {
            modLogger.Message($"Failed to generate a new faction to replace {faction}. Retaining the old faction.");
            return;
        }

        replaceFaction(faction, newFaction, existingFactions);
    }

    public void ReplaceWithRandomNonHiddenWarlordFaction(Faction faction, bool allowDuplicates)
    {
        var priorFactions = Find.World.factionManager.AllFactions;
        var existingFactions = priorFactions as Faction[] ?? priorFactions.ToArray();
        var newFaction = randomRoughFaction(existingFactions, allowDuplicates);
        if (newFaction == null)
        {
            modLogger.Message($"Failed to generate a new faction to replace {faction}. Retaining the old faction.");
            return;
        }

        replaceFaction(faction, newFaction, existingFactions);
    }

    public void ReplaceWithRandomNonHiddenTraderFaction(Faction faction, bool allowDuplicates)
    {
        var priorFactions = Find.World.factionManager.AllFactions;
        var existingFactions = priorFactions as Faction[] ?? priorFactions.ToArray();
        var newFaction = randomNeutralFaction(existingFactions, allowDuplicates);
        if (newFaction == null)
        {
            modLogger.Message($"Failed to generate a new faction to replace {faction}. Retaining the old faction.");
            return;
        }

        replaceFaction(faction, newFaction, existingFactions);
    }

    public void ReplaceWithRandomNamedFaction(Faction faction, bool allowDuplicates, params string[] validDefNames)
    {
        var priorFactions = Find.World.factionManager.AllFactions;
        var existingFactions = priorFactions as Faction[] ?? priorFactions.ToArray();
        var newFaction = randomNamedFaction(existingFactions, allowDuplicates, validDefNames);
        if (newFaction == null)
        {
            modLogger.Message($"Failed to generate a new faction to replace {faction}. Retaining the old faction.");
            return;
        }

        replaceFaction(faction, newFaction, existingFactions);
    }

    private void replaceFaction(Faction oldFaction, Faction newFaction, IEnumerable<Faction> priorFactions)
    {
        modLogger.Message(
            $"Replacing faction {oldFaction.Name} ({oldFaction.def.defName}) with faction {newFaction.Name} ({newFaction.def.defName})");
        //var ignoreRelation = new FactionRelation(oldFaction, FactionRelationKind.Neutral)
        //{
        //    baseGoodwill = 0
        //};
        //foreach (var faction in priorFactions)
        //{
        //    if (faction.IsPlayer)
        //    {
        //        continue;
        //    }

        //    if (faction.Equals(oldFaction))
        //    {
        //        continue;
        //    }

        //    faction.SetRelation(ignoreRelation);
        //}

        foreach (var stl in Find.WorldObjects.Settlements)
        {
            if (stl.Faction.Equals(oldFaction))
            {
                stl.SetFaction(newFaction);
            }
        }

        oldFaction.defeated = true;
        Find.World.factionManager.Add(newFaction);
    }

    private int countFactionsOfType(FactionDef def, IEnumerable<Faction> factions)
    {
        var count = 0;
        foreach (var f in factions)
        {
            if (f.def.defName == def.defName)
            {
                count++;
            }
        }

        return count;
    }

    private FactionDef drawRandomFactionDef(List<FactionDef> factionDefs, IEnumerable<Faction> existingFactions)
    {
        FactionDef randomFactionDef = null;
        var limit = 100;
        var enumerable = existingFactions as Faction[] ?? existingFactions.ToArray();
        while (--limit > 0)
        {
            randomFactionDef = factionDefs[prng.Next(factionDefs.Count)];
            var count = countFactionsOfType(randomFactionDef, enumerable);
            if (randomFactionDef.maxCountAtGameStart <= 0 || count < randomFactionDef.maxCountAtGameStart)
            {
                break;
            }
        }

        if (!hasBiotech || !RandomFactionsMod.IsXenotypePatchable(randomFactionDef) || !Rand.Chance(percentXeno / 100f))
        {
            Log.Message($"Will not add xeno, returning {randomFactionDef}");
            return randomFactionDef;
        }

        var randomXenotypeDef = drawRandomXenotypeDef(DefDatabase<XenotypeDef>.AllDefsListForReading);
        var xenoFactionDefName = RandomFactionsMod.XenoFactionDefName(randomXenotypeDef, randomFactionDef);
        var factionDef = findFactionDefByName(xenoFactionDefName);
        return factionDef ?? randomFactionDef;
    }

    private XenotypeDef drawRandomXenotypeDef(List<XenotypeDef> xenotypeDefs)
    {
        return xenotypeDefs[prng.Next(xenotypeDefs.Count)];
    }

    private static FactionDef findFactionDefByName(string name)
    {
        foreach (var def in DefDatabase<FactionDef>.AllDefs)
        {
            if (name.Equals(def.defName))
            {
                return def;
            }
        }

        return null;
    }

    private Faction randomNpcFaction(IEnumerable<Faction> existingFactions, bool allowDuplicates)
    {
        while (true)
        {
            var enumerable = existingFactions as Faction[] ?? existingFactions.ToArray();
            var filterFactionDefs = FactionDefFilter.FilterFactionDefs(definedFactionDefs,
                new PlayerFactionDefFilter(false), new HiddenFactionDefFilter(false),
                new FactionDefNameFilter(false, modOffBooksFactionDefNames),
                duplicateFilter(enumerable, allowDuplicates));
            // if there's already one of everything, allow duplicates again
            if (filterFactionDefs.Count <= 0)
            {
                existingFactions = enumerable;
                allowDuplicates = true;
                continue;
            }

            var randomFactionDef = drawRandomFactionDef(filterFactionDefs, enumerable);
            return generateFactionFromDef(randomFactionDef, enumerable);
        }
    }

    private Faction randomEnemyFaction(IEnumerable<Faction> existingFactions, bool allowDuplicates)
    {
        while (true)
        {
            var enumerable = existingFactions as Faction[] ?? existingFactions.ToArray();
            var filterFactionDefs = FactionDefFilter.FilterFactionDefs(definedFactionDefs,
                new PlayerFactionDefFilter(false), new HiddenFactionDefFilter(false),
                new FactionDefNameFilter(false, modOffBooksFactionDefNames), new PermanentEnemyFactionDefFilter(true),
                duplicateFilter(enumerable, allowDuplicates));
            // if there's already one of everything, allow duplicates again
            if (filterFactionDefs.Count <= 0)
            {
                existingFactions = enumerable;
                allowDuplicates = true;
                continue;
            }

            var randomFactionDef = drawRandomFactionDef(filterFactionDefs, enumerable);
            return generateFactionFromDef(randomFactionDef, enumerable);
        }
    }

    private Faction randomRoughFaction(IEnumerable<Faction> existingFactions, bool allowDuplicates)
    {
        while (true)
        {
            var enumerable = existingFactions as Faction[] ?? existingFactions.ToArray();
            var filterFactionDefs = FactionDefFilter.FilterFactionDefs(definedFactionDefs,
                new PlayerFactionDefFilter(false), new HiddenFactionDefFilter(false),
                new FactionDefNameFilter(false, modOffBooksFactionDefNames), new PermanentEnemyFactionDefFilter(false),
                new NaturalEnemyFactionDefFilter(true), duplicateFilter(enumerable, allowDuplicates));
            // if there's already one of everything, allow duplicates again
            if (filterFactionDefs.Count <= 0)
            {
                existingFactions = enumerable;
                allowDuplicates = true;
                continue;
            }

            var randomFactionDef = drawRandomFactionDef(filterFactionDefs, enumerable);
            return generateFactionFromDef(randomFactionDef, enumerable);
        }
    }

    private Faction randomNeutralFaction(IEnumerable<Faction> existingFactions, bool allowDuplicates)
    {
        while (true)
        {
            var enumerable = existingFactions as Faction[] ?? existingFactions.ToArray();
            var filterFactionDefs = FactionDefFilter.FilterFactionDefs(definedFactionDefs,
                new PlayerFactionDefFilter(false), new HiddenFactionDefFilter(false),
                new FactionDefNameFilter(false, modOffBooksFactionDefNames), new PermanentEnemyFactionDefFilter(false),
                new NaturalEnemyFactionDefFilter(false), duplicateFilter(enumerable, allowDuplicates));
            // if there's already one of everything, allow duplicates again
            if (filterFactionDefs.Count <= 0)
            {
                existingFactions = enumerable;
                allowDuplicates = true;
                continue;
            }

            var randomFactionDef = drawRandomFactionDef(filterFactionDefs, enumerable);
            return generateFactionFromDef(randomFactionDef, enumerable);
        }
    }

    private Faction randomNamedFaction(IEnumerable<Faction> existingFactions, bool allowDuplicates,
        params string[] nameList)
    {
        while (true)
        {
            var enumerable = existingFactions as Faction[] ?? existingFactions.ToArray();
            var filterFactionDefs = FactionDefFilter.FilterFactionDefs(definedFactionDefs,
                new PlayerFactionDefFilter(false), new FactionDefNameFilter(false, modOffBooksFactionDefNames),
                new FactionDefNameFilter(nameList), duplicateFilter(enumerable, allowDuplicates));
            if (filterFactionDefs.Count <= 0)
            {
                existingFactions = enumerable;
                allowDuplicates = true;
                continue;
            }

            var randomFactionDef = drawRandomFactionDef(filterFactionDefs, enumerable);
            return generateFactionFromDef(randomFactionDef, enumerable);
        }
    }

    private Faction generateFactionFromDef(FactionDef def, IEnumerable<Faction> existingFactions)
    {
        var relations = defaultRelations(def, existingFactions);
        try
        {
            var fac = FactionGenerator.NewGeneratedFactionWithRelations(def, relations, def.hidden);
            return fac;
        }
        catch
        {
            return null;
        }
    }

    private static FactionDefFilter duplicateFilter(IEnumerable<Faction> existingFactions, bool allowDuplicates)
    {
        FactionDefFilter dupFilter;
        if (allowDuplicates)
        {
            dupFilter = new FactionDefNoOpFilter();
        }
        else
        {
            var defNames = new List<string>();
            foreach (var f in existingFactions)
            {
                defNames.Add(f.def.defName);
            }

            dupFilter = new FactionDefNameFilter(false, defNames.ToArray());
        }

        return dupFilter;
    }

    private static int defaultGoodwill(FactionDef def)
    {
        if (def.categoryTag.EqualsIgnoreCase(RandomFactionsMod.RANDOM_CATEGORY_NAME))
        {
            return 0;
        }

        if (def.permanentEnemy)
        {
            return -100;
        }

        if (def.naturalEnemy)
        {
            return -80;
        }

        return 0;
    }

    private static int defaultGoodwill(Faction fac)
    {
        if (fac.def.categoryTag.EqualsIgnoreCase(RandomFactionsMod.RANDOM_CATEGORY_NAME))
        {
            return 0;
        }

        var goodwill = defaultGoodwill(fac.def);
        return goodwill == 0 ? fac.NaturalGoodwill : goodwill;
    }

    private static List<FactionRelation> defaultRelations(FactionDef target, IEnumerable<Faction> allFactions)
    {
        var relList = new List<FactionRelation>();
        foreach (var fac in allFactions)
        {
            if (fac.IsPlayer)
            {
                continue;
            }

            var initGw = lowestOf(defaultGoodwill(target), defaultGoodwill(fac));
            relList.Add(new FactionRelation(fac, (FactionRelationKind)initGw));
        }

        return relList;
    }

    private static int lowestOf(params int[] n)
    {
        var lowest = n[0];

        return n.Prepend(lowest).Min();
    }
}