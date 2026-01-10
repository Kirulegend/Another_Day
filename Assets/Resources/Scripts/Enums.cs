using System;
using UnityEngine;
namespace ITISKIRU
{
    public enum ItemType { Vegi, Frozen, Dairy, Sause, Cover }
    public enum ItemName { Egg, Rice, Noodles, Pearls, Sugarcane, Carrot, Batter }

    [Flags]
    public enum InteractionType
    {
        None = 0,
        Discard = 1 << 0,
        Pick = 1 << 1,
        Take = 1 << 2,
        Open = 1 << 3,
        Close = 1 << 4,
        Rotate = 1 << 5,
        End = 1 << 6,
        Place = 1 << 7,
        Use = 1 << 8,
        Back = 1 << 9,
        Putin = 1 << 10,
        Edit = 1 << 11,
        Boil = 1 << 12,
        Cook = 1 << 13,
        Fry = 1 << 14
    }

    [Serializable]
    public class ItemInfo
    {
        public string _name;
        public Sprite _icon;
        public ItemType _type;
        public Sprite _typeIcon;
        public int _price;
        public int itemQuantity;
    }
}