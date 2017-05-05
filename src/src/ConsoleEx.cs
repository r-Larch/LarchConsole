using System;
using System.Collections.Generic;
using System.Linq;


namespace LarchConsole {
    public class ConsoleEx {
        public static void PrintException(string message, Exception e) {
            if (Console.CursorLeft != 0) {
                Console.WriteLine();
            }

            var width = Console.WindowWidth;
            Console.ForegroundColor = ConsoleColor.Red;

            Repeat("─", width);
            Console.WriteLine();
            Console.WriteLine($"{e.GetType().Name}: {message}");
            Console.WriteLine(e.StackTrace);
            Console.WriteLine();
            Repeat("─", width);

            Console.ResetColor();
        }

        public static void Repeat(string sign, int num) {
            for (var i = 0; i < num; i++) {
                Console.Write(sign);
            }
        }

        public static bool AskForYes(string question) {
            return AskForYes(ConsoleWriter.Create(question));
        }

        public static bool AskForYes(ConsoleWriter question) {
            question.Write(" (Y/N) ");
            question.Flush();
            while (true) {
                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.Y || key == ConsoleKey.N) {
                    Console.WriteLine();
                    return key == ConsoleKey.Y;
                }
            }
        }

        public static List<T> AskYesOrNo<T>(List<T> list, Func<T, ConsoleWriter> question) {
            var result = new List<T>();
            foreach (var x in list) {
                if (AskForYes(question(x))) {
                    result.Add(x);
                }
                Console.WriteLine();
            }

            return result;
        }

        public static void PrintWithPaging<T>(IEnumerable<T> list, Func<T, int, ConsoleWriter> line, int countAll = -1, ConsoleWriter header = null) {
            var array = list as T[] ?? list.ToArray();

            var writer = new ConsoleWriter();
            writer.Append(header);
            var lineNumber = 0;
            foreach (var item in array) {
                writer.Append(line?.Invoke(item, lineNumber++) ?? new ConsoleWriter().WriteLine());
            }

            var found = array.Length;

            var height = Console.WindowHeight;
            var pages = (int) Math.Ceiling((writer.LinesCount)/(double) height);

            Console.WriteLine($"found: {found}" + (countAll != -1 ? $" matchs in {countAll} entries" : ""));

            if (writer.LinesCount == 0) {
                return;
            }

            if (writer.LinesCount >= height) {
                if (!AskForYes($"Are you sure you wand show {pages} pages full text?")) {
                    return;
                }

                Console.WriteLine();
            }

            var startIndex = 0;
            var flushLines = height - 1;
            for (var page = 0; page < pages; page++) {
                if ((startIndex + flushLines) < writer.LinesCount) {
                    writer.Flush(startIndex, flushLines);
                    startIndex += flushLines;

                    var keyInfo = ReadKey($" -- page: {page + 1}/{pages} -- ");
                    if (keyInfo.Key != ConsoleKey.Escape) {
                        DeleteCurrentLine();
                        continue;
                    }

                    break;
                }

                writer.Flush(startIndex, writer.LinesCount - startIndex - 1);
            }
        }

        public static void DeleteCurrentLine() {
            var top = Console.CursorTop;
            Console.CursorLeft = 0;
            Repeat(" ", Console.WindowWidth);
            Console.CursorLeft = 0;
            Console.CursorTop = top;
        }

        public static ConsoleKeyInfo ReadKey(string text) {
            Console.Write(text);
            while (true) {
                var keyInfo = Console.ReadKey();
                if (keyInfo.Modifiers == ConsoleModifiers.Control) {
                    if (
                        // do nothing if the user wants to scroll
                        keyInfo.Key == ConsoleKey.UpArrow ||
                        keyInfo.Key == ConsoleKey.DownArrow ||
                        keyInfo.Key == ConsoleKey.Pa1 ||
                        keyInfo.Key == ConsoleKey.End
                        ) {
                        continue;
                    }
                }

                return keyInfo;
            }
        }
    }
}