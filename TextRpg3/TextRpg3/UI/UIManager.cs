using System.Drawing;
using System.Text.Json;
using System.Text.RegularExpressions;
using TextRpg3.Data;
using TextRpg3.Data.Models;

namespace TextRpg3.UI
{
    public class UIManager
    {

        private void WriteLineColor(bool isWriteLine, string text)
        {
            Random random = new Random();

            ConsoleColor[] colors = (ConsoleColor[])Enum.GetValues(typeof(ConsoleColor));
            ConsoleColor color = colors[random.Next(1, colors.Length)];
            Console.ForegroundColor = color;

            if (isWriteLine) Console.WriteLine(text);
            else Console.Write(text);
        }

        public void DisplayMainMenu()
        {
            Console.Clear();
            var scene = TextManager.GetScene("MainMenu");

            WriteLineColor(true,scene.GetProperty("Title").GetString());
            WriteLineColor(true,scene.GetProperty("Prompt").GetString());
            Console.WriteLine();

            foreach (var choice in scene.GetProperty("Choices").EnumerateArray())
            {
                WriteLineColor(true,choice.GetString());
            }

            Console.WriteLine();
            WriteLineColor(true,TextManager.GetCommonText("Prompt"));
            WriteLineColor(false,TextManager.GetCommonText("InputPrefix"));
        }

        public void DisplayStatusScreen(Player player)
        {
            Console.Clear();
            var scene = TextManager.GetScene("StatusScreen");
            var common = TextManager.GetScene("Common");

            WriteLineColor(true,scene.GetProperty("Title").GetString());
            WriteLineColor(true,scene.GetProperty("Description").GetString());
            Console.WriteLine();

            float totalAttack = player.GetTotalAttack();
            int totalDefense = player.GetTotalDefense();

            var statusData = new Dictionary<string, object>
            {
                { "Level", player.Level },
                { "Name", player.Name },
                { "Job", player.Job },
                { "TotalAttack", totalAttack },
                { "TotalDefense", totalDefense },
                { "Hp", player.Hp },
                { "Gold", player.Gold },
                { "Exp", player.Exp },
                { "Stamina", player.Stamina },
                { "AttackBonusText", player.Attack != totalAttack ? string.Format(common.GetProperty("BonusStat").GetString(), totalAttack - player.Attack) : "" },
                { "DefenseBonusText", player.Defense != totalDefense ? string.Format(common.GetProperty("BonusStat").GetString(), totalDefense - player.Defense) : "" }
            };

            foreach (var lineTemplate in scene.GetProperty("Body_Lines").EnumerateArray())
            {
                WriteLineColor(true,RenderTemplate(lineTemplate.GetString(), statusData));
            }

            Console.WriteLine();

            foreach (var choice in scene.GetProperty("Choices").EnumerateArray())
            {
                WriteLineColor(true,choice.GetString());
            }

            Console.WriteLine();
            WriteLineColor(true,TextManager.GetCommonText("Prompt"));
            WriteLineColor(false,TextManager.GetCommonText("InputPrefix"));
        }

        public void DisplayInventoryScreen(List<Item> inventory)
        {
            DisplayInventory("InventoryScreen", inventory, false);
        }

        public void DisplayEquipScreen(List<Item> inventory)
        {
            DisplayInventory("EquipScreen", inventory, true);
        }

        private void DisplayInventory(string sceneName, List<Item> inventory, bool showNumbers)
        {
            Console.Clear();
            var scene = TextManager.GetScene(sceneName);
            string Equipped = TextManager.GetCommonText("Equipped");

            WriteLineColor(true,scene.GetProperty("Title").GetString());
            WriteLineColor(true,scene.GetProperty("Description").GetString());
            Console.WriteLine();
            WriteLineColor(true,scene.GetProperty("Body_Header").GetString());

            for (int i = 0; i < inventory.Count; i++)
            {
                string prefix = showNumbers ? $"{i + 1}. " : "- ";
                string equipped = inventory[i].IsEquipped ? Equipped : "";
                string attack = inventory[i].AttackBonus > 0 ? $"공격력 +{inventory[i].AttackBonus}" : "";
                string defense = inventory[i].DefenseBonus > 0 ? $"방어력 +{inventory[i].DefenseBonus}" : "";
                string statText = string.Join(" | ", new[] { attack, defense }.Where(s => !string.IsNullOrEmpty(s)));
                WriteLineColor(true,$"{prefix}{equipped}{inventory[i].Name,-10} | {statText,-10} | {inventory[i].Description}");
            }

            Console.WriteLine();

            foreach (var choice in scene.GetProperty("Choices").EnumerateArray())
            {
                WriteLineColor(true,choice.GetString());
            }

            Console.WriteLine();
            WriteLineColor(true,TextManager.GetCommonText("Prompt"));
            WriteLineColor(false,TextManager.GetCommonText("InputPrefix"));
        }

