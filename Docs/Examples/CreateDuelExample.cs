// Example: Using the CreateDuel interface to create custom duels
// This file demonstrates how to use the GameServer.CreateDuel method

using System;
using System.Collections.Generic;
using System.IO;
using YgoMaster;

namespace YgoMasterExamples
{
    /// <summary>
    /// Example class demonstrating how to use the CreateDuel interface
    /// </summary>
    public class CreateDuelExample
    {
        /// <summary>
        /// Example 1: Create a simple duel with two YDK decks
        /// </summary>
        public static void SimpleDuelExample(GameServer gameServer, string dataDirectory)
        {
            // Load player deck from YDK file
            DeckInfo playerDeck = new DeckInfo();
            playerDeck.File = Path.Combine(dataDirectory, "Decks", "PlayerDeck.ydk");
            playerDeck.Load();

            // Load opponent deck from YDK file
            DeckInfo opponentDeck = new DeckInfo();
            opponentDeck.File = Path.Combine(dataDirectory, "Decks", "OpponentDeck.ydk");
            opponentDeck.Load();

            // Create the duel with default settings
            DuelSettings duel = gameServer.CreateDuel(playerDeck, opponentDeck, null);

            Console.WriteLine("Simple duel created!");
            Console.WriteLine("Player 1: " + duel.name[0]);
            Console.WriteLine("Player 2: " + duel.name[1]);
            Console.WriteLine("First player: " + duel.FirstPlayer);
        }

        /// <summary>
        /// Example 2: Create a duel with custom settings
        /// </summary>
        public static void CustomSettingsDuelExample(GameServer gameServer, string dataDirectory)
        {
            // Load decks
            DeckInfo playerDeck = new DeckInfo();
            playerDeck.File = Path.Combine(dataDirectory, "Decks", "PlayerDeck.json");
            playerDeck.Load();

            DeckInfo opponentDeck = new DeckInfo();
            opponentDeck.File = Path.Combine(dataDirectory, "Decks", "OpponentDeck.json");
            opponentDeck.Load();

            // Define custom settings
            Dictionary<string, object> settings = new Dictionary<string, object>()
            {
                { "FirstPlayer", 0 }, // Player goes first
                { "life", new int[] { 4000, 4000 } }, // Lower life points
                { "hnum", new int[] { 6, 6 } }, // Start with 6 cards
                { "name", new string[] { "Hero", "Villain" } }, // Custom names
                { "noshuffle", true }, // Disable shuffling for consistent testing
            };

            // Create the duel with custom settings
            DuelSettings duel = gameServer.CreateDuel(playerDeck, opponentDeck, settings);

            Console.WriteLine("Custom duel created!");
            Console.WriteLine("Life points: " + duel.life[0] + " vs " + duel.life[1]);
            Console.WriteLine("Starting hand: " + duel.hnum[0] + " cards each");
        }

        /// <summary>
        /// Example 3: Create multiple duels in a tournament-style setup
        /// </summary>
        public static void TournamentExample(GameServer gameServer, string dataDirectory)
        {
            // List of deck files for the tournament
            string[] deckFiles = new string[]
            {
                "Deck1.ydk",
                "Deck2.ydk",
                "Deck3.ydk",
                "Deck4.ydk"
            };

            // Load all decks
            List<DeckInfo> decks = new List<DeckInfo>();
            foreach (string deckFile in deckFiles)
            {
                DeckInfo deck = new DeckInfo();
                deck.File = Path.Combine(dataDirectory, "Decks", deckFile);
                deck.Load();
                decks.Add(deck);
            }

            // Create duels for each pair
            Console.WriteLine("Creating tournament duels...");
            for (int i = 0; i < decks.Count; i += 2)
            {
                if (i + 1 < decks.Count)
                {
                    DuelSettings duel = gameServer.CreateDuel(decks[i], decks[i + 1], null);
                    Console.WriteLine("Match " + ((i / 2) + 1) + ": Created");
                }
            }
        }

        /// <summary>
        /// Example 4: Create a practice duel with specific CPU behavior
        /// </summary>
        public static void PracticeDuelExample(GameServer gameServer, string dataDirectory)
        {
            // Load player deck
            DeckInfo playerDeck = new DeckInfo();
            playerDeck.File = Path.Combine(dataDirectory, "Decks", "MyDeck.ydk");
            playerDeck.Load();

            // Load practice opponent deck
            DeckInfo opponentDeck = new DeckInfo();
            opponentDeck.File = Path.Combine(dataDirectory, "Decks", "PracticeDeck.ydk");
            opponentDeck.Load();

            // Settings for practice mode
            Dictionary<string, object> settings = new Dictionary<string, object>()
            {
                { "FirstPlayer", 0 }, // Player always goes first in practice
                { "cpu", 50 }, // Easier CPU difficulty
                { "cpuflag", "Light" }, // CPU plays lightly
                { "name", new string[] { "Player", "Practice Bot" } },
            };

            DuelSettings duel = gameServer.CreateDuel(playerDeck, opponentDeck, settings);

            Console.WriteLine("Practice duel created!");
            Console.WriteLine("CPU difficulty set to: " + duel.cpu);
        }

