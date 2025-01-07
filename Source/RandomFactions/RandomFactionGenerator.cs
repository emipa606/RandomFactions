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
using RimWorld.Planet;
using Verse;

public class RandomFactionGenerator
{
    private readonly List<FactionDef> definedFactionDefs = [];

    private readonly bool hasBiotech;
    //private RandFacDataStore dataStore;

    private readonly ModLogger Logger;
    private readonly string[] offBooksFactionDefNames;
    private readonly int percentXeno;
    private readonly Random prng;
    private readonly World world;

    public RandomFactionGenerator(World world, int percentXenoFaction, IEnumerable<FactionDef> allFactionDefs,
        string[] offBooksFactionDefNames, bool hasBiotechExpansion, ModLogger logger)
    {
        // init globals
        Logger = logger;
        this.world = world;
        percentXeno = percentXenoFaction;
        //this.dataStore = dataStore;
        hasBiotech = hasBiotechExpansion;
        this.offBooksFactionDefNames = offBooksFactionDefNames;
        var seeder = new Random(world.ConstantRandSeed);
        var seed_buffer = new byte[4];
        seeder.NextBytes(seed_buffer);
        var seed = BitConverter.ToInt32(seed_buffer, 0);
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

        logger.Trace($"RandomFactionGenerator constructed with random number seed {world.ConstantRandSeed}");
    }

    public void replaceWithRandomNonHiddenFaction(Faction faction, bool allowDuplicates)
    {
        var priorFactions = world.factionManager.AllFactions;
        var newFaction = randomNPCFaction(priorFactions, allowDuplicates);
        replaceFaction(faction, newFaction, priorFactions);
    }

    public void replaceWithRandomNonHiddenEnemyFaction(Faction faction, bool allowDuplicates)
    {
        var priorFactions = world.factionManager.AllFactions;
        var newFaction = randomEnemyFaction(priorFactions, allowDuplicates);
        replaceFaction(faction, newFaction, priorFactions);
    }

    public void replaceWithRandomNonHiddenWarlordFaction(Faction faction, bool allowDuplicates)
    {
        var priorFactions = world.factionManager.AllFactions;
        var newFaction = randomRoughFaction(priorFactions, allowDuplicates);
        replaceFaction(faction, newFaction, priorFactions);
    }

    public void replaceWithRandomNonHiddenTraderFaction(Faction faction, bool allowDuplicates)
    {
        var priorFactions = world.factionManager.AllFactions;
        var newFaction = randomNeutralFaction(priorFactions, allowDuplicates);
        replaceFaction(faction, newFaction, priorFactions);
    }

    public void replaceWithRandomNamedFaction(Faction faction, bool allowDuplicates, params string[] validDefNames)
    {
        var priorFactions = world.factionManager.AllFactions;
        var newFaction = randomNamedFaction(priorFactions, allowDuplicates, validDefNames);
        replaceFaction(faction, newFaction, priorFactions);
    }

