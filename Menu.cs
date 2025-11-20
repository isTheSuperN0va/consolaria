

using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Reflection;
using Game;
//using static System.Console; *levanta a sobrançelha* ...talvez? 

namespace Menu {
public abstract class Base
{
        public int CalculateLabelOffset(string text) { return text.Length / 2; }
        public Base(string key)
        {
            this.key = key;

            this.label = Loc.Reader.Get(key);
            this.labelOffset = CalculateLabelOffset(this.label);
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
        public string key;
        public string label;
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
        public Button(string key, byte buttonIndex, Alignment alignment, Action? action) : base(key) { 
            this.alignment = alignment; 
            this.buttonIndex = buttonIndex; 
            this.action = action; 
        }
        public Button(string key, Action? action) : base(key) { this.action = action; }

        override public void Draw(bool isSelected)
        {
            Console.BackgroundColor = isSelected ? Options.Color.selectedColor : Options.Color.backgroundColor;

            label = Loc.Reader.Get(key);

            SetCursorBasedOnAlignment(alignment, label);

            Console.WriteLine(label);
        }

        public override void OnConfirm() { action?.Invoke(); } 
    }

    public class SubmenuButton : Base
    {
        Manager.Menus menu;
        public SubmenuButton(string key, Manager.Menus menu) : base(key) { this.menu = menu; }

        public override void Draw(bool isSelected)
        {
            Console.BackgroundColor = isSelected ? Options.Color.selectedColor : Options.Color.backgroundColor;
            Console.WriteLine($"{Loc.Reader.Get(key)}");
        }

        public override void OnConfirm() { Manager.ChangeMenu((int)menu); }
    }

    public class Selector : Base
    {
        public byte index = 0;
        string[] selector;
        Action<string>? selectorAction;
        public Selector(string key, byte buttonIndex, Alignment alignment, string[] selector, Action<string> selectorAction) : base(key)
        {
            this.selector = selector;
            this.selectorAction = selectorAction;
            this.buttonIndex = buttonIndex;
            this.alignment = alignment;
        }

        public Selector(string key, string[] selector, Action<string> selectorAction) : base(key)
        {
            this.selector = selector;
            this.selectorAction = selectorAction;
        }
        override public void Draw(bool isSelected)
        {

            string formattedLocLabel = Loc.Reader.Get(key) + " - " + "< " + selector[index] + " >";

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


        public KeybindHolder(string key, string keybindName) : base(key) {
            this.keybindField = Options.Keybinds.FindKeybindField(keybindName);
            keybind = (ConsoleKey)keybindField.GetValue(null); 
        }
        
        override public void Draw(bool isSelected)
        {
            Console.BackgroundColor = isSelected ? Options.Color.selectedColor : Options.Color.backgroundColor;
            Console.WriteLine($"{Loc.Reader.Get(key)} - {Convert.ToString(keybind)}");
        }

        public override void OnConfirm()
        {
            keybind = Console.ReadKey(true).Key;
            Options.Keybinds.FindKeybindField(Loc.Reader.Get(key)).SetValue(null, keybind);
        }
    }

    static public class Manager
    {
        

        static int selectedOption = 0;
        static int currentMenu = 0;
        static bool running = true;
        public static bool gameStart = false;

        static public void Run(Base[][] menu)
        {
            Input[] menuInputs = RegisterMenuInputs(menu);

            while (running)
            {

                Console.ResetColor();
                Console.Clear();
                Console.CursorVisible = false;

                Draw(menu);
                HandleInput(menu, menuInputs);
            }
        static void Draw(Base[][] menu)
        {
            bool isSelected;

            for (int i = 0; i < menu[currentMenu].Length; i++)
            {
                isSelected = i == selectedOption;
                menu[currentMenu][i].Draw(isSelected);
            }
        }
        }

