using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIManager : MonoBehaviour
{
    private Item _item;
    public Image _image;
    
    private void OnEnable()
    {
        if (_image == null)
            _image = GetComponentInChildren<Image>();

        if (_item != null)
        {
            _image.sprite = _item.icon;
            _image.color = Color.white;
        }
        else
            _image.color = Color.clear;

    }


    public void SetItem(Item item)
    {
        _item = item;

        _image.color = Color.white;
        
        //_image.gameObject.SetActive(true);
        if (gameObject.activeInHierarchy)
        {
            _image.sprite = _item.icon;
        }

    }

    public void RemoveItem()
    {
        _item = null;
        _image.sprite = null;
        //_image.gameObject.SetActive(false);

        
    }
}
