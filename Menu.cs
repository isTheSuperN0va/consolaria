

using System.ComponentModel;

public class MenuItem
{
    string label = "";

    public Action? action;
    bool isSelected = false;

    public MenuItem(string label, Action? action)
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

static public class Menu
{
    static MenuItem[][] menu = {
        new MenuItem[] {new MenuItem("Jogar", Nothing), new MenuItem("Opções", () => ChangeMenu(1)), new MenuItem("Sair", Nothing)},
        new MenuItem[] {new MenuItem("Linguagem", Nothing), new MenuItem("Teclas", Nothing), new MenuItem("Voltar", Nothing)}};

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