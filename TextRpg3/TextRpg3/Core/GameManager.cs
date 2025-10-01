using TextRpg3.Data;
using TextRpg3.Data.Models;
using TextRpg3.UI;

namespace TextRpg3.Core
{
    public class GameManager
    {
        private GameState _currentState;
        private readonly UIManager _uiManager;
        private readonly ActionManager _actionManager;
        private bool _isRunning = true;
        private Player _player;

        private const int RestCost= 500;
        private const int RestRecoveryStamina = 20;
        private const int RestRecoveryHp = 100;

        public List<Item> ItemsInShop { get; private set; }
        public List<Dungeon> Dungeons { get; private set; }
        public GameManager()
        {
            DataManager.Init();
            TextManager.Init();
            _player = DataManager.Player;
            _uiManager = new UIManager();
            ItemsInShop = DataManager.ItemsInShop;
            Dungeons = DataManager.Dungeons;
            _actionManager = new ActionManager(_uiManager, _player);
            _currentState = GameState.MainMenu;
        }

        public void Run()
        {

            while (_isRunning)
            {
                switch (_currentState)
                {
                    case GameState.MainMenu:
                        _uiManager.DisplayMainMenu();
                        MainMenu();
                        break;
                    case GameState.StatusScreen:
                        _uiManager.DisplayStatusScreen(_player);
                        StatusScreen();
                        break;
                    case GameState.InventoryScreen:
                        _uiManager.DisplayInventoryScreen(_player.Inventory);
                        InventoryScreen();
                        break;
                    case GameState.EquipScreen:
                        _uiManager.DisplayEquipScreen(_player.Inventory);
                        EquipScreen();
                        break;
                    case GameState.DungeonEntranceScreen:
                        _uiManager.DisplayDungeonEntranceScreen(Dungeons);
                        DungeonEntranceScreen();
                        break;
                    case GameState.ShopScreen:
                        _uiManager.DisplayShopScreen(_player, ItemsInShop);
                        ShopScreen();
                        break;
                    case GameState.ShopInBuyScreen:
                        _uiManager.DisplayShopInBuyScreen(_player, ItemsInShop);
                        ShopInBuyScreen();
                        break;
                    case GameState.ShopInSellScreen:
                        _uiManager.DisplayShopInSellScreen(_player, ItemsInShop);
                        ShopInSellScreen();
                        break;
                    case GameState.RestScreen:
                        _uiManager.DisplayRestScreen(_player);
                        RestScreen();
                        break;
                }
            }
        }

        private void MainMenu()
        {
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    _currentState = GameState.StatusScreen;
                    break;
                case "2":
                    _currentState = GameState.InventoryScreen;
                    break;
                case "3":
                    _actionManager.ActionSequence(Action.Adventure);
                    break;
                case "4":
                    _actionManager.ActionSequence(Action.Patrol);
                    break;
                case "5":
                    _actionManager.ActionSequence(Action.Training);
                    break;
                case "6":
                    _currentState = GameState.ShopScreen;
                    break;
                case "7":
                    _currentState = GameState.DungeonEntranceScreen;
                    break;
                case "8":
                    _currentState = GameState.RestScreen;
                    break;
                case "9":
                    DataManager.SaveGame(_player);
                    _uiManager.DisplaySave();
                    break;

                case "0" : _isRunning = false;
                    break;
                default:
                    _uiManager.DisplayInvalid();
                    break;
            }
        }

        private void StatusScreen()
        {
            string input = Console.ReadLine();
            switch (input)
            {
                case "0":
                    _currentState = GameState.MainMenu;
                    break;
                default:
                    _uiManager.DisplayInvalid();
                    break;
            }
        }

