using System.Text.Json;

namespace Loc {
    public class Menu
    {
        public string? play {get; set;}
        public string? options {get; set;}
        public string? exit {get; set;}
        public string? language {get; set;}
        public string? keybinds {get; set;}
        public string? colors {get; set;}
        public string? goback {get; set;}
    }

    public class Loc
    {
        public Menu? menu {get; set;}
    }

    static public class Reader
    {
        static public Loc? loc = new Loc();
        static public void UpdateLoc(string locfilename)
        {
            string json = File.ReadAllText("lang/" + locfilename + ".json");
            loc = JsonSerializer.Deserialize<Loc>(json);
        }
    }
}

namespace Options {
    static public class Color
    {
        public static ConsoleColor backgroundColor = Console.BackgroundColor;
        public static ConsoleColor selectedColor = ConsoleColor.White;

    }


    static public class Keybinds
    {
        public static ConsoleKey MenuUp = ConsoleKey.UpArrow;
        public static ConsoleKey MenuUpAlt = ConsoleKey.W;
        public static ConsoleKey MenuDown = ConsoleKey.DownArrow;
        public static ConsoleKey MenuDownAlt = ConsoleKey.S;
        public static ConsoleKey MenuLeft = ConsoleKey.LeftArrow;
        public static ConsoleKey MenuLeftAlt = ConsoleKey.A;
        public static ConsoleKey MenuRight = ConsoleKey.RightArrow;
        public static ConsoleKey MenuRightAlt = ConsoleKey.D;
        public static ConsoleKey Confirm = ConsoleKey.Enter;
        public static ConsoleKey ConfirmAlt = ConsoleKey.Spacebar;
        public static ConsoleKey Exit = ConsoleKey.Escape;

    }
}