        public void DisplayAdventureResult(bool isSuccess)
        {
            JsonElement adventureResultJson = TextManager.GetScene("AdventureResult");
            if (isSuccess)
            {
                WriteLineColor(true,adventureResultJson.GetProperty("ResultSuceess").GetString());
            }
            else
            {
                WriteLineColor(true,adventureResultJson.GetProperty("ResultFailed").GetString());
            }
            Thread.Sleep(1000);

        }
        public void DisplayPatrolResult(int patrolResult)
        {
            JsonElement adventureResultJson = TextManager.GetScene("PatrolResult");
            if (patrolResult < 1)
            {
                WriteLineColor(true,adventureResultJson.GetProperty("MeetChildren").GetString());
            }else if(patrolResult < 2)
            {
                WriteLineColor(true,adventureResultJson.GetProperty("MeetVillageChief").GetString());
            }else if (patrolResult < 4)
            {
                WriteLineColor(true,adventureResultJson.GetProperty("GoodAction").GetString());
            }else if (patrolResult < 7)
            {
                WriteLineColor(true,adventureResultJson.GetProperty("MeetAnyone").GetString());
            }
            else
            {
                WriteLineColor(true,adventureResultJson.GetProperty("NotingHappened").GetString());
            }
            Thread.Sleep(1000);
        }
        public void DisplayTrainingResult(int trainingResult)
        {
            JsonElement trainingResultJson = TextManager.GetScene("TrainingResult");
            if (trainingResult < 15)
            {
                WriteLineColor(true,trainingResultJson.GetProperty("Good").GetString());
            }else if(trainingResult < 60)
            {
                WriteLineColor(true,trainingResultJson.GetProperty("VeryGood").GetString());
            }else if (trainingResult < 100)
            {
                WriteLineColor(true,trainingResultJson.GetProperty("NotGood").GetString());
            }
            Thread.Sleep(1000);
        }

        public void DisplayInsufficientStamina()
        {
            WriteLineColor(true, TextManager.GetCommonText("InsufficientStamina"));
            Thread.Sleep(1000);
        }
        public void DisplayInsufficientGold()
        {
            WriteLineColor(true, TextManager.GetCommonText("InsufficientGold"));
            Thread.Sleep(1000);
        }

        public void DisplayRestScreen(Player player)
        {
            Console.Clear();
            JsonElement restScreenJson = TextManager.GetScene("RestScreen");
            WriteLineColor(true,restScreenJson.GetProperty("Title").GetString());

            var playerData = new Dictionary<string, object>
            {
                { "Gold", player.Gold }
            };
            WriteLineColor(true,RenderTemplate(restScreenJson.GetProperty("Description").GetString(), playerData));
            foreach (var choice in restScreenJson.GetProperty("Choices").EnumerateArray())
            {
                WriteLineColor(true,choice.GetString());
            }
        }

        public void DisplayRestResult()
        {

            WriteLineColor(true,TextManager.GetScene("RestScreen").GetProperty("RestSuccess").GetString());
            Thread.Sleep(1000);
        }
        public void DisplayShopScreen(Player player, List<Item> itemsInShop)
        {
            Console.Clear();
            var scene = TextManager.GetScene("ShopScreen");
            var common = TextManager.GetScene("Common");

            WriteLineColor(true, scene.GetProperty("Title").GetString());
            WriteLineColor(true, scene.GetProperty("Description").GetString());
            Console.WriteLine();

            WriteLineColor(true, scene.GetProperty("Body_Gold_Header").GetString());
            var playerData = new Dictionary<string, object>
            {
                { "Gold", player.Gold }
            };
            WriteLineColor(true, RenderTemplate(scene.GetProperty("Body_Gold_Line").GetString(), playerData));
            Console.WriteLine();

            WriteLineColor(true, scene.GetProperty("Body_Item_Header").GetString());

            string purchasedStatus = scene.GetProperty("PurchasedStatus").GetString();

            for (int i = 0; i < itemsInShop.Count; i++)
            {
                Item shopItem = itemsInShop[i];
                bool isPurchased = player.Inventory.Any(item => item.Name == shopItem.Name);

                string prefix = $"{i + 1}. ";
                string attack = shopItem.AttackBonus > 0 ? $"공격력 +{shopItem.AttackBonus}" : "";
                string defense = shopItem.DefenseBonus > 0 ? $"방어력 +{shopItem.DefenseBonus}" : "";

                string statText = string.Join(" | ", new[] { attack, defense }.Where(s => !string.IsNullOrEmpty(s)));
                string priceText = isPurchased ? purchasedStatus : $"{shopItem.Price} G";

                WriteLineColor(true, $"{prefix}{shopItem.Name,-10} | {statText,-10} | {shopItem.Description,-30} | {priceText}");
            }

            Console.WriteLine();

            foreach (var choice in scene.GetProperty("Choices").EnumerateArray())
            {
                WriteLineColor(true, choice.GetString());
            }

            Console.WriteLine();
            WriteLineColor(true, TextManager.GetCommonText("Prompt"));
            WriteLineColor(false, TextManager.GetCommonText("InputPrefix"));
        }

