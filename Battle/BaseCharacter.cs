using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Linq;
using System;

[CreateAssetMenu(fileName = "New Character", menuName = "RPG/Character")]
public class BaseCharacter : ScriptableObject
{
    public event Action OnCurrentHpChanged;
    public event Action OnCurrentMpChanged;
    public event Action OnStatusChanged;

    public string Name;
    public Sprite Icon;

    #region Stats
    public Stats BaseStats;

    public int MaxHp
    {
        get { return (int)Math.Round(LevelMultiplier(Level, FinalStats.Hp, 1.03f) * GetMultiplierForStat(kBuff.MaxHp)); }
    }

    private int currentHp;
    public int CurrentHp
    {
        get { return currentHp; }
        set
        {
            currentHp = Mathf.Clamp(value, 0, MaxHp);
            OnCurrentHpChanged?.Invoke();
        }
    }

    public int MaxMp
    {
        get { return (int)Math.Round(LevelMultiplier(Level, FinalStats.Mp, 1.02f) * GetMultiplierForStat(kBuff.MaxMp)); }
    }

    private int currentMp;
    public int CurrentMp
    {
        get { return currentMp; }
        set
        {
            currentMp = Mathf.Clamp(value, 0, MaxMp);
            OnCurrentMpChanged?.Invoke();
        }
    }

    public int PhysicalAttack
    {
        get { return (int)(LevelMultiplier(Level, FinalStats.PhysicalAttack, 1.02f) * GetMultiplierForStat(kBuff.PhysicalAttack)); }
    }

    public int MagicalAttack
    {
        get { return (int)(LevelMultiplier(Level, FinalStats.MagicalAttack, 1.02f) * GetMultiplierForStat(kBuff.MagicalAttack)); }
    }

    public int PhysicalDefense
    {
        get { return (int)(FinalStats.PhysicalDefense * GetMultiplierForStat(kBuff.PhysicalDefense)); }
    }

    public int MagicalDefense
    {
        get { return (int)(FinalStats.MagicalDefense * GetMultiplierForStat(kBuff.MagicalDefense)); }
    }

    public float Speed
    {
        get
        {
            var finalSpeed = LevelMultiplier(Level, FinalStats.Speed, 1.02f) * GetMultiplierForStat(kBuff.Speed);
            return Math.Max(finalSpeed, 1);
        }
    }

    public int FireDefense
    {
        get { return (int)(FinalStats.FireDefense * GetMultiplierForStat(kBuff.FireDefense)); }
    }

    public int WaterDefense
    {
        get { return (int)(FinalStats.WaterDefense * GetMultiplierForStat(kBuff.WaterDefense)); }
    }

    public int WindDefense
    {
        get { return (int)(FinalStats.WindDefense * GetMultiplierForStat(kBuff.WindDefense)); }
    }

    public int EarthDefense
    {
        get { return (int)(FinalStats.EarthDefense * GetMultiplierForStat(kBuff.EarthDefense)); }
    }

    public int LightDefense
    {
        get { return (int)(FinalStats.LightDefense * GetMultiplierForStat(kBuff.LightDefense)); }
    }

    public int DarkDefense
    {
        get { return (int)(FinalStats.DarkDefense * GetMultiplierForStat(kBuff.DarkDefense)); }
    }



    #endregion

    public int LevelExp { get; set; }

    public int Level { get; set; }

    public float LevelMultiplier(int level, float baseValue, float multiplier)
    {
        return level <= 1 ? baseValue : LevelMultiplier(level - 1, baseValue, multiplier) * multiplier;
    }

    public void AddExp(int exp)
    {
        if (Level < 100)
        {
            LevelExp += exp;
            int nextLevelExpRequired = (int)LevelMultiplier(Level, 100, 1.2f);

            while (LevelExp >= nextLevelExpRequired)
            {
                LevelExp -= nextLevelExpRequired;
                LevelUp();
                nextLevelExpRequired = (int)LevelMultiplier(Level, 100, 1.2f);
            }
        }
    }

