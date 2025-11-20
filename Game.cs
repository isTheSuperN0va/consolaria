using System.Drawing;
using System.Net;
using System.Threading.Tasks.Dataflow;
using Loc;
using static System.Console;

namespace Game
{
    public static class Manager
    {
        static int counter = 0;
        static public Queue<ConsoleKeyInfo> IBuffer = new();
        
        static bool gameRunning = false;
        static string[] room;
        public static void Start()
        {
            Menu.Manager.Exit();
            CursorVisible = false;



            Input[] playerInputs = RegisterPlayerInput();
            room = File.ReadAllLines("test.txt");
            gameRunning = true;

            while (gameRunning)
            {
                RoomWriter(room);

                

                CheckPlayerInput(playerInputs);
                SetCursorPosition(0, 0);

            }
        }

        static void RoomWriter(string[] room)
        {
            for (int x = 0; x < room.Length; x++) {
                for (int y = 0; y < room[0].ToCharArray().Length; y++) {
                    if (x == Player.x && y == Player.y)  { Write(Player.body); }
                    else { Write(room[x][y]); }
                }
                WriteLine();
            }
        }

        static Input[] RegisterPlayerInput()
        {
            Action moveLeftAction =  () => Player.y--; 
            Action moveRightAction = () => Player.y++; 
            Action moveUpAction =    () => Player.x--; 
            Action moveDownAction =  () => Player.x++; 

            Input MoveLeft = new Input(Options.Keybinds.MenuLeft, moveLeftAction);
            Input MoveRight = new Input(Options.Keybinds.MenuRight, moveRightAction);
            Input MoveUp = new Input(Options.Keybinds.MenuUp, moveUpAction);
            Input MoveDown = new Input(Options.Keybinds.MenuDown, moveDownAction);

            Input[] inputs = {MoveRight, MoveLeft, MoveUp, MoveDown};
            return inputs;
        }

        static void CheckPlayerInput(Input[] inputs)
        {
            foreach (Input input in inputs) { input.CheckOnce(); }
        }
        
    }

    public class Input
    {
        private ConsoleKey key;
        private Action action;

        ConsoleKeyInfo? cki = null;

        public Input(ConsoleKey key, Action action)
        {
            this.key = key;
            this.action = action;
        }

        
        public void CheckOnce()
        {
            if (Utils.KeyPressedOnce(key)) { action.Invoke(); }
        }

        public void CheckAgainst(ConsoleKey? keyOut) { 
            System.Console.WriteLine("done");
            if (key == keyOut) action.Invoke(); }
    }

    public static class Utils
    {
        private static ConsoleKey? pressedKey = null;
        public static bool KeyPressedOnce(ConsoleKey key)
        {
            if (KeyAvailable) { pressedKey = ReadKey().Key; }

            if (pressedKey == key) 
            { 
                pressedKey = null;
                return true; 
            }
            else { return false; }
        }

        public static bool KeyPressedWait(ConsoleKey? key, out ConsoleKey? pressedKey)
        {
            pressedKey = ReadKey(true).Key;
            if (key == pressedKey || key == null ) return true;
            else return false;
        }
    }
}