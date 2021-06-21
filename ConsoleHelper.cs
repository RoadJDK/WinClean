using WinClean.resources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Globalization;

namespace WinClean {
    public class ConsoleHelper {

        private const int FixedWidthTrueType = 54;
        private const int StandardOutputHandle = -11;

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
            }
            else {
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
            ConsoleFatal(exitCode, Strings.FatalErrorDefault);
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

        public static void ConsoleLang(string locale) {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(locale);
        }

        public static void ConsoleSleep(int ms) {
            Thread.Sleep(ms);
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
                }
                catch (FormatException) {
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

        public static void ConsoleClear() {
            Console.Clear();
        }


        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int nStdHandle);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool SetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool GetCurrentConsoleFontEx(IntPtr hConsoleOutput, bool MaximumWindow, ref FontInfo ConsoleCurrentFontEx);


        private static readonly IntPtr ConsoleOutputHandle = GetStdHandle(StandardOutputHandle);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct FontInfo {
            internal int cbSize;
            internal int FontIndex;
            internal short FontWidth;
            public short FontSize;
            public int FontFamily;
            public int FontWeight;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            //[MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.wc, SizeConst = 32)]
            public string FontName;
        }

        public static FontInfo[] ConsoleFont(string font, short fontSize = 0) {
            Console.WriteLine("Set Current Font: " + font);

            FontInfo before = new FontInfo {
                cbSize = Marshal.SizeOf<FontInfo>()
            };

            if (GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref before)) {

                FontInfo set = new FontInfo {
                    cbSize = Marshal.SizeOf<FontInfo>(),
                    FontIndex = 0,
                    FontFamily = FixedWidthTrueType,
                    FontName = font,
                    FontWeight = 400,
                    FontSize = fontSize > 0 ? fontSize : before.FontSize
                };

                // Get some settings from current font.
                if (!SetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref set)) {
                    var ex = Marshal.GetLastWin32Error();
                    Console.WriteLine("Set error " + ex);
                    throw new System.ComponentModel.Win32Exception(ex);
                }

                FontInfo after = new FontInfo {
                    cbSize = Marshal.SizeOf<FontInfo>()
                };
                GetCurrentConsoleFontEx(ConsoleOutputHandle, false, ref after);

                return new[] { before, set, after };
            } else {
                var er = Marshal.GetLastWin32Error();
                Console.WriteLine("Get error " + er);
                throw new System.ComponentModel.Win32Exception(er);
            }
        }
    }
}
