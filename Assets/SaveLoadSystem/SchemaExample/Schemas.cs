using SaveLoadSystem;
using SaveLoadSystem.Core;
using System.Collections.Generic;

public class PlayerData : Schema
{

    public string name;
    public float hp;
    public float mp;
    public InventoryData inventory;

    public PlayerData() { }
    public PlayerData(SaveableData saveableData) : base(saveableData) { }

    public override string ToString()
    {
        return $"PlayerData: {{ Name: {name}, HP: {hp}, MP: {mp}, Inventory: {inventory} }}";
    }
}




public class InventoryData : Schema
{
    public int capacity;
    public List<ItemData> items;
    public InventoryData() { }
    public InventoryData(SaveableData saveableData) : base(saveableData) { }

    public override string ToString()
    {
        string itemList = items != null ? string.Join(", ", items) : "None";
        return $"InventoryData: {{ Capacity: {capacity}, Items: [{itemList}] }}";
    }
}

public class ItemData : Schema
{

    public string name;
    public float damage;
    public ItemData(string name, float damage) 
    { 
        this.name = name;
        this.damage = damage;
    }
    public ItemData(SaveableData saveableData) : base(saveableData) { }



    public override string ToString()
    {
        return $"ItemData: {{ Name: {name}, Damage: {damage} }}";
    }
}


