using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleModel
{
    public BaseCharacter CurrentChar;
    public BattleSlot CurrentSlot;
    public BaseSkill CurrentSkill;

    public List<BaseCharacter> AllCharacters = new List<BaseCharacter>();
    public List<BattleSlot> AllSlots = new List<BattleSlot>();
    public List<BattleSlot> AlliesSlots = new List<BattleSlot>();
    public List<BattleSlot> EnemiesSlots = new List<BattleSlot>();
    public List<BattleSlot> TargetSlots = new List<BattleSlot>();
}