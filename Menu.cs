

using System;

public abstract class MenuBase
{
    public enum MenuTypes
        {
            Button,
            Selector,
            Keybind
        }   

    public string label = "";
    public Action? action;

    public MenuTypes menuType;

    public bool isSelected = false;
    public abstract void Draw(bool isSelected);

}

public class MenuButton : MenuBase
{
    public MenuButton(string label, Action? action)
    {
        this.menuType = MenuTypes.Button;
        this.label = label;
        this.action = action;
    }

    override public void Draw(bool isSelected)
    {
        Console.BackgroundColor = isSelected ? Color.selectedColor : Color.backgroundColor;
        Console.WriteLine(label);
    }
}

public class MenuSelector : MenuBase
{
    public byte index = 0;
    string[] selector;
    public MenuSelector(string label, Action? action, string[] selector)
    {
        this.menuType = MenuTypes.Selector;
        
        this.label = label;
        this.action = action;
        this.selector = selector;
    }
    override public void Draw(bool isSelected)
    {
        index = (byte)((index + selector.Length) % selector.Length);
        
        Console.BackgroundColor = isSelected ? Color.selectedColor : Color.backgroundColor;
        Console.WriteLine(label + " - " + selector[index]);

    }

    public void Move(bool left)
    {
        if (left) { index++; }
        else { index--; }
    }   
}

static public class Menu
{
    static MenuBase[][] menu = {
        new MenuBase[] {new MenuButton("Jogar", Nothing), new MenuButton("Opções", () => ChangeMenu(1)), new MenuButton("Sair", Nothing)},
        new MenuBase[] {new MenuSelector("Linguagem", Nothing, new string[]{"PT-BR", "EN-US"}), new MenuButton("Teclas", Nothing), new MenuButton("Cores", Nothing), new MenuButton("Voltar", Nothing)}};

    static int selectedOption = 0;
    static int currentMenu = 0;
    static bool running = true;

    static public void Run()
    {
        while (running)
        {
            Draw();
            running = HandleInput();

            Console.BackgroundColor = Color.backgroundColor;
            Console.Clear();
        }
    static void Draw()
    {
        bool isSelected = false;

        for (int i = 0; i < menu[currentMenu].Length; i++)
        {
            isSelected = i == selectedOption;
            menu[currentMenu][i].Draw(isSelected);
        }
    }
    }
    static bool HandleInput()
    {
        ConsoleKey key = Console.ReadKey(true).Key;

        if (key == Keybinds.MenuUp) { selectedOption--; }
        else if (key == Keybinds.MenuDown) { selectedOption++; }
        else if (key == Keybinds.Confirm) { menu[currentMenu][selectedOption].action?.Invoke(); }
        else if (key == Keybinds.MenuLeft && menu[currentMenu][selectedOption] is MenuSelector selLeft) { selLeft.Move(true); }
        else if (key == Keybinds.MenuRight && menu[currentMenu][selectedOption] is MenuSelector selRight) { selRight.Move(false); }
        else if (key == Keybinds.Exit) { return false; }

        selectedOption = (selectedOption + menu[currentMenu].Length) % menu[currentMenu].Length;
        return true;
    }
    

    static void ChangeMenu(int menu) { currentMenu = menu; }

    static void Nothing() {  }

}