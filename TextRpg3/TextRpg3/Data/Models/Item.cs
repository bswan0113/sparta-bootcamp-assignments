using System.Text.Json.Serialization;

namespace TextRpg3.Data.Models
{
    public enum ItemType
    {
        Weapon, Armor,Undefined
    }
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int AttackBonus { get; set; }
        public int DefenseBonus { get; set; }

        public ItemType ItemType { get; set; }

        public int Price { get; set; }

        [JsonIgnore]
        public bool IsEquipped { get; set; }

        public Item(string name, string description, int attackBonus, int defenseBonus, int price, ItemType itemType)
        {
            Name = name;
            Description = description;
            AttackBonus = attackBonus;
            DefenseBonus = defenseBonus;
            ItemType = itemType;
            Price = price;
            IsEquipped = false;
        }
    }
}