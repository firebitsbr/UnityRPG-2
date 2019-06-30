using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class BattleTargetController
{
    public static void EnableTargetableCharacterSlots(BattleModel battleModel)
    {
        foreach (var slot in battleModel.AllSlots)
            slot.SetTargetable(false);

        if (battleModel.CurrentSkill.Target == kTarget.Self)
        {
            battleModel.CurrentSlot.SetTargetable(true);
        }
        else if (battleModel.CurrentSkill.Target == kTarget.DeadAlly)
        {
            var slots = battleModel.AlliesSlots.Where(e => !e.Character.IsAlive());
            foreach (var slot in slots)
                slot.SetTargetable(true);
        }
        else if (battleModel.CurrentSkill.Target == kTarget.SingleAlly || battleModel.CurrentSkill.Target == kTarget.AllAllies)
        {
            var slots = battleModel.AlliesSlots.Where(e => e.Character.IsAlive());
            foreach (var slot in slots)
                slot.SetTargetable(true);
        }
        else if (battleModel.CurrentSkill.Target == kTarget.AllEnemies)
        {
            var slots = battleModel.EnemiesSlots.Where(e => e.Character.IsAlive());
            foreach (var slot in slots)
                slot.SetTargetable(true);
        }
        else if (battleModel.CurrentSkill.Target == kTarget.SingleEnemy || battleModel.CurrentSkill.Target == kTarget.AllEnemies)
        {
            var enemyFrontLinersAlive = battleModel.EnemiesSlots.Any(e => e.IsFrontLine && e.Character.IsAlive());
            var slots = battleModel.EnemiesSlots.Where(e => e.Character.IsAlive());

            foreach (var slot in slots)
            {
                if (!battleModel.CurrentSkill.IsRanged && !slot.IsFrontLine && enemyFrontLinersAlive)
                    continue;

                slot.SetTargetable(true);
            }
        }
    }
}