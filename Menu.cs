

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
//using static System.Console; *levanta a sobrançelha* ...talvez? 

public abstract class MenuBase
{
    public byte CalculateLabelOffset(string textLength) { return (byte)(textLength.Length / 2); }
    public MenuBase(Func<string> label, Action? action, byte buttonIndex)
    {
        this.label = label;
        this.action = action;
        this.buttonIndex = buttonIndex;

        this.labelOffset = CalculateLabelOffset(label());
    }
    public enum MenuTypes
        {
            Button,
            Selector,
            Keybind
        }   

    public byte buttonIndex = 0;
    public byte labelOffset;

    public byte[] pos = {(byte)(Console.WindowWidth / 2), (byte)(Console.WindowHeight / 2)};
    public Func<string> label;
    public Action? action;

    public MenuTypes menuType;

    public bool isSelected = false;
    public abstract void Draw(bool isSelected);

}

public class MenuButton : MenuBase
{
    public MenuButton(Func<string> label, Action? action, byte buttonIndex) : base(label, action, buttonIndex)
    {
        this.menuType = MenuTypes.Button;
    }

    override public void Draw(bool isSelected)
    {
        Console.BackgroundColor = isSelected ? Options.Color.selectedColor : Options.Color.backgroundColor;

        string locLabel = label();

        labelOffset = CalculateLabelOffset(locLabel);
        Console.SetCursorPosition(pos[0] - labelOffset, Console.WindowHeight / 2 + buttonIndex);
        Console.WriteLine(locLabel);
    }
}

public class MenuSelector : MenuBase
{
    public byte index = 0;
    string[] selector;
    Action<string>? selectorAction;
    public MenuSelector(Func<string> label, Action? action, byte buttonIndex, string[] selector, Action<string> selectorAction) : base(label, action, buttonIndex)
    {
        this.menuType = MenuTypes.Selector;

        this.selector = selector;
        this.selectorAction = selectorAction;
    }
    override public void Draw(bool isSelected)
    {
        Loc.Reader.UpdateLoc(selector[index]);

        string formattedLabel = label() + " - " + "< " + selector[index] + " >";
        labelOffset = CalculateLabelOffset(formattedLabel);

        Console.SetCursorPosition(pos[0] - labelOffset, pos[1] + buttonIndex);
        
        Console.BackgroundColor = isSelected ? Options.Color.selectedColor : Options.Color.backgroundColor;
        Console.WriteLine(formattedLabel);

    }
    public void Move(bool left)
    {
        if (left) { index++; }
        else { index--; }

        index = (byte)((index + selector.Length) % selector.Length);
        selectorAction.Invoke(selector[index]);
    }   
}

static public class Menu
{
    static string[] langOptions = {"EN-US", "PT-BR"}; // Esses devem ser colocado em um json de alguma maneira.

    // Wawa, dereference to a possibly null blabla. Ajeitar isso depois.
    static Func<string> playFunc = () => Loc.Reader.loc.menu.play;
    static Func<string> optionsFunc = () => Loc.Reader.loc.menu.options;
    static Func<string> exitFunc = () => Loc.Reader.loc.menu.exit;
    static Func<string> languageFunc = () => Loc.Reader.loc.menu.language;
    static Func<string> keybindsplayFunc = () => Loc.Reader.loc.menu.keybinds;
    static Func<string> colorsFunc = () => Loc.Reader.loc.menu.colors;
    static Func<string> gobackFunc = () => Loc.Reader.loc.menu.goback;

    // Funções de seletores
    static Action<string> languageUpdater = Loc.Reader.UpdateLoc;

    static MenuBase[][] menu = {
        new MenuBase[] {new MenuButton(playFunc, Nothing, 1), 
                        new MenuButton(optionsFunc, () => ChangeMenu((int)Menus.Options), 2), 
                        new MenuButton(exitFunc, Exit, 3)},
        new MenuBase[] {new MenuSelector(languageFunc, Nothing, 1, langOptions, languageUpdater), 
                        new MenuButton(keybindsplayFunc, Nothing, 2), 
                        new MenuButton(colorsFunc, Nothing, 3), 
                        new MenuButton(gobackFunc, () => ChangeMenu((int)Menus.Main), 4)}};

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
            HandleInput();

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
    static void HandleInput()
    {
        ConsoleKey key = Console.ReadKey(true).Key;



        if (key == Options.Keybinds.MenuUp) { selectedOption--; }
        else if (key == Options.Keybinds.MenuDown) { selectedOption++; }
        else if (key == Options.Keybinds.Confirm) { menu[currentMenu][selectedOption].action?.Invoke(); }
        else if (key == Options.Keybinds.MenuLeft && menu[currentMenu][selectedOption] is MenuSelector selLeft) { selLeft.Move(true); }
        else if (key == Options.Keybinds.MenuRight && menu[currentMenu][selectedOption] is MenuSelector selRight) { selRight.Move(false); }
        else if (key == Options.Keybinds.Exit) { Exit(); }

        selectedOption = (selectedOption + menu[currentMenu].Length) % menu[currentMenu].Length;
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

