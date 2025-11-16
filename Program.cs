// See https://aka.ms/new-console-template for more information


using System.Reflection;

Loc.Reader.UpdateLoc("en-us");
Menu.Run();

























// const int fps = 60;

// const char player = 'P';
// const char background = '□';

// int[] size = { 20, 20 };
// int[] playerPos = { 0, 0 };




// Console.Clear();
// while (true) {
//     Console.SetCursorPosition(0, 0);

//     for (int i = 0; i < size[0]; i++)
//     {
//         for (int j = 0; j < size[1]; j++)
//         {
//             if (i == playerPos[0] && j == playerPos[1]) { Console.Write(player); }
//             else { Console.Write(background); }
//         }
//         Console.WriteLine();
//     }
//     Console.WriteLine("x = " + playerPos[0] + " y = " + playerPos[1]);


//     if (Console.KeyAvailable)
//     {
//         ConsoleKey key = Console.ReadKey(true).Key;

//         if (key == ConsoleKey.Escape) { break; }

//         switch (key)
//         {
//             case ConsoleKey.W:
//             case ConsoleKey.UpArrow:
//                 playerPos[0]--;

//                 break;
//             case ConsoleKey.S:
//             case ConsoleKey.DownArrow:
//                 playerPos[0]++;

//                 break;
//             case ConsoleKey.A:
//             case ConsoleKey.LeftArrow:
//                 playerPos[1]--;

//                 break;
//             case ConsoleKey.D:
//             case ConsoleKey.RightArrow:
//                 playerPos[1]++;

//                 break;
//         }
//     }


//     playerPos[0] = (playerPos[0] + size[0]) % size[0];
//     playerPos[1] = (playerPos[1] + size[1]) % size[1];

//     Thread.Sleep(1000 / fps); 
// }