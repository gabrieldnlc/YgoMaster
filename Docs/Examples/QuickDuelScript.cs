// QuickDuelScript.cs
// A simple, copy-paste ready script for creating custom duels
// Place your deck files in YgoMaster/Data/ directory

using System;
using System.Collections.Generic;
using System.IO;
using YgoMaster;

/// <summary>
/// Quick start script for creating a custom duel
/// Modify the deck file names and settings below to create your own duels
/// </summary>
public class QuickDuelScript
{
    /// <summary>
    /// Main entry point - call this method to create a duel
    /// </summary>
    public static DuelSettings CreateMyDuel(GameServer gameServer, string dataDirectory)
    {
        // ============================================
        // CONFIGURATION - Modify these values
        // ============================================
        
        // Deck file names (should be in dataDirectory)
        string playerDeckFile = "PlayerDeck.ydk";   // Your deck
        string opponentDeckFile = "OpponentDeck.ydk"; // Opponent's deck
        
        // Duel settings (modify as needed)
        int firstPlayer = -1;          // -1 = random, 0 = player, 1 = opponent
        int playerLifePoints = 8000;    // Starting LP for player
        int opponentLifePoints = 8000;  // Starting LP for opponent
        int startingHandSize = 5;       // Cards in starting hand
        bool shuffleDecks = true;       // true = shuffle, false = no shuffle
        string playerName = "Player";   // Player 1 name
        string opponentName = "CPU";    // Player 2 name
        
        // ============================================
        // DECK LOADING
        // ============================================
        
        try
        {
            Console.WriteLine("Loading decks...");
            
            // Load player deck
            DeckInfo playerDeck = new DeckInfo();
            playerDeck.File = Path.Combine(dataDirectory, playerDeckFile);
            if (!File.Exists(playerDeck.File))
            {
                Console.WriteLine("ERROR: Player deck file not found: " + playerDeck.File);
                return null;
            }
            playerDeck.Load();
            Console.WriteLine("Player deck loaded: " + playerDeckFile);
            
            // Load opponent deck
            DeckInfo opponentDeck = new DeckInfo();
            opponentDeck.File = Path.Combine(dataDirectory, opponentDeckFile);
            if (!File.Exists(opponentDeck.File))
            {
                Console.WriteLine("ERROR: Opponent deck file not found: " + opponentDeck.File);
                return null;
            }
            opponentDeck.Load();
            Console.WriteLine("Opponent deck loaded: " + opponentDeckFile);
            
            // ============================================
            // DUEL CREATION
            // ============================================
            
            Console.WriteLine("Creating duel...");
            
            // Create settings dictionary
            Dictionary<string, object> settings = new Dictionary<string, object>()
            {
                { "FirstPlayer", firstPlayer },
                { "life", new int[] { playerLifePoints, opponentLifePoints } },
                { "hnum", new int[] { startingHandSize, startingHandSize } },
                { "noshuffle", !shuffleDecks },
                { "name", new string[] { playerName, opponentName } },
            };
            
            // Create the duel using the CreateDuel interface
            DuelSettings duel = gameServer.CreateDuel(playerDeck, opponentDeck, settings);
            
            // ============================================
            // SUCCESS
            // ============================================
            
            Console.WriteLine("Duel created successfully!");
            Console.WriteLine("  Player: " + duel.name[0] + " (" + duel.life[0] + " LP)");
            Console.WriteLine("  Opponent: " + duel.name[1] + " (" + duel.life[1] + " LP)");
            Console.WriteLine("  First player: " + (duel.FirstPlayer == 0 ? duel.name[0] : duel.name[1]));
            Console.WriteLine("  Shuffling: " + (shuffleDecks ? "Enabled" : "Disabled"));
            
            return duel;
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR creating duel: " + e.Message);
            Console.WriteLine(e.StackTrace);
            return null;
        }
    }
    
    /// <summary>
    /// Alternative version that saves the duel to CustomDuel.json
    /// This allows the duel to replace the tutorial duel
    /// </summary>
    public static void CreateAndSaveCustomDuel(GameServer gameServer, string dataDirectory)
    {
        // Create the duel
        DuelSettings duel = CreateMyDuel(gameServer, dataDirectory);
        
        if (duel == null)
        {
            Console.WriteLine("Failed to create duel, not saving.");
            return;
        }
        
        try
        {
            // Convert to dictionary
            Dictionary<string, object> duelData = duel.ToDictionary();
            
            // Add deck file names for CustomDuel.json format
            duelData["Deck"] = new List<object>()
            {
                "PlayerDeck.ydk",
                "OpponentDeck.ydk"
            };
            
            // Target the tutorial duel (chapter 10001)
            duelData["targetChapterId"] = 10001;
            
            // Save to CustomDuel.json
            string customDuelPath = Path.Combine(dataDirectory, "CustomDuel.json");
            string json = MiniJSON.Json.Serialize(duelData);
            File.WriteAllText(customDuelPath, json);
            
            Console.WriteLine("Duel saved to CustomDuel.json!");
            Console.WriteLine("The tutorial duel will now use these settings.");
        }
        catch (Exception e)
        {
            Console.WriteLine("ERROR saving CustomDuel.json: " + e.Message);
        }
    }
}
