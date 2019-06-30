using System;
using UnityEngine;

public enum kEffect
{
    CasterPhysicalAttack, TargetPhysicalAttack,
    CasterMagicalAttack, TargetMagicalAttack,
    CasterMaxHp, TargetMaxHp,
    CasterRemainingHp, TargetRemainingHp,
    CasterMissingHp, TargetMissingHp,
    CasterMaxMp, TargetMaxMp,
    CasterRemainingMp, TargetRemainingMp,
    CasterMissingMp, TargetMissingMp
}

public enum kEffectType
{
    ConsumeHp, RestoreHp, ConsumeMp, RestoreMp
}

[Serializable]
public class BaseEffect
{
    public kEffect Id;
    public kEffectType Type;
    public float Multiplier;

    public float CalculateEffect(BaseCharacter caster, BaseCharacter target)
    {
        // Attack
        if (kEffect.CasterPhysicalAttack == Id)
            return caster.PhysicalAttack * Multiplier;

        if (kEffect.TargetPhysicalAttack == Id)
            return target.PhysicalAttack * Multiplier;

        if (kEffect.CasterMagicalAttack == Id)
            return caster.MagicalAttack * Multiplier;

        if (kEffect.TargetMagicalAttack == Id)
            return target.MagicalAttack * Multiplier;

        // Hp
        if (kEffect.CasterMaxHp == Id)
            return caster.MaxHp * Multiplier;

        if (kEffect.TargetMaxHp == Id)
            return target.MaxHp * Multiplier;

        if (kEffect.CasterRemainingHp == Id)
            return caster.CurrentHp * Multiplier;

        if (kEffect.TargetRemainingHp == Id)
            return target.CurrentHp * Multiplier;

        if (kEffect.CasterMissingHp == Id)
            return (caster.MaxHp - caster.CurrentHp) * Multiplier;

        if (kEffect.TargetMissingHp == Id)
            return (target.MaxHp - target.CurrentHp) * Multiplier;

        // Mp
        if (kEffect.CasterMaxMp == Id)
            return caster.MaxMp * Multiplier;

        if (kEffect.TargetMaxMp == Id)
            return target.MaxMp * Multiplier;

        if (kEffect.CasterRemainingMp == Id)
            return caster.CurrentMp * Multiplier;

        if (kEffect.TargetRemainingMp == Id)
            return target.CurrentMp * Multiplier;

        if (kEffect.CasterMissingMp == Id)
            return (caster.MaxMp - caster.CurrentMp) * Multiplier;

        if (kEffect.TargetMissingMp == Id)
            return (target.MaxMp - target.CurrentMp) * Multiplier;

        Debug.LogError("ERROR EFFECT NOT FOUND");

        return caster.PhysicalAttack * Multiplier;
    }
}