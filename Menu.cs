

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
//using static System.Console; *levanta a sobrançelha* ...talvez? 

namespace Menu {
public abstract class Base
{
        public int CalculateLabelOffset(string text) { return text.Length / 2; }
        public Base(Func<string> label, Action? action, byte buttonIndex, Alignment alignment)
        {
            this.label = label;
            this.action = action;
            this.buttonIndex = buttonIndex;
            this.alignment = alignment;

            this.labelOffset = CalculateLabelOffset(label());
        }
        public enum Alignment
            {
                Default,
                Centered,
                CenteredLeft
            }   


        public byte buttonIndex = 0;
        public int labelOffset;

        public byte[] pos = {(byte)(Console.WindowWidth / 2), (byte)(Console.WindowHeight / 2)};
        public Func<string> label;
        public Action? action;

        public Alignment alignment;

        public bool isSelected = false;
        public abstract void Draw(bool isSelected);

        public void SetCursorBasedOnAlignment(Alignment alignment, string label)
        {
            switch (alignment)
            {
                case Alignment.Centered:
                labelOffset = CalculateLabelOffset(label);
                Console.SetCursorPosition(pos[0] - labelOffset, Console.WindowHeight / 2 + buttonIndex);
                break;
                case Alignment.CenteredLeft:
                Console.SetCursorPosition(pos[0], Console.WindowHeight / 2 + buttonIndex);
                break;
            }
        }

    }

    public class Button : Base
    {
        public Button(Func<string> label, Action? action, byte buttonIndex, Alignment alignment) : base(label, action, buttonIndex, alignment) { }

        override public void Draw(bool isSelected)
        {
            Console.BackgroundColor = isSelected ? Options.Color.selectedColor : Options.Color.backgroundColor;

            string locLabel = label();

            SetCursorBasedOnAlignment(alignment, locLabel);

            Console.WriteLine(locLabel);
        }
    }

    public class Selector : Base
    {
        public byte index = 0;
        string[] selector;
        Action<string>? selectorAction;
        public Selector(Func<string> label, Action? action, byte buttonIndex, Alignment alignment, string[] selector, Action<string> selectorAction) : base(label, action, buttonIndex, alignment)
        {
            this.selector = selector;
            this.selectorAction = selectorAction;
        }
        override public void Draw(bool isSelected)
        {

            string formattedLocLabel = label() + " - " + "< " + selector[index] + " >";

            SetCursorBasedOnAlignment(alignment, formattedLocLabel);
            Console.BackgroundColor = isSelected ? Options.Color.selectedColor : Options.Color.backgroundColor;
            Console.WriteLine(formattedLocLabel);

        }
        public void Move(bool left)
        {
            if (left) { index++; }
            else { index--; }

            index = (byte)((index + selector.Length) % selector.Length);
            selectorAction.Invoke(selector[index]);
        }   
    }

    static public class Manager
    {
        static string[] langOptions = {"EN-US", "PT-BR"}; // Esses devem ser colocado em um json de alguma maneira.
        static string[] presetColorOptions = {"Default", "Hacker", "Hot Pink", "Nature"};

        // Wawa, dereference to a possibly null blabla. Ajeitar isso depois.
        // Meu deus. Deve ter uma maneira melhor de declarar isso.
        static Func<string> playFunc = () => Loc.Reader.loc.menu.main.play;
        static Func<string> optionsFunc = () => Loc.Reader.loc.menu.main.options;
        static Func<string> exitFunc = () => Loc.Reader.loc.menu.main.exit;
        static Func<string> languageFunc = () => Loc.Reader.loc.menu.options.language;
        static Func<string> keybindsFunc = () => Loc.Reader.loc.menu.options.keybinds;
        static Func<string> colorsFunc = () => Loc.Reader.loc.menu.options.colors;
        static Func<string> menuupFunc = () => Loc.Reader.loc.menu.keybinds.menuup;
        static Func<string> menudownFunc = () => Loc.Reader.loc.menu.keybinds.menudown;
        static Func<string> menuleftFunc = () => Loc.Reader.loc.menu.keybinds.menuleft;
        static Func<string> menurightFunc = () => Loc.Reader.loc.menu.keybinds.menuright;
        static Func<string> confirmFunc = () => Loc.Reader.loc.menu.keybinds.confirm;
        static Func<string> exitKFunc = () => Loc.Reader.loc.menu.keybinds.exit;
        static Func<string> exitgameFunc = () => Loc.Reader.loc.menu.keybinds.exitgame;
        static Func<string> presetsFunc = () => Loc.Reader.loc.menu.colors.presets;
        static Func<string> backgroundcolorFunc = () => Loc.Reader.loc.menu.colors.backgroundcolor;
        static Func<string> foregroundcolorFunc = () => Loc.Reader.loc.menu.colors.foregroundcolor;
        static Func<string> colorblindmodeFunc = () => Loc.Reader.loc.menu.colors.colorblindmode;
        static Func<string> gobackFunc = () => Loc.Reader.loc.menu.goback;