        public void DisplayShopInSellScreen(Player player, List<Item> itemsInShop)
        {
            Console.Clear();
            var scene = TextManager.GetScene("ShopScreen");
            var common = TextManager.GetScene("Common");

            WriteLineColor(true, scene.GetProperty("Title_SellScreen").GetString());
            WriteLineColor(true, scene.GetProperty("Description_SellScreen").GetString());
            Console.WriteLine();

            WriteLineColor(true, scene.GetProperty("Body_Gold_Header").GetString());
            var playerData = new Dictionary<string, object>
            {
                { "Gold", player.Gold }
            };
            WriteLineColor(true, RenderTemplate(scene.GetProperty("Body_Gold_Line").GetString(), playerData));
            Console.WriteLine();

            WriteLineColor(true, scene.GetProperty("Body_Item_Header").GetString());

            string equippedText = common.GetProperty("Equipped").GetString();

            if (player.Inventory.Count == 0)
            {
                WriteLineColor(true, "소유하고 있는 아이템이 없습니다.");
            }
            else
            {
                for (int i = 0; i < player.Inventory.Count; i++)
                {
                    Item playerItem = player.Inventory[i];

                    Item originalShopItem = itemsInShop.FirstOrDefault(item => item.Name == playerItem.Name);
                    float basePrice = originalShopItem.Price;

                    string prefix = $"{i + 1}. ";
                    string equipped = playerItem.IsEquipped ? equippedText : "";
                    string attack = playerItem.AttackBonus > 0 ? $"공격력 +{playerItem.AttackBonus}" : "";
                    string defense = playerItem.DefenseBonus > 0 ? $"방어력 +{playerItem.DefenseBonus}" : "";

                    string statText = string.Join(" | ", new[] { attack, defense }.Where(s => !string.IsNullOrEmpty(s)));
                    string sellPrice = $"{basePrice * 0.85} G";

                    WriteLineColor(true, $"{prefix}{equipped}{playerItem.Name,-10} | {statText,-10} | {playerItem.Description,-30} | {sellPrice}");
                }
            }

            Console.WriteLine();
            WriteLineColor(true, scene.GetProperty("Choices").EnumerateArray().Last().GetString());
            Console.WriteLine();
            WriteLineColor(true, TextManager.GetCommonText("Prompt"));
            WriteLineColor(false, TextManager.GetCommonText("InputPrefix"));
        }
        public void DisplayShopInBuyScreen(Player player, List<Item> itemsInShop)
        {
            Console.Clear();
            var scene = TextManager.GetScene("ShopScreen");

            WriteLineColor(true, scene.GetProperty("Title_BuyScreen").GetString());
            WriteLineColor(true, scene.GetProperty("Description_BuyScreen").GetString());
            Console.WriteLine();

            WriteLineColor(true, scene.GetProperty("Body_Gold_Header").GetString());
            var playerData = new Dictionary<string, object>
            {
                { "Gold", player.Gold }
            };
            WriteLineColor(true, RenderTemplate(scene.GetProperty("Body_Gold_Line").GetString(), playerData));
            Console.WriteLine();

            WriteLineColor(true, scene.GetProperty("Body_Item_Header").GetString());

            string purchasedStatus = scene.GetProperty("PurchasedStatus").GetString();

            for (int i = 0; i < itemsInShop.Count; i++)
            {
                Item shopItem = itemsInShop[i];
                bool isPurchased = player.Inventory.Any(item => item.Name == shopItem.Name);
                if(isPurchased) continue;
                string prefix = $"{i + 1}. ";
                string attack = shopItem.AttackBonus > 0 ? $"공격력 +{shopItem.AttackBonus}" : "";
                string defense = shopItem.DefenseBonus > 0 ? $"방어력 +{shopItem.DefenseBonus}" : "";

                string statText = string.Join(" | ", new[] { attack, defense }.Where(s => !string.IsNullOrEmpty(s)));
                string priceText = isPurchased ? purchasedStatus : $"{shopItem.Price} G";

                WriteLineColor(true, $"{prefix}{shopItem.Name,-10} | {statText,-10} | {shopItem.Description,-30} | {priceText}");
            }
            Console.WriteLine();
            WriteLineColor(true, scene.GetProperty("Choices").EnumerateArray().Last().GetString());
            Console.WriteLine();
            WriteLineColor(true, TextManager.GetCommonText("Prompt"));
            WriteLineColor(false, TextManager.GetCommonText("InputPrefix"));
        }

