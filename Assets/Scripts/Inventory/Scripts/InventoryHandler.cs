using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class InventoryHandler : MonoBehaviour
{
    public List<Item> items = new List<Item>();

    public GameObject inventory;
    public GameObject inGameInventory;

    private InventoryUIManager[] inGameUIManager;
    private InventoryUIManager[] inventoryUIManager;
    private bool _inventoryIsToggled = false;
    private GameObject[] inventoryObjects;
    private PlayerMovement _playerController;
    private int currentItemSelected = 0;

    private KeyCode[] hotbarKeys = new[]
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, 
        KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9,KeyCode.Alpha0,
    };
    void Start()
    {
        _playerController = GetComponent<PlayerMovement>();
        inGameUIManager = inGameInventory.GetComponentsInChildren<InventoryUIManager>();
        inventoryUIManager = inventory.GetComponentsInChildren<InventoryUIManager>();
        UpdateInventoryUI();
        
        //Set Default Item
        if (items.Count > 0)
        {
            UseItem(items[0]);
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleInventoryUI();
        }


        
        for (int i = 0; i < hotbarKeys.Length; i++)
        {
            if (Input.GetKeyDown(hotbarKeys[i]))
            {
                if (i != currentItemSelected)
                {
                    UseItem(items[i]);
                    currentItemSelected = i;
                }
            }
        }
    }




    public void UseItem(Item item)
    {
        switch (item.inventoryItemType)
        {
            case InventoryItemType.Weapon:
                gameObject.GetComponent<GunPositionController>().UpdateWeapon((Weapon)item.itemObject);
                break;
        }
    }

    void ToggleInventoryUI()
    {
        _inventoryIsToggled = !_inventoryIsToggled;
        _playerController.isInMenu = _inventoryIsToggled;

        
        inventory.SetActive(_inventoryIsToggled);
        inGameInventory.SetActive(!_inventoryIsToggled);
        
    }

    private void UpdateInventoryUI()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (i < inGameUIManager.Length)
                inGameUIManager[i].SetItem(items[i]);
            
            inventoryUIManager[i].SetItem(items[i]);
        }
    }
    


    // /// <summary>
    // /// Swaps weapon A with B
    // /// </summary>
    // /// <param name="index">A</param>
    // /// <param name="swapIndex">B</param>
    // public void SwapWeapon(int index, int swapIndex)
    // {
    //     var a = weaponInventory[index];
    //     var b = weaponInventory[swapIndex];
    //
    //     weaponInventory.Remove(index);
    //     weaponInventory.Remove(swapIndex);
    //     
    //     weaponInventory.Add(swapIndex, a);
    //     weaponInventory.Add(index, b);
    // }
    //
    //
    // /// <summary>
    // /// Moves weapon at startIndex to endIndex. If endIndex exists, it will swap the positions
    // /// </summary>
    // /// <param name="startIndex">Weapon to move</param>
    // /// <param name="endIndex">Index to move weapon to</param>
    // public void MoveWeapon(int startIndex, int endIndex)
    // {
    //     //If our end index exists, we swap the weapon
    //     if (weaponInventory.ContainsKey(endIndex))
    //     {
    //         SwapWeapon(startIndex, endIndex);
    //     }
    //     
    //     var a = weaponInventory[startIndex];
    //     
    //     weaponInventory.Remove(startIndex);
    //     weaponInventory.Add(endIndex, a);
    // }
}