    private void replaceFaction(Faction oldFaction, Faction newFaction, IEnumerable<Faction> priorFactions)
    {
        Logger.Message(string.Format("Replacing faction {0} ({1}) with faction {2} ({3})", oldFaction.Name,
            oldFaction.def.defName, newFaction.Name, newFaction.def.defName));
        var ignoreRelation = new FactionRelation(oldFaction, FactionRelationKind.Neutral)
        {
            baseGoodwill = 0
        };
        foreach (var faction in priorFactions)
        {
            if (faction.IsPlayer)
            {
                continue;
            }

            if (faction.Equals(oldFaction))
            {
                continue;
            }

            faction.SetRelation(ignoreRelation);
        }

        foreach (var stl in Find.WorldObjects.Settlements)
        {
            if (stl.Faction.Equals(oldFaction))
            {
                stl.SetFaction(newFaction);
            }
        }

        oldFaction.defeated = true;
        world.factionManager.Add(newFaction);
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

    private FactionDef drawRandomFactionDef(List<FactionDef> fdefList, IEnumerable<Faction> existingFactions)
    {
        FactionDef fdef = null;
        var retyLimit = 100;
        while (--retyLimit > 0)
        {
            fdef = fdefList[prng.Next(fdefList.Count)];
            var count = countFactionsOfType(fdef, existingFactions);
            if (fdef.maxCountAtGameStart <= 0 || count < fdef.maxCountAtGameStart)
            {
                break;
            }
        }

        if (!hasBiotech || !RandomFactionsMod.isXenotypePatchable(fdef) || prng.Next(100) >= percentXeno)
        {
            return fdef;
        }

        var xenoDef = drawRandomXenotypeDef(DefDatabase<XenotypeDef>.AllDefsListForReading);
        var xfName = RandomFactionsMod.xenoFactionDefName(xenoDef, fdef);
        var xenoFacDef = findFactionDefByName(xfName);
        return xenoFacDef ?? fdef;
    }

    private XenotypeDef drawRandomXenotypeDef(List<XenotypeDef> xdefList)
    {
        return xdefList[prng.Next(xdefList.Count)];
    }

    private FactionDef findFactionDefByName(string name)
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

    public Faction randomNPCFaction(IEnumerable<Faction> existingFactions, bool allowDuplicates)
    {
        var fdefList = FactionDefFilter.filterFactionDefs(definedFactionDefs,
            new PlayerFactionDefFilter(false), new HiddenFactionDefFilter(false),
            new FactionDefNameFilter(false, offBooksFactionDefNames),
            duplicateFilter(existingFactions, allowDuplicates)
        );
        // if there's already one of everything, allow duplicates again
        if (fdefList.Count <= 0)
        {
            return randomNPCFaction(existingFactions, true);
        }

        var fdef = drawRandomFactionDef(fdefList, existingFactions);
        return generateFactionFromDef(fdef, existingFactions);
    }

    public Faction randomEnemyFaction(IEnumerable<Faction> existingFactions, bool allowDuplicates)
    {
        var fdefList = FactionDefFilter.filterFactionDefs(definedFactionDefs,
            new PlayerFactionDefFilter(false), new HiddenFactionDefFilter(false),
            new FactionDefNameFilter(false, offBooksFactionDefNames),
            new PermanentEnemyFactionDefFilter(true),
            duplicateFilter(existingFactions, allowDuplicates)
        );
        // if there's already one of everything, allow duplicates again
        if (fdefList.Count <= 0)
        {
            return randomEnemyFaction(existingFactions, true);
        }

        var fdef = drawRandomFactionDef(fdefList, existingFactions);
        return generateFactionFromDef(fdef, existingFactions);
    }

    public Faction randomRoughFaction(IEnumerable<Faction> existingFactions, bool allowDuplicates)
    {
        var fdefList = FactionDefFilter.filterFactionDefs(definedFactionDefs,
            new PlayerFactionDefFilter(false), new HiddenFactionDefFilter(false),
            new FactionDefNameFilter(false, offBooksFactionDefNames),
            new PermanentEnemyFactionDefFilter(false), new NaturalEnemyFactionDefFilter(true),
            duplicateFilter(existingFactions, allowDuplicates)
        );
        // if there's already one of everything, allow duplicates again
        if (fdefList.Count <= 0)
        {
            return randomRoughFaction(existingFactions, true);
        }

        var fdef = drawRandomFactionDef(fdefList, existingFactions);
        return generateFactionFromDef(fdef, existingFactions);
    }

    public Faction randomNeutralFaction(IEnumerable<Faction> existingFactions, bool allowDuplicates)
    {
        var fdefList = FactionDefFilter.filterFactionDefs(definedFactionDefs,
            new PlayerFactionDefFilter(false), new HiddenFactionDefFilter(false),
            new FactionDefNameFilter(false, offBooksFactionDefNames),
            new PermanentEnemyFactionDefFilter(false), new NaturalEnemyFactionDefFilter(false),
            duplicateFilter(existingFactions, allowDuplicates)
        );
        // if there's already one of everything, allow duplicates again
        if (fdefList.Count <= 0)
        {
            return randomNeutralFaction(existingFactions, true);
        }

        var fdef = drawRandomFactionDef(fdefList, existingFactions);
        return generateFactionFromDef(fdef, existingFactions);
    }

    public Faction randomNamedFaction(IEnumerable<Faction> existingFactions, bool allowDuplicates,
        params string[] nameList)
    {
        var fdefList = FactionDefFilter.filterFactionDefs(definedFactionDefs,
            new PlayerFactionDefFilter(false), new FactionDefNameFilter(false, offBooksFactionDefNames),
            new FactionDefNameFilter(nameList),
            duplicateFilter(existingFactions, allowDuplicates)
        );
        if (fdefList.Count <= 0)
        {
            return randomNamedFaction(existingFactions, true, nameList);
        }

        var fdef = drawRandomFactionDef(fdefList, existingFactions);
        return generateFactionFromDef(fdef, existingFactions);
    }

    public Faction generateFactionFromDef(FactionDef def, IEnumerable<Faction> existingFactions)
    {
        var relations = defaultRelations(def, existingFactions);
        var fac = FactionGenerator.NewGeneratedFactionWithRelations(def, relations, def.hidden);
        return fac;
    }

    private FactionDefFilter duplicateFilter(IEnumerable<Faction> existingFactions, bool allowDuplicates)
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

    private int defaultGoodwill(FactionDef def)
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

    private int defaultGoodwill(Faction fac)
    {
        if (fac.def.categoryTag.EqualsIgnoreCase(RandomFactionsMod.RANDOM_CATEGORY_NAME))
        {
            return 0;
        }

        var defGW = defaultGoodwill(fac.def);
        return defGW == 0 ? fac.NaturalGoodwill : defGW;
    }

    private List<FactionRelation> defaultRelations(FactionDef target, IEnumerable<Faction> allFactions)
    {
        var relList = new List<FactionRelation>();
        foreach (var fac in allFactions)
        {
            if (fac.IsPlayer)
            {
                continue;
            }

            var initGW = lowestOf(defaultGoodwill(target), defaultGoodwill(fac));
            relList.Add(new FactionRelation(fac, (FactionRelationKind)initGW));
        }

        return relList;
    }

    private static int lowestOf(params int[] n)
    {
        var lowest = n[0];

        return n.Prepend(lowest).Min();
    }
}