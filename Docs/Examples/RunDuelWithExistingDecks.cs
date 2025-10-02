// RunDuelWithExistingDecks.cs
// Practical example that runs a duel using existing AI/NPC decks from the game
// This demonstrates how to use the CreateDuel interface with real deck data

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YgoMaster;

/// <summary>
/// Example showing how to run a duel with existing AI/structure decks
/// </summary>
public class RunDuelWithExistingDecks
{
    /// <summary>
    /// Example 1: Create a duel using two random structure decks
    /// Structure decks are pre-built decks that come with the game
    /// </summary>
    public static DuelSettings RunDuelWithRandomStructureDecks(GameServer gameServer, string dataDirectory)
    {
        Console.WriteLine("=== Running Duel with Random Structure Decks ===");
        
        try
        {
            // Find all structure deck files
            string structureDecksPath = Path.Combine(dataDirectory, "StructureDecks");
            if (!Directory.Exists(structureDecksPath))
            {
                Console.WriteLine("ERROR: StructureDecks directory not found at: " + structureDecksPath);
                return null;
            }
            
            string[] deckFiles = Directory.GetFiles(structureDecksPath, "*.json");
            if (deckFiles.Length < 2)
            {
                Console.WriteLine("ERROR: Not enough structure decks found. Need at least 2.");
                return null;
            }
            
            // Pick two random structure decks
            Random random = new Random();
            string deck1File = deckFiles[random.Next(deckFiles.Length)];
            string deck2File = deckFiles[random.Next(deckFiles.Length)];
            
            // Ensure we have different decks
            while (deck1File == deck2File && deckFiles.Length > 1)
            {
                deck2File = deckFiles[random.Next(deckFiles.Length)];
            }
            
            Console.WriteLine("Player deck: " + Path.GetFileName(deck1File));
            Console.WriteLine("Opponent deck: " + Path.GetFileName(deck2File));
            
            // Load the decks
            DeckInfo playerDeck = LoadStructureDeck(deck1File);
            DeckInfo opponentDeck = LoadStructureDeck(deck2File);
            
            if (playerDeck == null || opponentDeck == null)
            {
                Console.WriteLine("ERROR: Failed to load one or both decks");
                return null;
            }
            
            // Create custom settings for the duel
            Dictionary<string, object> settings = new Dictionary<string, object>()
            {
                { "FirstPlayer", -1 }, // Random first player
                { "life", new int[] { 8000, 8000 } },
                { "hnum", new int[] { 5, 5 } },
                { "name", new string[] { "Player", "AI Opponent" } },
                { "cpu", 100 }, // Standard AI difficulty
            };
            
            // Create the duel using the CreateDuel interface
            DuelSettings duel = gameServer.CreateDuel(playerDeck, opponentDeck, settings);
            
            Console.WriteLine("✓ Duel created successfully!");
            Console.WriteLine("  Player 1: " + duel.name[0] + " (" + playerDeck.MainDeckCards.Count + " main deck cards)");
            Console.WriteLine("  Player 2: " + duel.name[1] + " (" + opponentDeck.MainDeckCards.Count + " main deck cards)");
            Console.WriteLine("  First player: " + (duel.FirstPlayer == 0 ? duel.name[0] : duel.name[1]));
            Console.WriteLine("  Life points: " + duel.life[0] + " vs " + duel.life[1]);
            Console.WriteLine("  Random seed: " + duel.RandSeed);
            
            return duel;
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR: " + e.Message);
            Console.WriteLine(e.StackTrace);
            return null;
        }
    }
    
    /// <summary>
    /// Example 2: Create a duel using specific structure decks by ID
    /// </summary>
    public static DuelSettings RunDuelWithSpecificStructureDecks(GameServer gameServer, string dataDirectory)
    {
        Console.WriteLine("=== Running Duel with Specific Structure Decks ===");
        
        try
        {
            // Use specific structure deck IDs
            // 1120002 = Power of the Dragon (Dragon-themed deck)
            // 1120004 = Synchro of Unity (Synchro-themed deck)
            string deck1File = Path.Combine(dataDirectory, "StructureDecks", "1120002.json");
            string deck2File = Path.Combine(dataDirectory, "StructureDecks", "1120004.json");
            
            Console.WriteLine("Player deck: 1120002 (Power of the Dragon)");
            Console.WriteLine("Opponent deck: 1120004 (Synchro of Unity)");
            
            // Load the decks
            DeckInfo playerDeck = LoadStructureDeck(deck1File);
            DeckInfo opponentDeck = LoadStructureDeck(deck2File);
            
            if (playerDeck == null || opponentDeck == null)
            {
                Console.WriteLine("ERROR: Failed to load one or both decks");
                return null;
            }
            
            // Create settings
            Dictionary<string, object> settings = new Dictionary<string, object>()
            {
                { "FirstPlayer", 0 }, // Player goes first
                { "life", new int[] { 8000, 8000 } },
                { "name", new string[] { "Dragon Master", "Synchro Expert" } },
            };
            
            // Create the duel
            DuelSettings duel = gameServer.CreateDuel(playerDeck, opponentDeck, settings);
            
            Console.WriteLine("✓ Duel created successfully!");
            Console.WriteLine("  " + duel.name[0] + " vs " + duel.name[1]);
            Console.WriteLine("  " + duel.name[0] + " goes first");
            
            return duel;
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR: " + e.Message);
            return null;
        }
    }
    
