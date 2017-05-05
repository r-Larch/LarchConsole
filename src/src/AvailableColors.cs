using System;
using System.Collections.Generic;
using System.Linq;


namespace LarchConsole {
    public class AvailableColors {
        private readonly List<ConsoleColor> _colors;

        public AvailableColors() {
            //_colors = DarkColors(Console.ForegroundColor, Console.BackgroundColor);
            _colors = LightColors(Console.ForegroundColor, Console.BackgroundColor);
        }

        public ConsoleColor GetColor(int num) {
            while (num >= _colors.Count) {
                num -= _colors.Count;
            }

            if (num < 0) {
                num = 0;
            }

            //return _colors[_colors.Count - num - 1];
            return _colors[num];
        }

        public static List<ConsoleColor> DarkColors(params ConsoleColor[] exclude) {
            return (new List<ConsoleColor>() {
                ConsoleColor.Cyan,
                ConsoleColor.DarkYellow,
                ConsoleColor.DarkGreen,
                ConsoleColor.DarkRed,
                ConsoleColor.Green,
                ConsoleColor.Yellow,
                ConsoleColor.DarkMagenta,
                ConsoleColor.DarkGray,
                ConsoleColor.DarkCyan,
                ConsoleColor.Red,
                ConsoleColor.Magenta,
                ConsoleColor.Blue,
                ConsoleColor.DarkBlue,
                ConsoleColor.Gray,
                ConsoleColor.Black,
                ConsoleColor.White
            }).Where(c => !exclude.Contains(c)).ToList();
        }

        public static List<ConsoleColor> LightColors(params ConsoleColor[] exclude) {
            return (new List<ConsoleColor>() {
                ConsoleColor.Red,
                ConsoleColor.Green,
                ConsoleColor.Blue,
                ConsoleColor.Cyan,
                ConsoleColor.DarkRed,
                ConsoleColor.DarkGreen,
                ConsoleColor.DarkBlue,
                ConsoleColor.DarkCyan,
                ConsoleColor.Yellow,
                ConsoleColor.Magenta,
                ConsoleColor.Gray,
                ConsoleColor.DarkYellow,
                ConsoleColor.DarkMagenta,
                ConsoleColor.DarkGray,
                ConsoleColor.Black,
                ConsoleColor.White
            }).Where(c => !exclude.Contains(c)).ToList();
        }
    }
}