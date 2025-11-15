using Microsoft.Win32.SafeHandles;

public class HealItem : Item
{
    private short healAmount;
    private bool percentage;

    public HealItem(string Name, short Quantity, short HealAmount, bool Percentage)
    {
        name = Name;
        quantity = Quantity;
        healAmount = HealAmount;
        percentage = Percentage;
    }

    public void OnUse()
    {
        //Player.Heal(healAmount, percentage);
    }
}