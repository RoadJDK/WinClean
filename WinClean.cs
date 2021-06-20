using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Drawing;
using Microsoft.Win32;


namespace WinClean {
    using SLID = Guid;

    struct SelectionOption {
        public int number { get; set; }
        public string text { get; set; }
        public string description { get; set; }
        public SelectionOption(int _number, string _text, string _description) {
            number = _number;
            text = _text;
            description = _description;
        }
    }
    
    public class WinClean {
        static void Main(string[] args) {
            Console.Clear();

            // Setting the font
            ConsoleTitle("Setting font...");
            if (IsFontInstalled("CascadiaCode Nerd Font")) {
                ConsoleHelper.SetCurrentFont("CascadiaCode Nerd Font", 24);
            }
            else {
                ConsoleHelper.SetCurrentFont("Consolas", 24);
            }
            Thread.Sleep(1000);

            Console.Clear();

            //Welcome Message
            ConsoleTitle("Welcome");
            ConsoleWrite("Welcome to WinClean! WinClean is used to clean and setup your new Windows installation.", ConsoleColor.White);
            ConsoleWrite("---------------------------------------------------------------------------------------", ConsoleColor.White);
            Thread.Sleep(3000);

            Console.Clear();

            // Check for Windows 10
            ConsoleTitle("Check for Windows 10");
            if (!IsWindows10()) {
                ConsoleWriteError("Your windows installation is not supported! WinClean is only designed for Windows 10!");
                EnterToContinue();
                Environment.Exit(-1);
            }

            Console.Clear();

            // Part 1 - Windows Activation
            ConsoleTitle("Windows Activation");
            ConsoleWrite("===== Part 1 - Windows Activation =====", ConsoleColor.White);
            ConsoleWrite("", ConsoleColor.White);
            ConsoleWrite("[WinClean] First, let's start by activating windows!", ConsoleColor.White);
            if (IsGenuineWindows()) {
                var userHasKeySelection = CreateSelection("Do you already have a windows activation key?", new List<SelectionOption>() {
                    new SelectionOption(1, "Yes, I do!", ""),
                    new SelectionOption(2, "No, I don't", "")
                });
                switch (userHasKeySelection) {
                    case 1:
                        ConsoleWrite("[WinClean] Great! Please enter your windows activation key below:", ConsoleColor.White);
                        string activationKey = ConsoleRead("Activation Key");
                        ConsoleWrite("[WinClean] Your activation key is: " + activationKey, ConsoleColor.White);
                        break;

                    case 2:
                        break;

                    default:
                        ConsoleWriteError("Something went wrong...");
                        break;
                }
            } else {
                ConsoleWrite("[WinClean] Windows is already activated! Skipping activation...", ConsoleColor.White);
            }

            // End
            EnterToContinue();
            Console.Clear();
            Environment.Exit(0);
        }

        private static void ConsoleWrite(string text, ConsoleColor color) {
            Console.ForegroundColor = color;
            Console.WriteLine(" " + text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void ConsoleWriteError(string text) {
            ConsoleWrite("[ERROR] " + text, ConsoleColor.Red);
        }

        private static string ConsoleRead() {
            Console.Write(" ");
            return Console.ReadLine();
        }

        private static string ConsoleRead(string text) {
            Console.Write(" " + text + ": ");
            return Console.ReadLine();
        }

        private static ConsoleKey ConsoleReadKey() {
            Console.Write(" ");
            return Console.ReadKey().Key;
        }

        private static void ConsoleTitle(string title) {
            if (title != "") {
                Console.Title = "WinClean - " + title;
            } else {
                Console.Title = "WinClean";
            }
        }

        private static void EnterToContinue() {
            Thread.Sleep(250);
            ConsoleWrite("[WinClean] Press Enter to continue...", ConsoleColor.White);
            var key = ConsoleReadKey();
            while (key != ConsoleKey.Enter) {
                key = ConsoleReadKey();
            }
        }

        private static bool IsFontInstalled(string fontName) {
            using (var testFont = new Font(fontName, 8)) {
                return 0 == string.Compare(
                  fontName,
                  testFont.Name,
                  StringComparison.InvariantCultureIgnoreCase);
            }
        }

        private static int CreateSelection(string question, List<SelectionOption> options) {
            ConsoleWrite("", ConsoleColor.White);
            ConsoleWrite("Q: " + question, ConsoleColor.White);
            foreach (SelectionOption option in options) {
                string line = option.number + ". - " + option.text;
                if (option.description.Length > 0) {
                    line += " - " + option.description;
                }
                ConsoleWrite(line, ConsoleColor.White);
            }
            int selectionNum = -1;
            //TODO: Make this good
            while (!((options.Any() ? options.Min(option => option.number) : 0) <= selectionNum && selectionNum <= (options.Any() ? options.Max(option => option.number) : 0))) {
                bool success = true;
                try {
                    selectionNum = Convert.ToInt32(ConsoleRead("Select (" + (options.Any() ? options.Min(option => option.number) : 0) + "-" + (options.Any() ? options.Max(option => option.number) : 0) + ")"));
                } catch (FormatException fe) {
                    ConsoleWrite("Selection is invalid!", ConsoleColor.Red);
                    success = false;
                } finally {
                    if (success) {
                        if (!((options.Any() ? options.Min(option => option.number) : 0) <= selectionNum && selectionNum <= (options.Any() ? options.Max(option => option.number) : 0))) {
                            ConsoleWrite("Selection is out of range!", ConsoleColor.Red);
                        }
                    }
                }
            }
            Console.WriteLine();

            return selectionNum;
        }

        private static bool IsWindows10() {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            string productName = (string)reg.GetValue("ProductName");
            return productName.StartsWith("Windows 10");
        }

        public enum SL_GENUINE_STATE {
            SL_GEN_STATE_IS_GENUINE = 0,
            SL_GEN_STATE_INVALID_LICENSE = 1,
            SL_GEN_STATE_TAMPERED = 2,
            SL_GEN_STATE_LAST = 3
        }

        [DllImportAttribute("Slwga.dll", EntryPoint = "SLIsGenuineLocal", CharSet = CharSet.None, ExactSpelling = false, SetLastError = false, PreserveSig = true, CallingConvention = CallingConvention.Winapi, BestFitMapping = false, ThrowOnUnmappableChar = false)]
        [PreserveSigAttribute()]
        internal static extern uint SLIsGenuineLocal(ref SLID slid, [In, Out] ref SL_GENUINE_STATE genuineState, IntPtr val3);

        public static bool IsGenuineWindows() {
            bool _IsGenuineWindows = false;
            Guid ApplicationID = new Guid("55c92734-d682-4d71-983e-d6ec3f16059f"); //Application ID GUID http://technet.microsoft.com/en-us/library/dd772270.aspx
            SLID windowsSlid = (Guid)ApplicationID;
            try {
                SL_GENUINE_STATE genuineState = SL_GENUINE_STATE.SL_GEN_STATE_LAST;
                uint ResultInt = SLIsGenuineLocal(ref windowsSlid, ref genuineState, IntPtr.Zero);
                if (ResultInt == 0) {
                    _IsGenuineWindows = (genuineState == SL_GENUINE_STATE.SL_GEN_STATE_IS_GENUINE);
                } else {
                    Console.WriteLine("Error getting information {0}", ResultInt.ToString());
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return _IsGenuineWindows;
        }
    }
}