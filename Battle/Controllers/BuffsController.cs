using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class BuffsController
{
    public static void ApplyBuffs(List<BattleSlot> targets, List<BaseBuff> buffs)
    {
        foreach (var buff in buffs)
        {
            foreach (var slot in targets)
            {
                slot.Character.AddBuffs(buff);
            }
        }
    }
}