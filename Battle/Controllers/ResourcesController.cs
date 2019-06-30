public class ResourcesController
{
    public static void Init(BaseCharacter c)
    {
        StatsController.CalculateFinalStats(c);
        c.Resources = new CharacterResources();
        c.Resources.CurrentHp = c.FinalStats.Hp;
        c.Resources.CurrentMp = c.FinalStats.Mp;
    }

    public static bool IsAlive(BaseCharacter character)
    {
        return character.Resources.CurrentHp > 0;
    }
}