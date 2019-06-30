using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class EnemyController
{
    public static List<BattleSlot> GetTargetsForEnemySkill(BattleModel battleModel)
    {
        var target = battleModel.CurrentSkill.Target;

        if (kTarget.AllEnemies == target)
            return battleModel.AlliesSlots;

        if (kTarget.SingleEnemy == target)
            return battleModel.AlliesSlots.Where(i => i.Character.IsAlive())
            .OrderBy(n => UnityEngine.Random.value).ToList();
        // TODO: Buffs entre inimigos n deveriam ativar a frameBar
        if (kTarget.AllAllies == target)
            return battleModel.EnemiesSlots;

        if (kTarget.SingleAlly == target)
            return battleModel.EnemiesSlots.Where(i => i.Character.IsAlive())
            .OrderBy(n => UnityEngine.Random.value).ToList();

        if (kTarget.Self == target)
            return battleModel.EnemiesSlots.Where(i => i.Character == battleModel.CurrentChar).ToList();

        throw new Exception("No target found for skill: " + battleModel.CurrentSkill.Name);
    }
}