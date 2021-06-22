using WinClean.resources;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using static WinClean.ConsoleHelper;
using static WinClean.WinHelper;

namespace WinClean {
    /// <summary>
    /// The main method for the WinClean program.
    /// 
    /// For Version: v0.0.1
    /// Author: Akjo03
    /// </summary>
    public class WinClean {

        public static string Version = "v0.0.1-alpha2";

        public static void Main(string[] args) {
            ConsoleClear();

            // === Setting font ===
            ConsoleTitle("Setting font...");
            ConsoleFont("Consolas", 24);

            // === Setting default language ===
            ConsoleLang("en-us");

            // === Part 0 - Checking for Windows 10 ===
            ConsoleTitle("Checking for Windows 10");
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                ConsoleWriteError(Strings.OsNotWindows);
                EnterToContinue(Strings.EnterToExit);
                ConsoleExit(-1);
            } else {
                if (!IsWindows10()) {
                    ConsoleWriteError(Strings.OsNotWindows10);
                    EnterToContinue(Strings.EnterToExit);
                    ConsoleExit(-1);
                }
            }

            // === Setting language ===
            if (!RegistryValueExists("lang")) {
                ConsoleTitle("Language");
                var lang = CreateQuestion("Language / Sprache:", new List<SelectionOption>() {
                    new SelectionOption(1, "English"),
                    new SelectionOption(2, "Deutsch")
                });

                switch (lang) {
                    case 1:
                        ConsoleLang("en-us");
                        RegistryAddValue("lang", "en-us");
                        break;

                    case 2:
                        ConsoleLang("de-de");
                        RegistryAddValue("lang", "de-de");
                        break;

                    default:
                        ConsoleWriteError(Strings.LangaugeSelectionError);
                        break;
                }
            } else {
                ConsoleLang(RegistryReadValue("lang"));
                ConsoleWrite(Strings.LanguageAlreadySet);
                ConsoleSleep(3000);
            }

            ConsoleClear();

            // === Welcome to WinClean ===
            ConsoleTitle(Strings.TitleWelcome);
            ConsoleWrite(Strings.WelcomeMessage);
            ConsoleSleep(4000);
            
            ConsoleWrite("", "");

            // === Part 1 - Activating Windows ===


            // === Part 2 - Updating Windows ===


            // === Part 3 - Cleaning Windows from Bloatware ===


            // === Part 4 - Setting System Settings ===


            // === Part 5 - Setting Security Settings ===


            // === Part 6 - Installing Basic Software ===


            // === Part 7 - Personalizing Windows ===


            // === Part 8 - Installing Additional Software ===


            // === Part 9 - Checking System Integrity ===


            // === Part 10 - Finishing WinClean ===
        }
    }
}