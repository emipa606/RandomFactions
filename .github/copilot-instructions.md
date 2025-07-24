# GitHub Copilot Instructions for RimWorld Modding Project

## Mod Overview and Purpose

This mod for RimWorld enhances the game's faction system by introducing a series of filters and generating random factions based on customizable criteria. The mod aims to provide deeper gameplay mechanics through more dynamic and diverse faction interactions, allowing players to experience a unique and unpredictable world.

## Key Features and Systems

- **Faction Filters**: Implement various filters that modify faction definitions. These include filters based on temperature, tech level, backstory tags, etc.
- **Random Faction Generator**: Provides functions to replace existing factions with randomly selected ones, adhering to specified criteria.
- **XML Integration**: Uses XML configuration files to manage faction settings and filter attributes.
- **Harmony Patching**: Incorporates Harmony for runtime patching to ensure compatibility and seamless integration with the base game and other mods.

## Coding Patterns and Conventions

1. **Class and Method Naming**: 
    - Classes follow the pattern `<Attribute>FactionDefFilter` or `<Attribute>FactionFilter`, indicating their purpose.
    - Methods are generally verb-based, providing a clear understanding of their action or outcome (e.g., `ReplaceWithRandomNonHiddenFaction`).

2. **Constructor Initialization**:
    - Each filter class includes a constructor for setting up its specific properties (e.g., temperature, backstory tags).

3. **Abstract Base Classes**:
    - `FactionDefFilter` and `FactionFilter` serve as abstract base classes, ensuring a common interface for filter operations and encourage code reusability.

4. **Method Overloading**:
    - Variants of methods cater to different scenarios (e.g., allowing or disallowing duplicate factions).

5. **Code Structure**:
    - Organized into specific files with a single responsibility principle; each file usually contains one class, enhancing readability and maintainability.

## XML Integration

While there was an issue parsing XML files from the summary, XML integration is critical for configuration management. Ensure all XML files:
- Follow RimWorld's XML schema for mod settings and definitions.
- Properly define new faction def elements and attributes (use the `<Defs>`, `<FactionDefs>` tags).

Use XML files to:
- Outline configuration settings for different filters.
- Store persistent mod settings, linked through unique ModBase identifiers.

## Harmony Patching

- **Usage**: Utilize Harmony to patch RimWorld's methods, allowing for custom logic insertion or modification without directly altering the base game files.
- **Initialization**: Ensure all Harmony patches are properly initialized in the `RandomFactionsMod` class.
- **Best Practices**: Apply patches that are non-intrusive and ensure they are compatible with potential updates and other mods.

## Suggestions for Copilot

When using GitHub Copilot to work on this mod, consider the following suggestions:

1. **Code Suggestions**:
   - Focus on extending existing filter classes for new faction criteria.
   - Utilize Copilot to automate method scaffolding for similar pattern-based classes or methods.

2. **Refactoring**:
   - Seek advice from Copilot for restructuring code, especially around complex method logic in `RandomFactionGenerator`.

3. **XML Handling**:
   - Leverage Copilot to auto-generate valid XML snippets for configuration, ensuring they align with expected RimWorld mod standards.

4. **Harmony Method Detection**:
   - When identifying methods for Harmony patching, use Copilot to assist in locating accurate method signatures or to create postfix/prefix patches.

With these guidelines, you can enhance the functionality and maintainability of your RimWorld mod, ensuring efficient development with GitHub Copilot.