        private void InventoryScreen()
        {
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    _currentState = GameState.EquipScreen;
                    break;
                case "0":
                    _currentState = GameState.MainMenu;
                    break;
                default:
                    _uiManager.DisplayInvalid();
                    break;
            }
        }

        private void EquipScreen()
        {
            string input = Console.ReadLine();
            if (int.TryParse(input, out int choice))
            {
                switch (choice)
                {
                    case 0:
                        _currentState = GameState.InventoryScreen;
                        break;
                    default:
                        if (choice > 0 && choice <= _player.Inventory.Count)
                        {
                            var item = _player.Inventory[choice - 1];
                            _player.ToggleEquipItem(item);
                        }
                        else
                        {
                            _uiManager.DisplayInvalid();
                        }
                        break;
                }
            }
            else
            {
                _uiManager.DisplayInvalid();
            }
        }

        private void DungeonEntranceScreen()
        {
            string input = Console.ReadLine();
            if (int.TryParse(input, out int choice))
            {
                if (choice == 0)
                {
                    _currentState = GameState.MainMenu;
                }else if (choice > 0 && choice <= Dungeons.Count)
                {
                    _currentState = GameState.DungeonClearScreen;
                    bool isSuccess = false;
                    Player originalPlayer = new Player();
                    originalPlayer.Hp = _player.Hp;
                    originalPlayer.Gold = _player.Gold;
                    originalPlayer.Exp = _player.Exp;
                    Player playerAfterClear = DungeonClearSequence(Dungeons[choice - 1], out isSuccess);
                    _uiManager.DisplayDungeonClearScreen(originalPlayer, playerAfterClear, Dungeons[choice - 1], isSuccess);
                    string input2 = Console.ReadLine();
                    if(input2 == "0") _currentState = GameState.DungeonEntranceScreen;
                }
                else
                {
                    _uiManager.DisplayInvalid();
                }
            }
        }

        private Player DungeonClearSequence(Dungeon dungeon, out bool isSuccess)
        {

            int totalDefense = _player.GetTotalDefense();
            int totalAttack = (int)_player.GetTotalAttack();
            float additionalRewardRate = (new Random().Next(totalAttack, totalAttack * 2)+100);

            if (totalDefense < dungeon.RequiredDefense)
            {
                int isClearRate = new Random().Next(0, 100);
                if (isClearRate < 40)
                {
                    isSuccess = true;
                    int hpReduce = new Random().Next(20, 36);
                    _player.Hp -= hpReduce;

                    _player.AddExp((int)(dungeon.RewardExp * (additionalRewardRate / 100)));
                    _player.AddGold((int)(dungeon.RewardGold* (additionalRewardRate / 100)));
                }
                else
                {
                    isSuccess = false;
                    _player.Hp = (int)Math.Ceiling((double)_player.Hp / 2);
                }
            }
            else
            {
                isSuccess = true;
                int defenseDifference = totalDefense - dungeon.RequiredDefense;

                int minHpReduce = Math.Max(1, 20 - defenseDifference);
                int maxHpReduce = Math.Max(minHpReduce + 1, 36 - defenseDifference);

                int hpReduce = new Random().Next(minHpReduce, maxHpReduce);
                _player.Hp -= hpReduce;
                _player.AddExp((int)(dungeon.RewardExp * (additionalRewardRate / 100)));
                _player.AddGold((int)(dungeon.RewardGold * (additionalRewardRate / 100)));
            }
            return _player;
        }

        private void RestScreen()
        {
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    if (_player.Gold >= RestCost)
                    {
                        _player.Gold -= RestCost;
                        _player.Stamina += RestRecoveryStamina;
                        _player.Hp += RestRecoveryHp;
                        _uiManager.DisplayRestResult();
                    }
                    else
                    {
                        _uiManager.DisplayInsufficientGold();
                    }
                    break;
                case "0": _currentState = GameState.MainMenu;
                    break;
                default:
                    _uiManager.DisplayInvalid();
                    break;
            }
        }

        private void ShopScreen()
        {
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    _currentState = GameState.ShopInBuyScreen;
                    break;
                case "2":
                    _currentState = GameState.ShopInSellScreen;
                    break;
                case "0":
                    _currentState = GameState.MainMenu;
                    break;
                default:
                    _uiManager.DisplayInvalid();
                    break;
            }
        }

        private void ShopInBuyScreen()
        {
            string input = Console.ReadLine();
            if (int.TryParse(input, out int choice))
            {
                if (choice == 0)
                {
                    _currentState = GameState.ShopScreen;
                }
                else if (choice > 0 && choice <= ItemsInShop.Count)
                {
                    Item itemToBuy = ItemsInShop[choice - 1];

                    bool isAlreadyPurchased = _player.Inventory.Any(item => item.Name == itemToBuy.Name);

                    if (isAlreadyPurchased)
                    {
                        _uiManager.DisplayInvalid();
                    }
                    else if (_player.Gold >= itemToBuy.Price)
                    {
                        _player.Gold -= itemToBuy.Price;
                        _player.Inventory.Add(DataManager.CreateItemInstance(itemToBuy));
                        _uiManager.DisplayBuySuccess(itemToBuy.Name);
                    }
                    else
                    {
                        _uiManager.DisplayInsufficientGold();
                    }
                }
                else
                {
                    _uiManager.DisplayInvalid();
                }
            }
            else
            {
                _uiManager.DisplayInvalid();
            }
        }

        private void ShopInSellScreen()
        {
            string input = Console.ReadLine();
            if (int.TryParse(input, out int choice))
            {
                if (choice == 0)
                {
                    _currentState = GameState.ShopScreen;
                }
                else if (choice > 0 && choice <= _player.Inventory.Count)
                {
                    Item itemToSell = _player.Inventory[choice - 1];
                    float sellPrice = itemToSell.Price * 0.85f;
                    _player.Gold += sellPrice;
                    _player.Inventory.Remove(itemToSell);
                    _uiManager.DisplaySellSuccess(itemToSell.Name);
                }
                else
                {
                    _uiManager.DisplayInvalid();
                }
            }
            else
            {
                _uiManager.DisplayInvalid();
            }
        }
    }
}