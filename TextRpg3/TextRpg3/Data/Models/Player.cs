namespace TextRpg3.Data.Models
{
    public class Player
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public string Job { get; set; }
        public float Attack { get; set; }
        public int Defense { get; set; }
        private int _hp;
        public int Hp
        {
            get { return _hp; }
            set
            {
                if (value < 0)
                {
                    _hp = 0;
                }
                else if (value > MaxHp)
                {
                    _hp = MaxHp;
                }
                else
                {
                    _hp = value;
                }
            }
        }
        public float Gold { get; set; }
        private int _exp;
        public int Exp
        {
            get { return _exp; }
            set
            {
                _exp = value;
                LevelUp();
            }
        }
        private int _stamina;
        private const int MaxStamina = 9999;
        private const int MaxHp = 9999;

        public int Stamina
        {
            get { return _stamina; }
            set
            {
                if (value < 0)
                {
                    _stamina = 0;
                }
                else if (value > MaxStamina)
                {
                    _stamina = MaxStamina;
                }
                else
                {
                    _stamina = value;
                }
            }
        }
        public List<Item> Inventory { get; set; }

        public Player()
        {
            Inventory = new List<Item>();
        }

        public float GetTotalAttack()
        {
            float totalAttack = Attack;
            foreach (var item in Inventory)
            {
                if (item.IsEquipped)
                {
                    totalAttack += item.AttackBonus;
                }
            }
            return totalAttack;
        }

        public int GetTotalDefense()
        {
            int totalDefense = Defense;
            foreach (var item in Inventory)
            {
                if (item.IsEquipped)
                {
                    totalDefense += item.DefenseBonus;
                }
            }
            return totalDefense;
        }

        public void AddGold(int amount)
        {
            Gold += amount;
        }
        public void AddExp(int amount)
        {
            Exp += amount;
        }

        public void AddHp(int amount)
        {
            Hp += amount;
        }
        public void ToggleEquipItem(Item itemToToggle)
        {
            if (itemToToggle.IsEquipped)
            {
                itemToToggle.IsEquipped = false;
            }
            else
            {
                Item existingEquippedItem = Inventory.FirstOrDefault(i => i.IsEquipped && i.ItemType == itemToToggle.ItemType);

                if (existingEquippedItem != null)
                {
                    existingEquippedItem.IsEquipped = false;
                }
                itemToToggle.IsEquipped = true;
            }
        }

        private void LevelUp()
        {
            bool levelUp = false;
            if (Level < 1)
            {
                if (_exp >= 50)
                {
                    Exp -= 50;
                    Level++;
                    levelUp = true;
                }
            }else if (Level < 2)
            {
                if (_exp >= 80)
                {
                    Exp -= 80;
                    Level++;
                    levelUp = true;
                }
            }else if (Level < 3)
            {
                if (_exp >= 150)
                {
                    _exp -= 150;
                    Level++;
                    levelUp = true;
                }
            }else if (Level < 4)
            {
                if (_exp >= 500)
                {
                    _exp -= 500;
                    Level++;
                    levelUp = true;
                }
            }
            if(!levelUp)return;
            Attack += 0.5f;
            Defense += 1;
        }
    }
}