

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Reflection;
//using static System.Console; *levanta a sobrançelha* ...talvez? 

namespace Menu {
public abstract class Base
{
        public int CalculateLabelOffset(string text) { return text.Length / 2; }
        public Base(Func<string> label)
        {
            this.label = label;

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
        
        public abstract void OnConfirm();
    }

    public class Button : Base
    {
        public Button(Func<string> label, byte buttonIndex, Alignment alignment, Action? action) : base(label) { 
            this.alignment = alignment; 
            this.buttonIndex = buttonIndex; 
            this.action = action; 
        }
        public Button(Func<string> label, Action? action) : base(label) { this.action = action; }

        override public void Draw(bool isSelected)
        {
            Console.BackgroundColor = isSelected ? Options.Color.selectedColor : Options.Color.backgroundColor;

            string locLabel = label();

            SetCursorBasedOnAlignment(alignment, locLabel);

            Console.WriteLine(locLabel);
        }

        public override void OnConfirm() { action?.Invoke(); }
    }

    public class SubmenuButton : Base
    {
        Manager.Menus menu;
        public SubmenuButton(Func<string> label, Manager.Menus menu) : base(label) { this.menu = menu; }

        public override void Draw(bool isSelected)
        {
            Console.BackgroundColor = isSelected ? Options.Color.selectedColor : Options.Color.backgroundColor;
            Console.WriteLine($"{label()}");
        }

        public override void OnConfirm() { Manager.ChangeMenu((int)menu); }
    }

    public class Selector : Base
    {
        public byte index = 0;
        string[] selector;
        Action<string>? selectorAction;
        public Selector(Func<string> label, byte buttonIndex, Alignment alignment, string[] selector, Action<string> selectorAction) : base(label)
        {
            this.selector = selector;
            this.selectorAction = selectorAction;
            this.buttonIndex = buttonIndex;
            this.alignment = alignment;
        }

        public Selector(Func<string> label, string[] selector, Action<string> selectorAction) : base(label)
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

        public override void OnConfirm() {}
    }

    public class KeybindHolder : Base
    {
        ConsoleKey keybind;
        FieldInfo keybindField;


        public KeybindHolder(Func<string> label, string name) : base(label) {
            this.keybindField = Options.Keybinds.FindKeybindField(name);
            keybind = (ConsoleKey)keybindField.GetValue(null); 
        }
        
        override public void Draw(bool isSelected)
        {
            Console.BackgroundColor = isSelected ? Options.Color.selectedColor : Options.Color.backgroundColor;
            Console.WriteLine($"{label()} - {Convert.ToString(keybind)}");
        }

        public override void OnConfirm()
        {
            keybind = Console.ReadKey(true).Key;
            Options.Keybinds.FindKeybindField(label()).SetValue(null, keybind);
        }
    }

    static public class Manager
    {
        

        static int selectedOption = 0;
        static int currentMenu = 0;
        static bool running = true;

        static public void Run(Base[][] menu)
        {
            while (running)
            {

                Console.Clear();
                Console.CursorVisible = false;

                Draw(menu);
                HandleInput(menu);

                Console.BackgroundColor = Options.Color.backgroundColor;
                Console.Clear();
            }
        static void Draw(Base[][] menu)
        {
            bool isSelected = false;

            for (int i = 0; i < menu[currentMenu].Length; i++)
            {
                isSelected = i == selectedOption;
                menu[currentMenu][i].Draw(isSelected);
            }
        }
        }
        static void HandleInput(Base[][] menu)
        {
            ConsoleKey key = Console.ReadKey(true).Key;



            if (key == Options.Keybinds.MenuUp) { selectedOption--; }
            else if (key == Options.Keybinds.MenuDown) { selectedOption++; }
            else if (key == Options.Keybinds.Confirm) { menu[currentMenu][selectedOption].OnConfirm(); }
            else if (key == Options.Keybinds.MenuLeft && menu[currentMenu][selectedOption] is Selector selLeft) { selLeft.Move(true); }
            else if (key == Options.Keybinds.MenuRight && menu[currentMenu][selectedOption] is Selector selRight) { selRight.Move(false); }
            else if (key == Options.Keybinds.Exit) { Exit(); }

            selectedOption = (selectedOption + menu[currentMenu].Length) % menu[currentMenu].Length;
        }
        

        public static void ChangeMenu(int menu) { currentMenu = menu; }

        public static void Exit() {
            Console.ResetColor();
            Console.Clear();
            Console.CursorVisible = true;

            running = false;
        }

        public static void Nothing() {  }

        public enum Menus {
            Main,
            Options,
            Keybinds,
            Colors,
        }

    }

    static public class Builtin
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


        // tá, func e action com lambda em KeybindHolder é muito merda
        static Base[] mainMenu = {new Button(playFunc, Manager.Nothing), 
                                    new Button(optionsFunc, () => Manager.ChangeMenu((int)Manager.Menus.Options)), 
                                    new Button(exitFunc, Manager.Exit)};
        
        static Base[] optionsMenu = {new Selector(languageFunc, langOptions, languageUpdater), 
                                        new SubmenuButton(keybindsFunc, Manager.Menus.Keybinds), 
                                        new SubmenuButton(colorsFunc, Manager.Menus.Colors), 
                                        new SubmenuButton(gobackFunc, Manager.Menus.Main)};

        static Base[] keybindsMenu = {new KeybindHolder(menuupFunc, "MenuUp"), 
                                        new KeybindHolder(menudownFunc, "MenuDown"), 
                                        new KeybindHolder(menuleftFunc, "MenuLeft"), 
                                        new KeybindHolder(menurightFunc, "MenuRight"),
                                        new KeybindHolder(confirmFunc, "Confirm"),
                                        new KeybindHolder(exitKFunc, "Exit"),
                                        new KeybindHolder(exitgameFunc, "Exit"),
                                        new SubmenuButton(gobackFunc, Manager.Menus.Options)};

        static Base[] colorsMenu =   {new Selector(presetsFunc, presetColorOptions, colorPresetsUpdater), 
                                        new Button(backgroundcolorFunc, Manager.Nothing), 
                                        new Button(foregroundcolorFunc, Manager.Nothing), 
                                        new Button(colorblindmodeFunc, Manager.Nothing),
                                        new SubmenuButton(gobackFunc, Manager.Menus.Options)};

        
        public static Base[][] menu = { mainMenu, optionsMenu, keybindsMenu, colorsMenu }; 
    }

}