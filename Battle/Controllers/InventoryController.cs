using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InventoryController
{
    private static List<Item> Inventory = new List<Item>();
    public static event Action<Item> OnItemAdded;
    public static event Action<Item> OnItemRemoved;

    public static void AddItem(Item item)
    {
        Inventory.Add(item);
        OnItemAdded?.Invoke(item);
    }

    public static void RemoveItem(Item item)
    {
        Inventory.Remove(item);
        OnItemRemoved?.Invoke(item);
    }

    public static List<Item> GetItems()
    {
        return Inventory;
    }

}