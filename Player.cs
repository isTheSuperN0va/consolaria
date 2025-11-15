using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

static public class Player
{
    static public string name = "";
    static public short x, y;
    static public int hp, xp, mana;

    static public double lvl;

    private const int HEALTH_MODIFIER = 5;

    static public int maxHp = (int)(HEALTH_MODIFIER*stats[(int)StatsNames.Vitality]*Math.Sqrt(lvl));

    static byte[] defence = {0, 0, 0, 0};

    static public byte[] stats = {0, 0, 0, 0, 0, 0, 0};


    static public void Damage(DamageTypes damageType, int damage)
    {
        int damageModified = (damage - defence[(int)damageType]) < 0 ? 0 : damage - defence[(int)damageType];
        hp -= damageModified;
    }

    static public void Heal(int amount, bool percentage)
    {
        if (!percentage) { hp = (hp + amount) > maxHp ? maxHp : hp + amount; }
        else { hp = hp *= amount > maxHp ? maxHp : hp *= amount; }
    }

    //Enums

    public enum StatsNames
    {
        Vitality,
        Strength,
        Endurance,
        Agility,
        Logic,
        Creativity,
        Social
    }

    public enum DamageTypes
    {
        Heat,
        Slash,
        Pierce,
        Blunt,
    }

}