        public void DisplayBuySuccess(string itemName)
        {
            var itemData = new Dictionary<string, object>
            {
                { "ItemName", itemName},
            };
            WriteLineColor(true,RenderTemplate(TextManager.GetScene("ShopScreen").GetProperty("BuySuccess").GetString(),itemData));
        }

        public void DisplaySellSuccess(string itemName)
        {
            var itemData = new Dictionary<string, object>
            {
                { "ItemName", itemName},
            };
            WriteLineColor(true,RenderTemplate(TextManager.GetScene("ShopScreen").GetProperty("SellSuccess").GetString(),itemData));
        }

        public void DisplayDungeonEntranceScreen(List<Dungeon> dungeons)
        {
            Console.Clear();
            var scene = TextManager.GetScene("DungeonEntranceScreen");

            WriteLineColor(true, scene.GetProperty("Title").GetString());
            WriteLineColor(true, scene.GetProperty("Description").GetString());
            Console.WriteLine();

            for (int i = 0; i < dungeons.Count; i++)
            {
                Dungeon dungeon = dungeons[i];
                string dungeonInfo = $"{i + 1}. {dungeon.Name} | 방어력 {dungeon.RequiredDefense} 이상 권장";
                WriteLineColor(true, dungeonInfo);
            }

            Console.WriteLine();

            WriteLineColor(true, scene.GetProperty("Choices").EnumerateArray().FirstOrDefault().GetString());

            Console.WriteLine();
            WriteLineColor(true, TextManager.GetCommonText("Prompt"));
            WriteLineColor(false, TextManager.GetCommonText("InputPrefix"));
        }

        public void DisplayDungeonClearScreen(Player originalPlayer, Player player, Dungeon dungeon, bool isSuccess)
        {
            Console.Clear();
            var scene = TextManager.GetScene("DungeonClearScreen");

            WriteLineColor(true, scene.GetProperty("Title").GetString());
            Console.WriteLine();

            var headerData = new Dictionary<string, object>
            {
                { "DungeonName", dungeon.Name }
            };
            if (isSuccess)
            {
                WriteLineColor(true, RenderTemplate(scene.GetProperty("Body_Header_Success").GetString(), headerData));
            }
            else
            {
                WriteLineColor(true, RenderTemplate(scene.GetProperty("Body_Header_Failed").GetString(), headerData));
            }

            Console.WriteLine();

            var resultData = new Dictionary<string, object>
            {
                { "Hp", originalPlayer.Hp },
                { "AfterHp", player.Hp },
                { "Gold", originalPlayer.Gold },
                { "AfterGold", player.Gold }
            };

            foreach (var lineTemplate in scene.GetProperty("Body_Lines").EnumerateArray())
            {
                WriteLineColor(true, RenderTemplate(lineTemplate.GetString(), resultData));
            }

            Console.WriteLine();

            WriteLineColor(true, scene.GetProperty("Choices").EnumerateArray().FirstOrDefault().GetString());

            Console.WriteLine();
            WriteLineColor(true, TextManager.GetCommonText("Prompt"));
            WriteLineColor(false, TextManager.GetCommonText("InputPrefix"));
        }

        public void DisplayInvalid()
        {
            WriteLineColor(true,TextManager.GetCommonText("InvalidInput"));
            Thread.Sleep(1000);
        }
        public void DisplaySave()
        {
            WriteLineColor(true,TextManager.GetCommonText("SaveSuccess"));
            Thread.Sleep(1000);
        }

        private string RenderTemplate(string template, Dictionary<string, object> data)
        {
            if (string.IsNullOrEmpty(template)) return "";

            return Regex.Replace(template, @"\{(.+?)\}", match =>
            {
                string keyWithFormat = match.Groups[1].Value;
                string[] parts = keyWithFormat.Split(':');
                string key = parts[0];

                if (data.TryGetValue(key, out object value))
                {
                    if (parts.Length > 1)
                    {
                        return string.Format($"{{0:{parts[1]}}}", value);
                    }
                    return value.ToString();
                }
                return match.Value;
            });
        }
    }
}