using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum kButtonSize
{
    Large, Medium, Small
}

[Serializable]
public class SkillFrame
{
    public float Time;
    public string Button;
    public kButtonSize Size;
    public float Multiplier;
}

public enum kElements
{
    None, Fire, Ice, Wind, Earth, Light, Dark
}

public enum kCostType
{
    Free, FlatMp, FlatHp, PercentageMp, PercentageHp
}

public enum kTarget
{
    SingleEnemy, AllEnemies, SingleAlly, AllAllies, Self, DeadAlly
}

[CreateAssetMenu(fileName = "New Skill", menuName = "RPG/Skill")]
public class BaseSkill : ScriptableObject
{
    public string Name;
    [TextArea(5, 10)]
    public string Description;
    public Sprite Icon;
    public kTarget Target;
    public kElements Element;
    public kCostType CostType;
    public float CostValue;
    //public bool IsAoe;
    public bool IsRanged;
    public float CastTime;
    public List<SkillFrame> Frames;
    public List<BaseBuff> Buffs;
    public List<BaseStatus> Status;
    public List<BaseEffect> Effects;

    private void OnEnable()
    {
        foreach (var buff in Buffs)
        {
            buff.SkillId = Name;
        }

        foreach (var status in Status)
        {
            status.SkillId = Name;
        }
    }

    public float CalculateTotalHp(BaseCharacter caster, BaseCharacter target)
    {
        float totalDamage = 0;

        var consume = Effects.Where(e => e.Type == kEffectType.ConsumeHp);
        foreach (var effect in consume)
            totalDamage += effect.CalculateEffect(caster, target);

        var restore = Effects.Where(e => e.Type == kEffectType.RestoreHp);
        foreach (var effect in restore)
            totalDamage -= effect.CalculateEffect(caster, target);

        return totalDamage;
    }

    public float CalculateTotalMp(BaseCharacter caster, BaseCharacter target)
    {
        float totalDamage = 0;

        var consume = Effects.Where(e => e.Type == kEffectType.ConsumeMp);
        foreach (var effect in consume)
            totalDamage += effect.CalculateEffect(caster, target);

        var restore = Effects.Where(e => e.Type == kEffectType.RestoreMp);
        foreach (var effect in restore)
            totalDamage -= effect.CalculateEffect(caster, target);

        return totalDamage;
    }

    public bool HasCostRequired(BaseCharacter caster)
    {
        if (CostType == kCostType.Free)
            return true;

        if (CostType == kCostType.FlatMp)
            return caster.CurrentMp >= CostValue;

        if (CostType == kCostType.FlatHp)
            return caster.CurrentHp >= CostValue;

        if (CostType == kCostType.PercentageMp)
            return caster.CurrentMp >= CostValue * caster.MaxMp;

        if (CostType == kCostType.PercentageHp)
            return caster.CurrentHp >= CostValue * caster.MaxHp;

        return false;
    }

    public void TakeCost(BaseCharacter caster)
    {
        if (CostType == kCostType.FlatMp)
        {
            caster.CurrentMp -= (int)CostValue;
        }

        if (CostType == kCostType.FlatHp)
            caster.CurrentHp -= (int)CostValue;

        if (CostType == kCostType.PercentageMp)
            caster.CurrentMp -= (int)(CostValue * caster.MaxMp);

        if (CostType == kCostType.PercentageHp)
            caster.CurrentHp -= (int)(CostValue * caster.MaxHp);
    }
}