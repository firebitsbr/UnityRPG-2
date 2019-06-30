public class StatsController
{
    public static void CalculateFinalStats(BaseCharacter character)
    {
        Stats finalStats = Sum(new Stats(), character.BaseStats);

        foreach (Equipment equip in character.Equipments.Values)
        {
            finalStats = Sum(finalStats, equip.Stats);
        }

        character.FinalStats = finalStats;
    }

    public static Stats Sum(Stats s1, Stats s2)
    {
        Stats sum = new Stats();
        sum.Hp = s1.Hp + s2.Hp;
        sum.Mp = s1.Mp + s2.Mp;
        sum.PhysicalAttack = s1.PhysicalAttack + s2.PhysicalAttack;
        sum.PhysicalDefense = s1.PhysicalDefense + s2.PhysicalDefense;
        sum.MagicalAttack = s1.MagicalAttack + s2.MagicalAttack;
        sum.MagicalDefense = s1.MagicalDefense + s2.MagicalDefense;
        sum.Speed = s1.Speed + s2.Speed;
        sum.FireDefense = s1.FireDefense + s2.FireDefense;
        sum.WaterDefense = s1.WaterDefense + s2.WaterDefense;
        sum.WindDefense = s1.WindDefense + s2.WindDefense;
        sum.EarthDefense = s1.EarthDefense + s2.EarthDefense;
        sum.LightDefense = s1.LightDefense + s2.LightDefense;
        sum.DarkDefense = s1.DarkDefense + s2.DarkDefense;

        return sum;
    }

}