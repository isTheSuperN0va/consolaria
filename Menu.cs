

using System.ComponentModel;

public abstract class MenuBase
{
    public string label = "";
    public Action? action;

    public bool isSelected = false;

}

public class MenuButton : MenuBase
{
    public MenuButton(string label, Action? action)
    {
        this.label = label;
        this.action = action;
    }

    public void Draw(bool isSelected)
    {
        Console.BackgroundColor = isSelected ? Color.selectedColor : Color.backgroundColor;
        Console.WriteLine(label);
    }
}

public class MenuSelector : MenuBase
{
    public MenuSelector(string label, Action? action)
    {
        this.label = label;
        this.action = action;
    }

    public void Draw(string[] selector, int i)
    {
        Console.BackgroundColor = isSelected ? Color.selectedColor : Color.backgroundColor;
        Console.WriteLine(label + " - " + selector[i]);
    }
}

static public class Menu
{
    static MenuButton[][] menu = {
        new MenuButton[] {new MenuButton("Jogar", Nothing), new MenuButton("Opções", () => ChangeMenu(1)), new MenuButton("Sair", Nothing)},
        new MenuButton[] {new MenuButton("Linguagem", Nothing), new MenuButton("Teclas", Nothing), new MenuButton("Cores", Nothing), new MenuButton("Voltar", Nothing)}};

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
    }
    static public void Draw()
    {
        bool isSelected = false;

        for (int i = 0; i < menu.Length; i++)
        {
            isSelected = i == selectedOption;
            menu[currentMenu][i].Draw(isSelected);
        }
    }

    static bool HandleInput()
    {
        ConsoleKey key = Console.ReadKey(true).Key;

        if (key == Keybinds.MenuUp) { selectedOption--; }
        else if (key == Keybinds.MenuDown) { selectedOption++; }
        else if (key == Keybinds.Confirm) { menu[currentMenu][selectedOption].action?.Invoke(); }
        else if (key == Keybinds.Exit) { return false; }

        selectedOption = (selectedOption + menu.Length) % menu.Length;
        return true;
    }

    static void ChangeMenu(int menu) { currentMenu = menu; }

    static void Nothing() {  }   
}