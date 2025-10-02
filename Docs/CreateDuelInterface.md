# Create-a-Duel Interface

This document describes the interface for creating custom duels programmatically using the `CreateDuel` method.

## Overview

The `CreateDuel` method provides a single, simple interface for creating custom duels with two decks. This is the recommended entry point for quick create-a-duel scripts and integrations.

## Method Signature

```csharp
public DuelSettings CreateDuel(DeckInfo playerDeck, DeckInfo opponentDeck, Dictionary<string, object> settings)
```

### Parameters

- **playerDeck** (DeckInfo): The deck for player 1 (human player). Cannot be null.
- **opponentDeck** (DeckInfo): The deck for player 2 (opponent/CPU). Cannot be null.
- **settings** (Dictionary<string, object>): Optional additional duel settings. Can be null for defaults.

### Returns

- **DuelSettings**: A fully configured DuelSettings object ready for use.

## Basic Usage Example

```csharp
// Load player deck
DeckInfo playerDeck = new DeckInfo();
playerDeck.File = Path.Combine(dataDirectory, "MyDeck.ydk");
playerDeck.Load();

// Load opponent deck
DeckInfo opponentDeck = new DeckInfo();
opponentDeck.File = Path.Combine(dataDirectory, "OpponentDeck.ydk");
opponentDeck.Load();

// Create the duel with default settings
DuelSettings duel = gameServer.CreateDuel(playerDeck, opponentDeck, null);
```

### Quick Start with Existing Game Decks

Want to try it immediately? See [RunDuelWithExistingDecks.cs](Examples/RunDuelWithExistingDecks.cs) for examples using structure decks that come with the game - no need to create your own deck files!

## Advanced Usage with Custom Settings

```csharp
// Load decks
DeckInfo playerDeck = new DeckInfo();
playerDeck.File = Path.Combine(dataDirectory, "MyDeck.json");
playerDeck.Load();

DeckInfo opponentDeck = new DeckInfo();
opponentDeck.File = Path.Combine(dataDirectory, "OpponentDeck.json");
opponentDeck.Load();

// Create custom settings
Dictionary<string, object> customSettings = new Dictionary<string, object>()
{
    { "FirstPlayer", 0 }, // Player 1 goes first (0 = player, 1 = opponent, -1 = random)
    { "life", new int[] { 8000, 8000 } }, // Life points for each player
    { "hnum", new int[] { 5, 5 } }, // Starting hand size for each player
    { "noshuffle", false }, // Allow deck shuffling
};

// Create the duel with custom settings
DuelSettings duel = gameServer.CreateDuel(playerDeck, opponentDeck, customSettings);
```

## Loading Decks

### From YDK Files

```csharp
DeckInfo deck = new DeckInfo();
deck.File = Path.Combine(dataDirectory, "Decks", "MyDeck.ydk");
deck.Load();
```

### From JSON Files

```csharp
DeckInfo deck = new DeckInfo();
deck.File = Path.Combine(dataDirectory, "Decks", "MyDeck.json");
deck.Load();
```

### From Player's Saved Decks

```csharp
// Assuming you have access to a Player object
foreach (DeckInfo playerDeck in player.Decks.Values)
{
    if (playerDeck.Name == "My Favorite Deck")
    {
        // Use this deck
        break;
    }
}
```

## Default Behavior

When `CreateDuel` is called with null settings, it automatically:

1. Sets both decks to the provided DeckInfo objects
2. Marks the duel as a custom duel (IsCustomDuel = true)
3. Sets required defaults via SetRequiredDefaults()
4. Assigns default player names ("Duelist" and "CPU")
5. Generates a random seed for the duel
6. Randomly selects which player goes first

## Integration with CustomDuel.json

The existing `CustomDuel.json` mechanism in `GetSoloDuelSettings` already provides a way to override tutorial duels. The `CreateDuel` method is complementary and provides a more direct programmatic interface.

You can combine both approaches:

1. Use `CustomDuel.json` for file-based configuration
2. Use `CreateDuel` for programmatic/scripted duel creation

## Common Settings Dictionary Keys

Here are common keys you can use in the settings dictionary:

- **FirstPlayer** (int): Who goes first (0 = player 1, 1 = player 2, -1 = random)
- **life** (int[]): Starting life points for each player (default: [8000, 8000])
- **hnum** (int[]): Starting hand size for each player (default: [5, 5])
- **noshuffle** (bool): Disable deck shuffling (default: false)
- **name** (string[]): Player names (default: ["Duelist", "CPU"])
- **RandSeed** (uint): Random seed for the duel
- **cpu** (int): CPU difficulty/behavior
- **cpuflag** (string): CPU behavior flags (e.g., "Def", "Fool", "Light")

For a complete list of available settings, see `DuelSettings.cs`.

## Example: Creating a Practice Script

```csharp
// Create a simple practice duel script
public void CreatePracticeDuel(GameServer gameServer, string playerDeckPath, string opponentDeckPath)
{
    try
    {
        // Load player deck
        DeckInfo playerDeck = new DeckInfo();
        playerDeck.File = playerDeckPath;
        playerDeck.Load();
        
        // Load opponent deck
        DeckInfo opponentDeck = new DeckInfo();
        opponentDeck.File = opponentDeckPath;
        opponentDeck.Load();
        
        // Create custom settings for practice
        Dictionary<string, object> settings = new Dictionary<string, object>()
        {
            { "FirstPlayer", 0 }, // Player always goes first for practice
            { "life", new int[] { 8000, 8000 } },
            { "cpu", 100 }, // Standard CPU difficulty
        };
        
        // Create the duel
        DuelSettings duel = gameServer.CreateDuel(playerDeck, opponentDeck, settings);
        
        // Now you can use this duel settings object as needed
        // For example, save it to a file or use it in your custom duel logic
        
        Console.WriteLine("Practice duel created successfully!");
    }
    catch (Exception e)
    {
        Console.WriteLine("Error creating practice duel: " + e.Message);
    }
}
```

## Location in Source Code

The `CreateDuel` method is located in:
- **File**: `YgoMasterServer/Acts/Act_Duel.cs`
- **Class**: `GameServer` (partial class)

## Related Classes

- **DeckInfo**: Represents a deck with main/extra/side deck cards (`YgoMasterServer/Infos/DeckInfo.cs`)
- **DuelSettings**: Contains all settings for a duel (`YgoMasterServer/DuelSettings.cs`)
- **GameServer**: Main server class that handles game logic (`YgoMasterServer/GameServer.cs`)

## Notes

- The method throws `ArgumentNullException` if either deck is null
- The method is thread-safe as it only reads from GameServer's random number generator
- All deck files must exist and be valid before calling Load()
- The returned DuelSettings object can be further modified before use