    /// <summary>
    /// Example 3: Load a duel configuration from Solo mode and run it
    /// This uses the same deck configuration that an AI opponent uses in story mode
    /// </summary>
    public static DuelSettings RunSoloModeDuelConfiguration(GameServer gameServer, string dataDirectory)
    {
        Console.WriteLine("=== Running Duel with Solo Mode Configuration ===");
        
        try
        {
            // Load a solo duel file (these contain pre-configured duels from story mode)
            string soloDuelFile = Path.Combine(dataDirectory, "SoloDuels", "10001.json");
            
            if (!File.Exists(soloDuelFile))
            {
                Console.WriteLine("ERROR: Solo duel file not found: " + soloDuelFile);
                return null;
            }
            
            Console.WriteLine("Loading configuration from: " + Path.GetFileName(soloDuelFile));
            
            // Parse the solo duel file
            string jsonContent = File.ReadAllText(soloDuelFile);
            Dictionary<string, object> soloDuelData = MiniJSON.Json.Deserialize(jsonContent) as Dictionary<string, object>;
            
            if (soloDuelData == null)
            {
                Console.WriteLine("ERROR: Failed to parse solo duel file");
                return null;
            }
            
            // Extract the duel settings from the solo duel data
            // Solo duel files have structure: {"code":0,"res":[[69,{"Duel":{...}}]]}
            List<object> res = Utils.GetValue<List<object>>(soloDuelData, "res");
            if (res != null && res.Count > 0)
            {
                List<object> innerArray = res[0] as List<object>;
                if (innerArray != null && innerArray.Count > 1)
                {
                    Dictionary<string, object> duelContainer = innerArray[1] as Dictionary<string, object>;
                    if (duelContainer != null && duelContainer.ContainsKey("Duel"))
                    {
                        Dictionary<string, object> duelData = Utils.GetDictionary(duelContainer, "Duel");
                        
                        // Extract deck information
                        List<object> deckArray = Utils.GetValue<List<object>>(duelData, "Deck");
                        if (deckArray != null && deckArray.Count >= 2)
                        {
                            // Load decks from the configuration
                            DeckInfo playerDeck = new DeckInfo();
                            playerDeck.FromDictionary(deckArray[0] as Dictionary<string, object>, true);
                            
                            DeckInfo opponentDeck = new DeckInfo();
                            opponentDeck.FromDictionary(deckArray[1] as Dictionary<string, object>, true);
                            
                            Console.WriteLine("Player deck: " + playerDeck.MainDeckCards.Count + " main cards");
                            Console.WriteLine("Opponent deck: " + opponentDeck.MainDeckCards.Count + " main cards");
                            
                            // Extract some settings from the original configuration
                            int[] lifePoints = Utils.GetIntArray(duelData, "life");
                            string[] names = Utils.GetStringArray(duelData, "name");
                            
                            Dictionary<string, object> settings = new Dictionary<string, object>()
                            {
                                { "life", lifePoints != null ? lifePoints : new int[] { 8000, 8000 } },
                                { "name", names != null ? names : new string[] { "Player", "Opponent" } },
                                { "FirstPlayer", -1 }, // Random
                            };
                            
                            // Create the duel
                            DuelSettings duel = gameServer.CreateDuel(playerDeck, opponentDeck, settings);
                            
                            Console.WriteLine("✓ Duel created successfully from solo mode configuration!");
                            Console.WriteLine("  " + duel.name[0] + " (" + duel.life[0] + " LP) vs " + duel.name[1] + " (" + duel.life[1] + " LP)");
                            
                            return duel;
                        }
                    }
                }
            }
            
            Console.WriteLine("ERROR: Could not extract duel configuration from solo duel file");
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR: " + e.Message);
            Console.WriteLine(e.StackTrace);
            return null;
        }
    }
    
    /// <summary>
    /// Helper method to load a structure deck from a JSON file
    /// </summary>
    private static DeckInfo LoadStructureDeck(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("ERROR: Deck file not found: " + filePath);
                return null;
            }
            
