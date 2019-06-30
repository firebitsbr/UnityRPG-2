using System;
using UnityEngine;
public enum kBuff
{
    MaxHp, MaxMp, PhysicalAttack, MagicalAttack, PhysicalDefense, MagicalDefense, Speed,
    FireDefense, WaterDefense, WindDefense, EarthDefense, LightDefense, DarkDefense
}

[Serializable]
public class BaseBuff
{
    public kBuff Stat;
    public float Multiplier;

    [HideInInspector]
    public String SkillId;
}