    void LevelUp()
    {
        float hpPercentage = (float)CurrentHp / MaxHp;
        float mpPercentage = (float)CurrentMp / MaxMp;

        Level++;

        CurrentHp = (int)(MaxHp * hpPercentage);
        CurrentMp = (int)(MaxMp * mpPercentage);
        Debug.Log("Upou nunca vi");
    }

    public float CurrentDelay { get; set; }
    public float MaxDelay { get; set; }

    public List<BaseSkill> Skills;
    public List<BaseBuff> Buffs { get; private set; }
    public List<BaseStatus> Status { get; private set; }

    public Dictionary<kEquipmentSlot, Equipment> Equipments = new Dictionary<kEquipmentSlot, Equipment>();

    [HideInInspector]
    public Stats FinalStats;
    public CharacterResources Resources;
    [HideInInspector]
    public bool isAlly = false;

    [HideInInspector]
    public bool IsCasting = false;

    [SerializeField]
    private int exp;
    public int Exp
    {
        get
        {
            return (int)LevelMultiplier(Level, exp, 1.1f);
        }
    }
    public struct CastingInfo
    {
        public BaseSkill Skill;
        public List<BattleSlot> Targets;
    }
    public CastingInfo castingInfo;

    public void AddBuffs(BaseBuff newBuff)
    {
        Buffs.RemoveAll(e => e.SkillId == newBuff.SkillId && e.Stat == newBuff.Stat);
        Buffs.Add(newBuff);

        if (newBuff.Stat == kBuff.MaxHp)
            OnCurrentHpChanged?.Invoke();

        if (newBuff.Stat == kBuff.MaxMp)
            OnCurrentMpChanged?.Invoke();
    }
    public void AddStatus(BaseStatus newStatus)
    {
        Status.RemoveAll(e => e.SkillId == newStatus.SkillId && e.Id == newStatus.Id);

        var clone = (BaseStatus)newStatus.Clone();
        Status.Add(clone);

        OnStatusChanged?.Invoke();
    }

    public void Awake()
    {
        Buffs = new List<BaseBuff>();
        Status = new List<BaseStatus>();
        ResourcesController.Init(this);
        CurrentHp = MaxHp;
        CurrentMp = MaxMp;

        MaxDelay = CurrentDelay = 1.0f / Speed;
        IsCasting = false;
    }

    public void ResetValues()
    {
        Buffs.Clear();
        Status.Clear();
        MaxDelay = CurrentDelay = 1.0f / Speed;
        IsCasting = false;
    }

    float GetMultiplierForStat(kBuff stat)
    {
        float finalMultiplier = 1;

        var attackBuffs = Buffs.Where(e => e.Stat == stat);

        foreach (var buff in attackBuffs)
            finalMultiplier += buff.Multiplier;

        // Debug.Log(Name + " finalMultiplier " + stat + " / " + finalMultiplier);

        return finalMultiplier;
    }

    public bool IsAlive()
    {
        return CurrentHp > 0;
    }

    public void UpdateStatus()
    {
        foreach (var status in Status)
        {
            status.Turns--;
        }

        Status.RemoveAll(e => e.Turns <= 0);

        OnStatusChanged?.Invoke();
    }

    public bool HasStatus(kStatus id)
    {
        return Status.Any(e => e.Id == id);
    }

    public void StartCasting(BaseSkill skill, List<BattleSlot> targetSlots)
    {
        CurrentDelay = skill.CastTime;
        castingInfo.Skill = skill;
        castingInfo.Targets = targetSlots;
        IsCasting = true;
    }

    public float CalculateDefense(float totalDamage, kElements element)
    {
        float defenseDamage = totalDamage * (1 - (PhysicalDefense / 100f));
        float elementDamage = defenseDamage * (1 - (GetElementDefense(element) / 100f));
        return elementDamage;
    }

    public float GetElementDefense(kElements element)
    {
        if (element == kElements.Fire)
            return FireDefense;

        if (element == kElements.Ice)
            return WaterDefense;

        if (element == kElements.Wind)
            return WindDefense;

        if (element == kElements.Earth)
            return EarthDefense;

        if (element == kElements.Light)
            return LightDefense;

        if (element == kElements.Dark)
            return DarkDefense;

        return 0;
    }
}
