using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WinClean {
    public class ConsoleHelper {
        public struct SelectionOption {
            public int Number { get; }
            public string Text { get; }
            public SelectionOption(int _number, string _text) {
                Number = _number;
                Text = _text;
            }
        }
        public static void ConsoleWrite(string text) {
            ConsoleWrite("WinClean", text, ConsoleColor.White);
        }

        public static void ConsoleWrite(string name, string text) {
            ConsoleWrite(name, text, ConsoleColor.White);
        }

        public static void ConsoleWrite(string text, ConsoleColor color) {
            ConsoleWrite("WinClean", text, color);
        }

        public static void ConsoleWrite(string name, string text, ConsoleColor color) {
            Console.ForegroundColor = color;
            if (name == "") {
                Console.WriteLine(" " + text);
            } else {
                Console.WriteLine(" [" + name + "] " + text);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void ConsoleWriteError(string text) {
            ConsoleWrite("ERROR", text, ConsoleColor.Red);
        }

        public static void ConsoleWriteWarn(string text) {
            ConsoleWrite("WARN", text, ConsoleColor.Yellow);
        }

        public static void ConsoleFatal() {
            ConsoleFatal(-1);
        }

        public static void ConsoleFatal(int exitCode) {
            ConsoleFatal(exitCode, "A fatal error has occured!");
        }

        public static void ConsoleFatal(string message) {
            ConsoleFatal(-1, message);
        }

        public static void ConsoleFatal(int exitCode, string message) {
            ConsoleWrite("FATAL", message, ConsoleColor.Red);
            EnterToContinue();
            Environment.Exit(exitCode);
        }

        public static void ConsoleExit() {
            Environment.Exit(0);
        }

        public static void ConsoleExit(int exitCode) {
            Environment.Exit(exitCode);
        }

        public static void ConsoleTitle(string title) {
            Console.Title = "WinClean " + WinClean.Version + " - " + title;
        }

        public static string ConsoleRead() {
            return Console.ReadLine();
        }

        public static string ConsoleRead(string text) {
            Console.Write(" " + text + ": "); return ConsoleRead();
        }

        public static void EnterToContinue() {
            ConsoleWrite("Press Enter to continue...");
            ConsoleKey key = Console.ReadKey().Key;
            while (key != ConsoleKey.Enter) {
                key = Console.ReadKey().Key;
            }
        }

        public static void EnterToContinue(string text) {
            ConsoleWrite(text);
            ConsoleKey key = Console.ReadKey().Key;
            while (key != ConsoleKey.Enter) {
                key = Console.ReadKey().Key;
            }
        }

        public static string CreateQuestion(string question) {
            ConsoleWrite("", "");
            ConsoleWrite(question);

            Console.Write(" > "); var result = ConsoleRead();

            ConsoleWrite("", "");
            return result;
        }

        public static int CreateQuestion(string question, List<SelectionOption> options) {
            ConsoleWrite("", "");
            ConsoleWrite(question);

            foreach (SelectionOption option in options) {
                ConsoleWrite("", option.Number + ": " + option.Text);
            }

            int result = -1;
            while (true) {
                Console.Write(" Select (" + options.Min(option => option.Number) + "-" + options.Max(option => option.Number) + ") > ");
                try {
                    result = Convert.ToInt32(ConsoleRead());
                } catch (FormatException) {
                    ConsoleWriteError("Selection is not a number!");
                    continue;
                }
                if (!(result >= options.Min(option => option.Number) && result <= options.Max(option => option.Number))) {
                    ConsoleWriteError("Selection is not in range!");
                    continue;
                }
                break;
            }

            ConsoleWrite("", "");

            return result;
        }
    }
}