        /// <summary>
        /// Example 5: Create a duel from random decks in a directory
        /// </summary>
        public static void RandomDeckDuelExample(GameServer gameServer, string dataDirectory)
        {
            string decksDir = Path.Combine(dataDirectory, "Decks");
            
            // Get all deck files
            string[] deckFiles = Directory.GetFiles(decksDir, "*.ydk");
            
            if (deckFiles.Length < 2)
            {
                Console.WriteLine("Not enough decks found!");
                return;
            }

            // Pick two random decks
            Random random = new Random();
            string playerDeckFile = deckFiles[random.Next(deckFiles.Length)];
            string opponentDeckFile = deckFiles[random.Next(deckFiles.Length)];

            // Load decks
            DeckInfo playerDeck = new DeckInfo();
            playerDeck.File = playerDeckFile;
            playerDeck.Load();

            DeckInfo opponentDeck = new DeckInfo();
            opponentDeck.File = opponentDeckFile;
            opponentDeck.Load();

            // Create duel with random decks
            DuelSettings duel = gameServer.CreateDuel(playerDeck, opponentDeck, null);

            Console.WriteLine("Random duel created!");
            Console.WriteLine("Player deck: " + Path.GetFileName(playerDeckFile));
            Console.WriteLine("Opponent deck: " + Path.GetFileName(opponentDeckFile));
        }

        /// <summary>
        /// Example 6: Integration with CustomDuel.json approach
        /// Shows how to save a created duel as CustomDuel.json
        /// </summary>
        public static void SaveAsCustomDuelJson(GameServer gameServer, string dataDirectory)
        {
            // Create a duel programmatically
            DeckInfo playerDeck = new DeckInfo();
            playerDeck.File = Path.Combine(dataDirectory, "Decks", "Deck1.ydk");
            playerDeck.Load();

            DeckInfo opponentDeck = new DeckInfo();
            opponentDeck.File = Path.Combine(dataDirectory, "Decks", "Deck2.ydk");
            opponentDeck.Load();

            Dictionary<string, object> settings = new Dictionary<string, object>()
            {
                { "FirstPlayer", 1 }, // Opponent goes first
                { "life", new int[] { 8000, 8000 } },
            };

            DuelSettings duel = gameServer.CreateDuel(playerDeck, opponentDeck, settings);

            // Convert to dictionary and save as CustomDuel.json
            Dictionary<string, object> duelData = duel.ToDictionary();
            
            // Add deck information for CustomDuel.json format
            duelData["Deck"] = new List<object>()
            {
                Path.GetFileName(playerDeck.File),
                Path.GetFileName(opponentDeck.File)
            };
            duelData["targetChapterId"] = 10001; // Tutorial duel chapter ID

            // Save to CustomDuel.json
            string customDuelPath = Path.Combine(dataDirectory, "CustomDuel.json");
            string json = MiniJSON.Json.Serialize(duelData);
            File.WriteAllText(customDuelPath, json);

            Console.WriteLine("Duel saved to CustomDuel.json!");
        }

        /// <summary>
        /// Example 7: Error handling when creating duels
        /// </summary>
        public static void ErrorHandlingExample(GameServer gameServer, string dataDirectory)
        {
            try
            {
                DeckInfo playerDeck = new DeckInfo();
                playerDeck.File = Path.Combine(dataDirectory, "Decks", "NonExistentDeck.ydk");
                
                // This will throw an exception because the file doesn't exist
                playerDeck.Load();

                DeckInfo opponentDeck = new DeckInfo();
                opponentDeck.File = Path.Combine(dataDirectory, "Decks", "OpponentDeck.ydk");
                opponentDeck.Load();

                DuelSettings duel = gameServer.CreateDuel(playerDeck, opponentDeck, null);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Deck file not found: " + e.Message);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Null deck provided: " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error creating duel: " + e.Message);
            }
        }

        /// <summary>
        /// Example 8: Creating a duel with validation
        /// </summary>
        public static void ValidatedDuelExample(GameServer gameServer, Player player, string dataDirectory)
        {
            // Load decks
            DeckInfo playerDeck = new DeckInfo();
            playerDeck.File = Path.Combine(dataDirectory, "Decks", "PlayerDeck.ydk");
            playerDeck.Load();

            DeckInfo opponentDeck = new DeckInfo();
            opponentDeck.File = Path.Combine(dataDirectory, "Decks", "OpponentDeck.ydk");
            opponentDeck.Load();

            // Validate decks before creating duel
            // Note: You would need access to regulations dictionary from GameServer
            // This is just an example of the validation pattern
            bool playerDeckValid = true; // playerDeck.IsValid(player, regulationId, regulations);
            bool opponentDeckValid = true; // opponentDeck.IsValid(player, regulationId, regulations);

            if (!playerDeckValid)
            {
                Console.WriteLine("Player deck is invalid!");
                return;
            }

            if (!opponentDeckValid)
            {
                Console.WriteLine("Opponent deck is invalid!");
                return;
            }

            // Create duel after validation
            DuelSettings duel = gameServer.CreateDuel(playerDeck, opponentDeck, null);
            Console.WriteLine("Validated duel created successfully!");
        }
    }
}
