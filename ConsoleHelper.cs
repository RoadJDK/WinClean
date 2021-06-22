using WinClean.resources;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Globalization;
using System.Diagnostics;

namespace WinClean {
    public class ConsoleHelper {

        private const int FixedWidthTrueType = 54;
        private const int StandardOutputHandle = -11;

        /// <summary>
        /// Use this struct inside a list together with the CreateQuestion function.
        /// </summary>
        /// <see cref="CreateQuestion(string, List{SelectionOption})"/></see>
        public struct SelectionOption {
            public int Number { get; }
            public string Text { get; }
            public SelectionOption(int _number, string _text) {
                Number = _number;
                Text = _text;
            }
        }
        /// <summary>
        /// Writes the specified text from WinClean to the Console in white.
        /// </summary>
        /// <param name="text">The text that should be written.</param>
        public static void ConsoleWrite(string text) {
            ConsoleWrite("WinClean", text, ConsoleColor.White);
        }

        /// <summary>
        /// Writes the specified text from the specified name to the Console in white.
        /// </summary>
        /// <param name="name">The name that should be written from.</param>
        /// <param name="text">The text that should be written.</param>
        public static void ConsoleWrite(string name, string text) {
            ConsoleWrite(name, text, ConsoleColor.White);
        }

        /// <summary>
        /// Writes the specified text from WinClean to the Console in the specified color.
        /// </summary>
        /// <param name="text">The text that should be written.</param>
        /// <param name="color">The color in which the text should be.</param>
        public static void ConsoleWrite(string text, ConsoleColor color) {
            ConsoleWrite("WinClean", text, color);
        }

        /// <summary>
        /// Writes the specified text from the specified name to the Console in the specified color.
        /// </summary>
        /// <param name="name">The name that should be written from.</param>
        /// <param name="text">The text that should be written.</param>
        /// <param name="color">The color in which the text should be.</param>
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

        /// <summary>
        /// Writes the specified text from Error to the Console in red.
        /// </summary>
        /// <param name="text">The text that should be written</param>
        public static void ConsoleWriteError(string text) {
            ConsoleWrite("ERROR", text, ConsoleColor.Red);
        }

        /// <summary>
        /// Writes the specified text from Warn to the Console in yellow.
        /// </summary>
        /// <param name="text"></param>
        public static void ConsoleWriteWarn(string text) {
            ConsoleWrite("WARN", text, ConsoleColor.Yellow);
        }

        /// <summary>
        /// Ends the program with exit code -1 and writes strings.FatalErrorDefault from Fatal to the Console in red.
        /// </summary>
        public static void ConsoleFatal() {
            ConsoleFatal(-1);
        }

        /// <summary>
        /// Ends the program with the specified exit code and writes strings.FatalErrorDefault from Fatal to the Console in red.
        /// </summary>
        /// <param name="exitCode"></param>
        public static void ConsoleFatal(int exitCode) {
            ConsoleFatal(exitCode, Strings.FatalErrorDefault);
        }

        /// <summary>
        /// Ends the program with exit code -1 and writes the specified message from Fatal to the Console in red.
        /// </summary>
        /// <param name="message">Message to write</param>
        public static void ConsoleFatal(string message) {
            ConsoleFatal(-1, message);
        }

        /// <summary>
        /// Ends the program with the specified exit code and writes the specified message from Fatal to the Console in red.
        /// </summary>
        /// <param name="exitCode"></param>
        /// <param name="message">Message to write</param>
        public static void ConsoleFatal(int exitCode, string message) {
            ConsoleWrite("FATAL", message, ConsoleColor.Red);
            EnterToContinue(Strings.EnterToExit);
            Environment.Exit(exitCode);
        }

        /// <summary>
        /// Ends the program with exit code 0.
        /// </summary>
        public static void ConsoleExit() {
            Environment.Exit(0);
        }

        /// <summary>
        /// Ends the program with the specified exit code.
        /// </summary>
        /// <param name="exitCode"></param>
        public static void ConsoleExit(int exitCode) {
            Environment.Exit(exitCode);
        }

        /// <summary>
        /// Changes the title to the specified title.
        /// Title Structure: "WinClean v0.1.0 - Example"
        /// </summary>
        /// <param name="title">The title to change to</param>
        public static void ConsoleTitle(string title) {
            Console.Title = "WinClean " + WinClean.Version + " - " + title;
        }

