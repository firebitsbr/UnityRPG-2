using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Equipment", menuName = "RPG/Equipment")]
public class Equipment : Item
{
    [SerializeField]
    public kEquipmentSlot Slot;

    [SerializeField]
    public Stats Stats;
}