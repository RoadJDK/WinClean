using System;

namespace WinClean {
    /// <summary>
    /// The main method for the WinClean program.
    /// 
    /// For Version: v0.0.1
    /// Author: Akjo03
    /// </summary>
    public class WinClean {

        public static string Version = "v0.0.1-alpha1";

        public static void Main(string[] args) {
            ConsoleHelper.ConsoleWrite("Hello World!");
            ConsoleHelper.EnterToContinue();
        }
    }
}