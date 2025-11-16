

using System;

public abstract class MenuBase
{
    public enum MenuTypes
        {
            Button,
            Selector,
            Keybind
        }   

    public Func<string> label;
    public Action? action;

    public MenuTypes menuType;

    public bool isSelected = false;
    public abstract void Draw(bool isSelected);

}

public class MenuButton : MenuBase
{
    public MenuButton(Func<string> label, Action? action)
    {
        this.label = label;
        this.action = action;

        this.menuType = MenuTypes.Button;
    }

    override public void Draw(bool isSelected)
    {
        Console.BackgroundColor = isSelected ? Options.Color.selectedColor : Options.Color.backgroundColor;
        Console.WriteLine(label());
    }
}

public class MenuSelector : MenuBase
{
    public byte index = 0;
    string[] selector;
    public MenuSelector(Func<string> label, Action? action, string[] selector)
    {
        this.menuType = MenuTypes.Selector;
        
        this.label = label;
        this.action = action;
        this.selector = selector;
    }
    override public void Draw(bool isSelected)
    {
        index = (byte)((index + selector.Length) % selector.Length);

        Loc.Reader.UpdateLoc(selector[index].ToLower());
        
        Console.BackgroundColor = isSelected ? Options.Color.selectedColor : Options.Color.backgroundColor;
        Console.WriteLine(label() + " - " + selector[index]);

    }

    public void Move(bool left)
    {
        if (left) { index++; }
        else { index--; }
    }   
}

static public class Menu
{
    static string[] langOptions = {"EN-US", "PT-BR"};

    static Func<string> playFunc = () => Loc.Reader.loc.menu.play;
    static Func<string> optionsFunc = () => Loc.Reader.loc.menu.options;
    static Func<string> exitFunc = () => Loc.Reader.loc.menu.play;
    static Func<string> languageFunc = () => Loc.Reader.loc.menu.language;
    static Func<string> keybindsplayFunc = () => Loc.Reader.loc.menu.keybinds;
    static Func<string> colorsFunc = () => Loc.Reader.loc.menu.colors;
    static Func<string> gobackFunc = () => Loc.Reader.loc.menu.goback;


    static MenuBase[][] menu = {
        new MenuBase[] {new MenuButton(playFunc, Nothing), 
                        new MenuButton(optionsFunc, () => ChangeMenu((int)Menus.Options)), 
                        new MenuButton(exitFunc, Exit)},
        new MenuBase[] {new MenuSelector(languageFunc, Nothing, langOptions), 
                        new MenuButton(keybindsplayFunc, Nothing), 
                        new MenuButton(colorsFunc, Nothing), 
                        new MenuButton(gobackFunc, () => ChangeMenu((int)Menus.Main))}};

    static int selectedOption = 0;
    static int currentMenu = 0;
    static bool running = true;

    static public void Run()
    {
        while (running)
        {
            Console.Clear();
            Console.CursorVisible = false;

            Draw();
            running = HandleInput();

            Console.BackgroundColor = Options.Color.backgroundColor;
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

        if (key == Options.Keybinds.MenuUp) { selectedOption--; }
        else if (key == Options.Keybinds.MenuDown) { selectedOption++; }
        else if (key == Options.Keybinds.Confirm) { menu[currentMenu][selectedOption].action?.Invoke(); }
        else if (key == Options.Keybinds.MenuLeft && menu[currentMenu][selectedOption] is MenuSelector selLeft) { selLeft.Move(true); }
        else if (key == Options.Keybinds.MenuRight && menu[currentMenu][selectedOption] is MenuSelector selRight) { selRight.Move(false); }
        else if (key == Options.Keybinds.Exit) { Exit(); }

        selectedOption = (selectedOption + menu[currentMenu].Length) % menu[currentMenu].Length;
        return true;
    }
    

    static void ChangeMenu(int menu) { currentMenu = menu; }

    static void Exit() {
        Console.BackgroundColor = Options.Color.backgroundColor; // TODO: Add uma variavel defaultColor. isso ds jeito pode dar merda mais na frente
        Console.Clear();
        Console.CursorVisible = true;

        running = false;
    }

    static void Nothing() {  }

    enum Menus {
        Main,
        Options
    }

}

