using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
[CreateAssetMenu(fileName = "Create Inventory Item", menuName = "Inventory/CreateItem", order = 1)]
public class Item : ScriptableObject
{
    public Sprite icon;
    public string name = "Default Item";
    [TextArea]
    public string description = "Item Description";
    public RarityType RarityType;
    public InventoryItemType inventoryItemType;
    public ScriptableObject itemObject;
    

}

public enum InventoryItemType
{
    Weapon,
    Barter,
    Armour,
    Meds
}
