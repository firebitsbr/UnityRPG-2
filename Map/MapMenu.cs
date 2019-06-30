using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MapMenu : MonoBehaviour
{
    public GameObject Menu;
    public GameObject Inventory;

    void Start()
    {
        LoadInventory();
        InventoryController.OnItemAdded += AddItem;
    }

    void Update()
    {
        if (Input.GetKeyDown("i"))
        {
            Menu.gameObject.SetActive(!Menu.gameObject.active);
        }
    }

    void LoadInventory()
    {
        foreach (Equipment equip in InventoryController.GetItems())
        {
            AddItem(equip);
        }
    }

    void AddItem(Item item)
    {
        GameObject go = Resources.Load<GameObject>("Prefabs/InventorySlot");
        go.GetComponent<Image>().sprite = item.Icon;
        Instantiate(go, Vector3.zero, Quaternion.identity, Inventory.transform);
    }
}