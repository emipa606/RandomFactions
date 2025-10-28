# GitHub Copilot Instructions for Random Factions (Continued)

## Mod Overview and Purpose

**Random Factions (Continued)** is an updated version of Dr. Plantabyte's mod designed for RimWorld players who enjoy a dynamic and ever-changing environment. The mod's main goal is to enhance the gameplay experience by introducing randomness in faction selection, resulting in unique world compositions each time you start a new game. It automatically integrates with other faction-modifying mods, offering a unique combination of factions, and provides translation support for better accessibility.

## Key Features and Systems

- **Random Factions**: Automatically generates a diverse set of factions, such as aggressive, neutral, and hostile, on new world generation.
- **Random Xenotypes (Biotech DLC)**: Replaces baseline human factions with randomly selected xenotype factions if the Biotech DLC is installed.
- **Mod Options**:
  - **Re-organize Factions**: Begins the New Colony screen with random factions.
  - **% Xenotype Frequency**: Controls the likelihood of converting baseline factions into xenotype factions.
  - **Allow Duplicate Factions**: Permits the same faction type to spawn multiple times.
- **Integration with Other Mods**: The mod automatically detects new factions added by other mods and incorporates them into the random selection pool. It pairs especially well with mods like Faction Control, and Vanilla Factions Expanded (VFE).

## Coding Patterns and Conventions

- **Class Definitions**: The code uses classes derived from either `FactionFilter` or `FactionDefFilter` to define criteria or characteristics for filtering factions.
- **Abstraction and Inheritance**: Abstract classes such as `FactionFilter` and `FactionDefFilter` provide a base for other filters, promoting code reuse and organization.
- **Methods**: Meaningful method names like `ReplaceWithRandomNonHiddenFaction` indicate function purpose clearly, following C# naming conventions for readability.

## XML Integration

- The mod's configuration can be adjusted using XML files, which store settings, such as `ModIdentifier` for different mods like Random Factions.
- The core mod functionality integrates with XML data to efficiently manage and apply user settings, leveraging RimWorld's robust XML loading system.

## Harmony Patching

- **Harmony Integration**: Each `ModBase` class is automatically assigned a `HarmonyInstance`, which applies all necessary patches seamlessly. This setup allows the mod to override or enhance RimWorld's existing functionality.
- **Custom Harmony Methods**: Use private methods within `RandomFactionsMod.cs` to influence game behavior. For example, `fixVFENewFactionPopups` resolves pop-up issues related to VFE integration.

## Suggestions for Copilot

1. **Ensure Correct Mod Dependencies**: Being dependent on the HugsLib mod, make sure the order of loading is:
   - HugsLib
   - Random Factions (Continued)

2. **Utilize Class and Method Templates**: When adding new filters or selectors, refer to existing class templates like `AllowedTemperatureFactionDefFilter` and methods like `ReplaceWithRandomNonHiddenFaction`.
  
3. **Overriding ModBase Identifiers**: When extending from `ModBase`, always remember to provide a unique `ModIdentifier` to avoid conflicts in settings management.

4. **Dynamic Faction Handling**: In `RandomFactionGenerator.cs`, use utility methods for faction selection like `drawRandomFactionDef` and `drawRandomXenotypeDef` to maintain consistency in faction assignment logic.

5. **Debugging Harmony Patches**: If encountering issues, start by checking that each class extending `ModBase` properly initializes its `HarmonyInstance`.

By following these instructions and using the provided guidelines, you can effectively contribute to and extend the functionality of the Random Factions mod, ensuring a stable and enjoyable experience for all players.
