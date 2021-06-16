﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
            if (!IsWindows10()) {
                ConsoleWrite("ERROR: This Windows version is not supported! WinClean is only designed for Windows 10.", ConsoleColor.Red);
                EnterToContinue();
                Environment.Exit(1);
            } else {
                ConsoleWrite("Welcome to WinClean! WinClean is used to clean and setup your new Windows installation.", ConsoleColor.White);

                // Windows Activation
                if (IsGenuineWindows()) {
                    ConsoleWrite("This is a genuine/activated windows copy!", ConsoleColor.White);
                } else {
                    ConsoleWrite("This is NOT a genuine/activated windows copy!", ConsoleColor.White);
                }
                CreateSelection("",
                    new List<SelectionOption>() {
                        new SelectionOption(1, "", "")
                    });
            }
            Console.ReadLine();
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

        private static void ConsoleWrite(string text, ConsoleColor color) {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void EnterToContinue() {
            ConsoleWrite("Press Enter to continue...", ConsoleColor.White);
            var key = Console.ReadKey().Key;
            while (key != ConsoleKey.Enter) {
                key = Console.ReadKey().Key;
            }
        }

        private static void CreateSelection(string question, List<SelectionOption> options) {

        }

        private static bool IsWindows10() {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            string productName = (string)reg.GetValue("ProductName");
            return productName.StartsWith("Windows 10");
        }

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