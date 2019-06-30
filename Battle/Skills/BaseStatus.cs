using System;
using UnityEngine;
public enum kStatus
{
    Dot, Stun, Invunerability, Double, Reraise, Doom, HealBlock, Regen, Lifesteal, Reflect, Curse
}

[Serializable]
public class BaseStatus : ICloneable
{
    public kStatus Id;
    public int Turns;
    [HideInInspector]
    public String SkillId;

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}