public class EquipmentController
{
    public static void EquipItem(BaseCharacter character, Equipment newEquip)
    {
        if (character.Equipments.ContainsKey(newEquip.Slot))
        {
            Equipment oldEquip = character.Equipments[newEquip.Slot];
            character.Equipments[newEquip.Slot] = newEquip;
            InventoryController.RemoveItem(newEquip);

            if (oldEquip != null)
            {
                InventoryController.AddItem(oldEquip);
            }
        }
        else
        {
            character.Equipments.Add(newEquip.Slot, newEquip);
            InventoryController.RemoveItem(newEquip);
        }

        StatsController.CalculateFinalStats(character);
    }
}