        /// <summary>
        /// Changes the language of the Console to the specified locale
        /// </summary>
        /// <param name="locale">The locale to change to.</param>
        public static void ConsoleLang(string locale) {
            CultureInfo culture = new CultureInfo(locale);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        /// <summary>
        /// Sleeps for the specified time
        /// </summary>
        /// <param name="ms">Time to sleep in milliseconds</param>
        public static void ConsoleSleep(int ms) {
            Thread.Sleep(ms);
        }

        public static int ConsoleRun(string command, bool hide, bool admin) {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = !hide;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            if (admin) {
                cmd.StartInfo.Verb = "runas";
            }
            cmd.Start();

            cmd.StandardInput.WriteLine(command);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            if (!hide) {
                ConsoleWrite("CMD", cmd.StandardOutput.ReadToEnd());
            }

            return cmd.ExitCode;
        }

        public static bool ConsoleRunWithChecks(string command, string check, bool admin) {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = false;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            if (admin) {
                cmd.StartInfo.Verb = "runas";
            }
            cmd.Start();

            cmd.StandardInput.WriteLine(command);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            if (cmd.StandardOutput.ReadToEnd().Contains(check)) {
                return true;
            } else {
                return false;
            }
        }

        public static string ConsoleRunWithChecks(string command, List<string> checks, bool hide, bool admin) {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            if (admin) {
                cmd.StartInfo.Verb = "runas";
            }
            cmd.Start();

            cmd.StandardInput.WriteLine(command);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            string cmdOut = cmd.StandardOutput.ReadToEnd();
            if (!hide) {
                ConsoleWrite("CMD", cmdOut);
            }
            foreach (string check in checks) {
                if (cmdOut.Contains(check, StringComparison.OrdinalIgnoreCase)) {
                    return check;
                }
            }
            return null;
        }

        /// <summary>
        /// Reads the next line from the Console
        /// </summary>
        /// <returns>The next line from the Console</returns>
        public static string ConsoleRead() {
            return Console.ReadLine();
        }

        /// <summary>
        /// Reads the next line from the Console and adds the specified text before.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ConsoleRead(string text) {
            Console.Write(" " + text + " > "); return ConsoleRead();
        }

        /// <summary>
        /// Writes strings.EnterToContinue to the Console and waits for user to press enter.
        /// </summary>
        public static void EnterToContinue() {
            ConsoleWrite(Strings.EnterToContinue);
            ConsoleKey key = Console.ReadKey().Key;
            while (key != ConsoleKey.Enter) {
                key = Console.ReadKey().Key;
            }
        }

        /// <summary>
        /// Writes the specified text to the Console and waits for user to press enter.
        /// </summary>
        /// <param name="text">The text to write.</param>
        public static void EnterToContinue(string text) {
            ConsoleWrite(text);
            ConsoleKey key = Console.ReadKey().Key;
            while (key != ConsoleKey.Enter) {
                key = Console.ReadKey().Key;
            }
        }

        /// <summary>
        /// Writes the specified question to the Console and awaits input from user.
        /// </summary>
        /// <param name="question">The question to ask the user.</param>
        /// <returns>The inputted string from the user.</returns>
        public static string CreateQuestion(string question) {
            ConsoleWrite("", "");
            ConsoleWrite(question);

            Console.Write(" > "); var result = ConsoleRead();

            ConsoleWrite("", "");
            return result;
        }

        /// <summary>
        /// Writes the specified question and all specified SelectionOptions. Asks the user to select one of the options.
        /// </summary>
        /// <param name="question">The question to ask the user.</param>
        /// <param name="options">The possible options for the user to select from.</param>
        /// <returns>The number of the selected SelectionOption</returns>
        /// <see cref="SelectionOption"></see>
        public static int CreateQuestion(string question, List<SelectionOption> options) {
            ConsoleWrite("", "");
            ConsoleWrite(question);

            foreach (SelectionOption option in options) {
                ConsoleWrite("", option.Number + ": " + option.Text);
            }

            int result = -1;
            while (true) {
                Console.Write(" " + Strings.SelectionSelect + " (" + options.Min(option => option.Number) + "-" + options.Max(option => option.Number) + ") > ");
                try {
                    result = Convert.ToInt32(ConsoleRead());
                }
                catch (FormatException) {
                    ConsoleWriteError(Strings.SelectionNotNumber);
                    continue;
                }
                if (!(result >= options.Min(option => option.Number) && result <= options.Max(option => option.Number))) {
                    ConsoleWriteError(Strings.SelectionNotInRange);
                    continue;
                }
                break;
            }

            ConsoleWrite("", "");

            return result;
        }

        /// <summary>
        /// Clears the Console.
        /// </summary>
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

        /// <summary>
        /// Changes the font of the Console to the specified font and optionally sets the font size.
        /// </summary>
        /// <param name="font">System font to change to</param>
        /// <param name="fontSize">Size of the font</param>
        /// <returns></returns>
        public static FontInfo[] ConsoleFont(string font, short fontSize = 0) {
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
