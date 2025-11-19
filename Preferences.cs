using System.Reflection;
using System.Security;
using System.Text.Json;

namespace Loc {
    public class Main
    {
        public string? play {get; set;} 
        public string? options {get; set;} 
        public string? exit {get; set;} 
    }
    public class Options
    {
        public string? language {get; set;} 
        public string? keybinds {get; set;} 
        public string? colors {get; set;} 
    }

    public class Keybinds
    {
        public string? menuup {get; set;} 
        public string? menudown {get; set;} 
        public string? menuleft {get; set;} 
        public string? menuright {get; set;} 
        public string? confirm {get; set;} 
        public string? exit {get; set;} 
        public string? exitgame {get; set;} 
    }

    public class Colors
    {
        public string? presets {get; set;} 
        public string? backgroundcolor {get; set;} 
        public string? foregroundcolor {get; set;} 
        public string? colorblindmode {get; set;} 
    }

    public class Menu
    {
        public Main? main {get; set;}
        public Options? options {get; set;}
        public Keybinds? keybinds {get; set;}
        public Colors? colors {get; set;}
        public string? goback {get; set;}
    }

    public class Loc
    {
        public Menu? menu {get; set;}
    }

    static public class Reader
    {
        static public Loc? loc = new Loc {};
        static Dictionary<string, string> Dictionary = new Dictionary<string, string>();

        public static void Load(string langName)
        {
            langName = langName.ToLower();
            string json = File.ReadAllText("lang/" + langName + ".json");
            loc = JsonSerializer.Deserialize<Loc>(json);
            Reader.Dictionary = Flatten(loc);

            foreach (string field in Dictionary.Keys) {
                System.Console.WriteLine(field);
            }
        }

        public static string Get(string key)
        {
            return Dictionary[key];
        }

        static Dictionary<string, string> Flatten(object? root)
        {
            var dict = new Dictionary<string, string>();
            Walk(root, "", dict);
            return dict;
        }

        static void Walk(object? node, string prefix, Dictionary<string, string> dict)
        {
            if (node == null) return;

            var type = node.GetType();

            if (type == typeof(string))
            {
                dict[prefix] = (string)node;
                return;
            }

            foreach (var property in type.GetProperties()) {
                if (!property.CanRead) continue;

                var value = property.GetValue(node, null);
                var nPrefix = prefix == "" ? property.Name : $"{prefix}.{property.Name}";
                Walk(value, nPrefix, dict);
            }


        }


    }
}

namespace Options {
    static public class Color
    {
        public static ConsoleColor backgroundColor = ConsoleColor.Black;
        public static ConsoleColor selectedColor = ConsoleColor.White;

        public static void ChangeColorToPreset(string preset)
        {
            switch (preset)
            {
                case "Default":
                backgroundColor = ConsoleColor.Black;
                selectedColor = ConsoleColor.White;

                break;
                case "Hacker":
                backgroundColor = ConsoleColor.Black;
                selectedColor = ConsoleColor.Green;

                break;
                case "Hot Pink":
                backgroundColor = ConsoleColor.Red;
                selectedColor = ConsoleColor.Magenta;

                break;
                case "Nature":
                backgroundColor = ConsoleColor.Cyan;
                selectedColor = ConsoleColor.DarkGreen;

                break;
            }
        }

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

        static FieldInfo[] GetFieldsOfType<T>(Type t)
        {
            return t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).Where(f => f.FieldType == typeof(T)).ToArray();
        }

        public static FieldInfo? FindKeybindField(string name)
        {
            foreach (FieldInfo fieldInfo in GetFieldsOfType<ConsoleKey>(typeof(Keybinds))) {
                if (fieldInfo.Name == name) return fieldInfo; 
            }

            return null;
        }

    }
}