            // Read and parse the structure deck JSON
            string jsonContent = File.ReadAllText(filePath);
            Dictionary<string, object> deckData = MiniJSON.Json.Deserialize(jsonContent) as Dictionary<string, object>;
            
            if (deckData == null)
            {
                Console.WriteLine("ERROR: Failed to parse deck JSON");
                return null;
            }
            
            // Create a DeckInfo object
            DeckInfo deck = new DeckInfo();
            
            // Structure decks have format: {"structure_id":X,"accessory":{...},"focus":{...},"contents":{"m":{...},"e":{...},"s":{...}}}
            Dictionary<string, object> contents = Utils.GetDictionary(deckData, "contents");
            if (contents != null)
            {
                // Load main deck
                Dictionary<string, object> mainDeck = Utils.GetDictionary(contents, "m");
                if (mainDeck != null)
                {
                    List<int> cardIds = Utils.GetValueTypeList<int>(mainDeck, "ids");
                    List<int> rarities = Utils.GetValueTypeList<int>(mainDeck, "r");
                    
                    if (cardIds != null && rarities != null)
                    {
                        for (int i = 0; i < cardIds.Count && i < rarities.Count; i++)
                        {
                            deck.MainDeckCards.Add(cardIds[i], (CardStyleRarity)rarities[i]);
                        }
                    }
                }
                
                // Load extra deck
                Dictionary<string, object> extraDeck = Utils.GetDictionary(contents, "e");
                if (extraDeck != null)
                {
                    List<int> cardIds = Utils.GetValueTypeList<int>(extraDeck, "ids");
                    List<int> rarities = Utils.GetValueTypeList<int>(extraDeck, "r");
                    
                    if (cardIds != null && rarities != null)
                    {
                        for (int i = 0; i < cardIds.Count && i < rarities.Count; i++)
                        {
                            deck.ExtraDeckCards.Add(cardIds[i], (CardStyleRarity)rarities[i]);
                        }
                    }
                }
                
                // Load side deck (usually empty for structure decks)
                Dictionary<string, object> sideDeck = Utils.GetDictionary(contents, "s");
                if (sideDeck != null)
                {
                    List<int> cardIds = Utils.GetValueTypeList<int>(sideDeck, "ids");
                    List<int> rarities = Utils.GetValueTypeList<int>(sideDeck, "r");
                    
                    if (cardIds != null && rarities != null)
                    {
                        for (int i = 0; i < cardIds.Count && i < rarities.Count; i++)
                        {
                            deck.SideDeckCards.Add(cardIds[i], (CardStyleRarity)rarities[i]);
                        }
                    }
                }
            }
            
            // Load accessory information if present
            Dictionary<string, object> accessory = Utils.GetDictionary(deckData, "accessory");
            if (accessory != null)
            {
                deck.Accessory.Sleeve = Utils.GetValue<int>(accessory, "sleeve");
                deck.Accessory.Field = Utils.GetValue<int>(accessory, "box");
            }
            
            // Set deck name from structure ID
            int structureId = Utils.GetValue<int>(deckData, "structure_id");
            deck.Name = "Structure Deck " + structureId;
            deck.File = filePath;
            
            Console.WriteLine("  Loaded: " + deck.Name + " (" + deck.MainDeckCards.Count + " main, " + deck.ExtraDeckCards.Count + " extra)");
            
            return deck;
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR loading deck from " + filePath + ": " + e.Message);
            return null;
        }
    }
    
    /// <summary>
    /// Main entry point - demonstrates all examples
    /// </summary>
    public static void RunAllExamples(GameServer gameServer, string dataDirectory)
    {
        Console.WriteLine("========================================");
        Console.WriteLine("CreateDuel Examples with Existing Decks");
        Console.WriteLine("========================================");
        Console.WriteLine();
        
        // Example 1: Random structure decks
        DuelSettings duel1 = RunDuelWithRandomStructureDecks(gameServer, dataDirectory);
        Console.WriteLine();
        
        // Example 2: Specific structure decks
        DuelSettings duel2 = RunDuelWithSpecificStructureDecks(gameServer, dataDirectory);
        Console.WriteLine();
        
        // Example 3: Solo mode configuration
        DuelSettings duel3 = RunSoloModeDuelConfiguration(gameServer, dataDirectory);
        Console.WriteLine();
        
        Console.WriteLine("========================================");
        Console.WriteLine("Examples completed!");
        if (duel1 != null && duel2 != null && duel3 != null)
        {
            Console.WriteLine("All 3 duels created successfully!");
        }
        Console.WriteLine("========================================");
    }
}
