# CreateDuel Interface Examples

This directory contains example scripts demonstrating how to use the `CreateDuel` interface for programmatically creating custom duels.

## Files

### CreateDuelExample.cs
Comprehensive examples showing various use cases of the `CreateDuel` method:
- Simple duel creation
- Custom settings
- Tournament-style setup
- Practice mode with CPU behavior
- Random deck selection
- Integration with CustomDuel.json
- Error handling
- Deck validation

### QuickDuelScript.cs
A simple, copy-paste ready script for quickly creating custom duels. This is the best starting point for beginners.

## Quick Start

1. Copy `QuickDuelScript.cs` to your YgoMaster integration
2. Modify the configuration section:
   - Set your deck file names
   - Adjust duel settings (life points, first player, etc.)
3. Call `QuickDuelScript.CreateMyDuel(gameServer, dataDirectory)`

## Example Usage

```csharp
// Basic usage
DuelSettings duel = QuickDuelScript.CreateMyDuel(gameServer, dataDirectory);

// Or save as CustomDuel.json to replace the tutorial
QuickDuelScript.CreateAndSaveCustomDuel(gameServer, dataDirectory);
```

## Requirements

- Deck files must exist in the data directory
- Deck files can be in `.ydk` or `.json` format
- GameServer instance must be initialized

## Common Deck File Locations

- Player decks: `YgoMaster/Data/Players/[PlayerName]/Decks/`
- Custom decks: `YgoMaster/Data/`
- Imported decks: Place `.ydk` files directly in `YgoMaster/Data/`

## Related Documentation

- [CreateDuelInterface.md](../CreateDuelInterface.md) - Complete API documentation
- [Settings.md](../Settings.md) - Server settings reference
- [SoloFileFormat.md](../SoloFileFormat.md) - Solo duel format details

## Tips

1. **Test with simple decks first**: Start with basic decks to verify your setup works
2. **Use YDK format for compatibility**: YDK files are widely supported and easy to find online
3. **Check file paths**: Most errors are due to incorrect file paths
4. **Validate decks**: Ensure decks are valid (40-60 main, 0-15 extra, 0-15 side)
5. **Experiment with settings**: Try different CPU difficulty and behavior flags

## Integration Points

The `CreateDuel` method can be integrated at several points:

1. **Script-based integration**: Call directly from custom scripts
2. **CustomDuel.json**: Save output to replace tutorial duels
3. **Server modifications**: Integrate into server logic for dynamic duel creation
4. **External tools**: Use from external tools that interface with YgoMaster

## Support

For more information:
- See the main [README.md](../../README.md) for general YgoMaster documentation
- Check [CreateDuelInterface.md](../CreateDuelInterface.md) for API details
- Review the source code in `YgoMasterServer/Acts/Act_Duel.cs`
