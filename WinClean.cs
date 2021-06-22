using WinClean.resources;

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using static WinClean.ConsoleHelper;
using static WinClean.WinHelper;

namespace WinClean {
    /// <summary>
    /// The main method for the WinClean program.
    /// 
    /// For Version: v0.1.0
    /// Author: Akjo03
    /// </summary>
    public class WinClean {

        public static string Version = "v0.1.0-iss11";

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
            Part1();

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

        private static void Part1() {
            ConsoleClear();
            ConsoleWrite(Strings.Part1TitleBar);

            if (!IsWindowsActivated()) {
                ConsoleWrite(Strings.WindowsNotYetActivated);
                ConsoleSleep(1000);
                var hasActivationKey = CreateQuestion(Strings.HasActivationKeySelection, new List<SelectionOption>() {
                    new SelectionOption(1, Strings.HasActivationKeySelectionAnswerY),
                    new SelectionOption(2, Strings.HasActivationKeySelectionAnswerN)
                });

                switch (hasActivationKey) {
                    case 1:
                        ConsoleWrite(Strings.EnterActivationKey);
                        var activationKey = ConsoleRead(Strings.ActivationKey);
                        var activationKeyTries = 0;
                        while (!Regex.IsMatch(activationKey, "^([A-Z0-9]{5}-){4}[A-Z0-9]{5}$") && ++activationKeyTries < 3) {
                            ConsoleWriteError(Strings.EnterActivationKeyError);
                            activationKey = ConsoleRead(Strings.ActivationKey);
                        }
                        if (activationKeyTries >= 3) {
                            ConsoleWriteError(Strings.EnterActivationKeyError_TooManyTries);
                            EnterToContinue();
                            return;
                        }
                        ConsoleWrite("Test0");
                        try {
                            string activationComplete = ConsoleRunWithChecks(@"cscript C:\Windows\System32\slmgr.vbs //T:60 /ipk " + activationKey.Trim(),
                            new List<string>() {
                                "0x8004FE21", // This computer is not running genuine Windows.
                                "0x80070005", // Access denied. The requested action requires elevated privileges.
                                "0x8007007b", // The product key you entered didn't work. Try again.
                                "0xC004B100", // Computer could not be activated.
                                "0xC004C001", // Product key is invalid.
                                "0xC004C003", // Product key is blocked.
                                "0xC004F050", // Product key is invalid.
                                "0xC004F051", // Product key is blocked.
                            }, false, true);

                            ConsoleWrite("Test1");

                            switch (activationComplete.ToUpper()) {
                                case null:
                                    ConsoleWrite("Test2");
                                    ConsoleWrite("Activation Key has been succesfully installed!");
                                    return;

                                case "0x8004FE21":
                                    ConsoleWrite("Test3");
                                    ConsoleWriteError(Strings.ActivationKeyInstallError_NotGenuineWindows);
                                    ConsoleWriteWarn(Strings.ActivationKeyInstallError_NotActivatingWindows);
                                    EnterToContinue();
                                    return;

                                case "0x80070005":
                                    ConsoleWrite("Test4");
                                    ConsoleWriteError(Strings.ActivationKeyInstallError_AccessDenied);
                                    ConsoleWriteWarn(Strings.ActivationKeyInstallError_NotActivatingWindows);
                                    EnterToContinue();
                                    return;

                                case "0x8007007B":
                                    ConsoleWrite("Test5");
                                    ConsoleWriteError(Strings.ActivationKeyInstallError_ProductKeyNotWorking);
                                    EnterToContinue();
                                    Part1();
                                    break;

                                case "0xC004B100":
                                    ConsoleWrite("Test6");
                                    ConsoleWriteError(Strings.ActivationKeyInstallError_ComputerActivationFailed);
                                    ConsoleWriteWarn(Strings.ActivationKeyInstallError_NotActivatingWindows);
                                    EnterToContinue();
                                    return;

                                case "0xC004C001":
                                    ConsoleWrite("Test7");
                                    ConsoleWriteError(Strings.ActivationKeyInstallError_ProductKeyInvalid);
                                    EnterToContinue();
                                    Part1();
                                    break;

                                case "0xC004C003":
                                    ConsoleWrite("Test8");
                                    ConsoleWriteError(Strings.ActivationKeyInstallError_ProductKeyBlocked);
                                    EnterToContinue();
                                    Part1();
                                    break;

                                case "0xC004F050":
                                    ConsoleWrite("Test9");
                                    ConsoleWriteError(Strings.ActivationKeyInstallError_ProductKeyInvalid);
                                    EnterToContinue();
                                    Part1();
                                    break;

                                case "0xC004F051":
                                    ConsoleWrite("Test10");
                                    ConsoleWriteError(Strings.ActivationKeyInstallError_ProductKeyBlocked);
                                    EnterToContinue();
                                    Part1();
                                    break;

                                default:
                                    ConsoleWrite("Test11");
                                    ConsoleWriteError(Strings.ActivationKeyInstallError_Other);
                                    ConsoleWriteWarn(Strings.ActivationKeyInstallError_NotActivatingWindows);
                                    EnterToContinue();
                                    return;
                            }
                        } finally {
                            ConsoleWrite("Test12");
                            ConsoleWriteError(Strings.SomethingWentWrong);
                            EnterToContinue(Strings.EnterToExit);
                            ConsoleExit(-1);
                        }
                        break;

                    case 2:
                        EnterToContinue(Strings.EnterToExit);
                        break;

                    default:
                        ConsoleWrite("Test13");
                        ConsoleWriteError(Strings.HasActivationKeySelectionError);
                        return;
                }
            } else {
                ConsoleWrite(Strings.WindowsAlreadyActivated);
                ConsoleSleep(1000);
                return;
            }
        }
    }
}