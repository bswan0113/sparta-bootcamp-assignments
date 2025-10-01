using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using TextRpg3.Data.Models;

namespace TextRpg3.Data
{
    public static class DataManager
    {
        public static Player Player { get; private set; }
        public static List<Item> Items { get; private set; }
        public static List<Item> ItemsInShop { get; private set; }
        public static List<Dungeon> Dungeons { get; private set; }

        public static void Init()
        {
            LoadPlayerData();
            LoadItemData();
            Player.Inventory = new List<Item>(Items);
            ItemsInShop = new List<Item>();
            Dungeons = new List<Dungeon>();
            LoadShopCatalogItemData();
            LoadDungeonData();
        }

        private static void LoadPlayerData()
        {
            string jsonString = File.ReadAllText("Resources/player_data.json");
            var players = JsonSerializer.Deserialize<List<Player>>(jsonString);
            Player = players[0];
        }

        private static void LoadItemData()
        {
            string jsonString = File.ReadAllText("Resources/player_inventory.json");
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };
            Items = JsonSerializer.Deserialize<List<Item>>(jsonString, options)
                .OrderByDescending(item => item.Name.Length)
                .ThenBy(item => item.Name)
                .ToList();
        }
        private static void LoadShopCatalogItemData()
        {
            string jsonString = File.ReadAllText("Resources/items_in_shop.json");
            var options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };
            ItemsInShop = JsonSerializer.Deserialize<List<Item>>(jsonString, options)
                .OrderByDescending(item => item.Name.Length)
                .ThenBy(item => item.Name)
                .ToList();
        }

        private static void LoadDungeonData()
        {
            string jsonString = File.ReadAllText("Resources/dungeon.json");
            Dungeons = JsonSerializer.Deserialize<List<Dungeon>>(jsonString);
        }
        
        public static Item CreateItemInstance(Item item)
        {
            return new Item(item.Name, item.Description, item.AttackBonus, item.DefenseBonus, item.Price,item.ItemType);
        }

        public static void SaveGame(Player player)
        {
            var playerListToSave = new List<Player> { player };
            var playerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            string playerJson = JsonSerializer.Serialize(playerListToSave, playerOptions);
            File.WriteAllText("Resources/player_data.json", playerJson);

            var inventoryOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                Converters = { new JsonStringEnumConverter() }
            };
            string inventoryJson = JsonSerializer.Serialize(player.Inventory, inventoryOptions);
            File.WriteAllText("Resources/player_inventory.json", inventoryJson);
        }
    }
}