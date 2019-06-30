using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class StatusController
{
    public static void ApplyStatus(List<BattleSlot> targets, List<BaseStatus> status)
    {
        foreach (var value in status)
        {
            foreach (var slot in targets)
            {
                slot.Character.AddStatus(value);
            }
        }
    }
}