        static Input[] RegisterMenuInputs(Base[][] menu)
        {
            Action nextOption = () => selectedOption--;
            Action previousOption = () => selectedOption++;
            Action confirm = () => menu[currentMenu][selectedOption].OnConfirm();
            Action selectorLeft = () => { if (menu[currentMenu][selectedOption] is Selector selLeft) { selLeft.Move(true); } }; 
            Action selectorRight = () => { if (menu[currentMenu][selectedOption] is Selector selLeft) { selLeft.Move(false); } };
            Action exit = Exit;

            Input[] inputs =
            {
                new Input(Options.Keybinds.MenuUp, nextOption),
                new Input(Options.Keybinds.MenuDown, previousOption),
                new Input(Options.Keybinds.Confirm, confirm),
                new Input(Options.Keybinds.MenuLeft, selectorLeft),
                new Input(Options.Keybinds.MenuRight, selectorRight),
                new Input(Options.Keybinds.Exit, exit),
            };

            return inputs;
        }
        static void HandleInput(Base[][] menu, Input[] inputs)
        {
            ConsoleKey? key; 
            bool keyWasPressed = Utils.KeyPressedWait(null, out key);

            if (keyWasPressed) foreach (Input input in inputs) input.CheckAgainst(key);
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

        static readonly string mainMenuPrefix = "menu.main";
        static readonly string optionsMenuPrefix = "menu.options";
        static readonly string keybindsMenuPrefix = "menu.keybinds";
        static readonly string colorsMenuPrefix = "menu.colors";
        static readonly string gobackKey = "menu.goback";


        // Funções de seletores
        static Action<string> languageUpdater = Loc.Reader.Load;
        static Action<string> colorPresetsUpdater = Options.Color.ChangeColorToPreset;

        static Base.Alignment alignment = Base.Alignment.Default;


        static Base[] mainMenu = {new Button(mainMenuPrefix + ".play", Game.Manager.Start), 
                                    new SubmenuButton(mainMenuPrefix + ".options", Manager.Menus.Options), 
                                    new Button(mainMenuPrefix + ".exit", Manager.Exit)};
        
        static Base[] optionsMenu = {new Selector(optionsMenuPrefix + ".language", langOptions, languageUpdater), 
                                        new SubmenuButton(optionsMenuPrefix + ".keybinds", Manager.Menus.Keybinds), 
                                        new SubmenuButton(optionsMenuPrefix + ".colors", Manager.Menus.Colors), 
                                        new SubmenuButton(gobackKey, Manager.Menus.Main)};

        static Base[] keybindsMenu = {new KeybindHolder(keybindsMenuPrefix + ".menuup", "MenuUp"), 
                                        new KeybindHolder(keybindsMenuPrefix + ".menudown", "MenuDown"), 
                                        new KeybindHolder(keybindsMenuPrefix + ".menuleft", "MenuLeft"), 
                                        new KeybindHolder(keybindsMenuPrefix + ".menuright", "MenuRight"),
                                        new KeybindHolder(keybindsMenuPrefix + ".confirm", "Confirm"),
                                        new KeybindHolder(keybindsMenuPrefix + ".exit", "Exit"),
                                        new KeybindHolder(keybindsMenuPrefix + ".exit", "Exit"),
                                        new SubmenuButton(gobackKey, Manager.Menus.Options)};

        static Base[] colorsMenu =   {new Selector(colorsMenuPrefix + ".presets", presetColorOptions, colorPresetsUpdater), 
                                        new Button(colorsMenuPrefix + ".backgroundcolor", Manager.Nothing), 
                                        new Button(colorsMenuPrefix + ".foregroundcolor", Manager.Nothing), 
                                        new Button(colorsMenuPrefix + ".colorblindmode", Manager.Nothing),
                                        new SubmenuButton(gobackKey, Manager.Menus.Options)};

        
        public static Base[][] menu = { mainMenu, optionsMenu, keybindsMenu, colorsMenu }; 
    }

}