        // Funções de seletores
        static Action<string> languageUpdater = Loc.Reader.UpdateLoc;
        static Action<string> colorPresetsUpdater = Options.Color.ChangeColorToPreset;

        static Base.Alignment alignment = Base.Alignment.Default;


        // Alignment é compartilhado por todos, considerar colocar como uma statica de Base de alguma maneira
        static Base[] mainMenu = {new Button(playFunc, Nothing, 1, alignment), 
                                    new Button(optionsFunc, () => ChangeMenu((int)Menus.Options), 2, alignment), 
                                    new Button(exitFunc, Exit, 3, alignment)};
        
        static Base[] optionsMenu = {new Selector(languageFunc, Nothing, 1, alignment, langOptions, languageUpdater), 
                                        new Button(keybindsFunc, () => ChangeMenu((int)Menus.Keybinds), 2, alignment), 
                                        new Button(colorsFunc, () => ChangeMenu((int)Menus.Colors), 3, alignment), 
                                        new Button(gobackFunc, () => ChangeMenu((int)Menus.Main), 4, alignment)};

        static Base[] keybindsMenu = {new Button(menuupFunc, Nothing, 1, alignment), 
                                        new Button(menudownFunc, Nothing, 2, alignment), 
                                        new Button(menuleftFunc, Nothing, 3, alignment), 
                                        new Button(menurightFunc, Nothing, 4, alignment),
                                        new Button(confirmFunc, Nothing, 4, alignment),
                                        new Button(exitKFunc, Nothing, 4, alignment),
                                        new Button(exitgameFunc, Nothing, 4, alignment),
                                        new Button(gobackFunc, () => ChangeMenu((int)Menus.Options), 4, alignment)};

        static Base[] colorsMenu =   {new Selector(presetsFunc, Nothing, 1, alignment, presetColorOptions, colorPresetsUpdater), 
                                        new Button(backgroundcolorFunc, Nothing, 2, alignment), 
                                        new Button(foregroundcolorFunc, Nothing, 3, alignment), 
                                        new Button(colorblindmodeFunc, Nothing, 4, alignment),
                                        new Button(gobackFunc, () => ChangeMenu((int)Menus.Options), 5, alignment)};

        
        // Apenas butões vão usar action? tirar isso do Base dps
        static Base[][] menu = { mainMenu, optionsMenu, keybindsMenu, colorsMenu }; 

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
            else if (key == Options.Keybinds.MenuLeft && menu[currentMenu][selectedOption] is Selector selLeft) { selLeft.Move(true); }
            else if (key == Options.Keybinds.MenuRight && menu[currentMenu][selectedOption] is Selector selRight) { selRight.Move(false); }
            else if (key == Options.Keybinds.Exit) { Exit(); }

            selectedOption = (selectedOption + menu[currentMenu].Length) % menu[currentMenu].Length;
        }
        

        static void ChangeMenu(int menu) { currentMenu = menu; }

        static void Exit() {
            Console.ResetColor();
            Console.Clear();
            Console.CursorVisible = true;

            running = false;
        }

        static void Nothing() {  }

        enum Menus {
            Main,
            Options,
            Keybinds,
            Colors,
        }

    }

}