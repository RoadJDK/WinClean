using Microsoft.Win32;

using System;
using System.Management;
using System.Security.AccessControl;
using System.Runtime.InteropServices;

namespace WinClean {
    using SLID = Guid;

    public class WinHelper {
        public enum SL_GENUINE_STATE {
            SL_GEN_STATE_IS_GENUINE = 0,
            SL_GEN_STATE_INVALID_LICENSE = 1,
            SL_GEN_STATE_TAMPERED = 2,
            SL_GEN_STATE_OFFLINE = 3,
            SL_GEN_STATE_LAST = 4
        }

        [DllImportAttribute("Slwga.dll", EntryPoint = "SLIsGenuineLocal", CharSet = CharSet.None, ExactSpelling = false, SetLastError = false, PreserveSig = true, CallingConvention = CallingConvention.Winapi, BestFitMapping = false, ThrowOnUnmappableChar = false)]
        [PreserveSigAttribute()]
        internal static extern uint SLIsGenuineLocal(ref SLID slid, [In, Out] ref SL_GENUINE_STATE genuineState, IntPtr val3);

        public static string environmentUser = Environment.UserDomainName + "\\" + Environment.UserName;

        /// <summary>
        /// Checks if this program is running on Windows 10.
        /// </summary>
        /// <returns>If this program is running on Windows 10.</returns>
        public static bool IsWindows10() {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

            string productName = (string)reg.GetValue("ProductName");

            return productName.StartsWith("Windows 10");
        }

        public static bool IsWindowsActivated() {
            bool _IsGenuineWindows = false;
            Guid ApplicationID = new Guid("55c92734-d682-4d71-983e-d6ec3f16059f"); //Application ID GUID http://technet.microsoft.com/en-us/library/dd772270.aspx
            SLID windowsSlid = (Guid)ApplicationID;
            try {
                SL_GENUINE_STATE genuineState = SL_GENUINE_STATE.SL_GEN_STATE_LAST;
                uint ResultInt = SLIsGenuineLocal(ref windowsSlid, ref genuineState, IntPtr.Zero);
                if (ResultInt == 0) {
                    _IsGenuineWindows = (genuineState == SL_GENUINE_STATE.SL_GEN_STATE_IS_GENUINE);
                }
                else {
                    Console.WriteLine("Error getting information {0}", ResultInt.ToString());
                }

            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            return _IsGenuineWindows;
        }

        // === Registry Functions ===

        /// <summary>
        /// Adds a new value to the WinClean registry key.
        /// </summary>
        /// <param name="name">Name of the new value</param>
        /// <param name="value">The value itself</param>
        public static void RegistryAddValue(string name, string value) {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\WinClean", RegistryKeyPermissionCheck.ReadWriteSubTree)) {
                RegistrySecurity rs = key.GetAccessControl();

            rs.AddAccessRule(new RegistryAccessRule(environmentUser,
                RegistryRights.SetValue |
                RegistryRights.ReadKey |
                RegistryRights.WriteKey |
                RegistryRights.Delete |
                RegistryRights.CreateSubKey |
                RegistryRights.QueryValues |
                RegistryRights.Notify |
                RegistryRights.ReadPermissions |
                RegistryRights.TakeOwnership,
                AccessControlType.Allow));

            key.SetValue(name, value);
            }
        }

        /// <summary>
        /// Updates the specified value inside the WinClean registry key to a new value
        /// </summary>
        /// <param name="name">The name of the value to change</param>
        /// <param name="newValue">The new value</param>
        public static void RegistryUpdateValue(string name, string newValue) {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\WinClean", RegistryKeyPermissionCheck.ReadWriteSubTree)) {
                if (key != null) {
                    key.SetValue(name, newValue);
                }
            }
        }

        /// <summary>
        /// Reads the specified value from the WinClean registry key.
        /// </summary>
        /// <param name="name">The name of the value to read from.</param>
        /// <returns>The value</returns>
        public static string RegistryReadValue(string name) {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\WinClean", RegistryKeyPermissionCheck.ReadWriteSubTree)) {
                if (key != null) {
                    return key.GetValue(name).ToString();
                } else {
                    return null;
                }
            }
        }

        /// <summary>
        /// Deletes the specified value from the WinClean registry key.
        /// </summary>
        /// <param name="name">The name of the value to delete.</param>
        public static void RegistryDeleteValue(string name) {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\WinClean", RegistryKeyPermissionCheck.ReadWriteSubTree)) {
                if (key != null) {
                    key.DeleteValue(name);
                }
            }
        }

        /// <summary>
        /// Checks if a value with the specified name exists in the WinClean registry key.
        /// </summary>
        /// <param name="name">The name of the value</param>
        /// <returns>If the value exists.</returns>
        public static bool RegistryValueExists(string name) {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\WinClean", RegistryKeyPermissionCheck.ReadWriteSubTree)) {
                if (key != null) {
                    if (key.GetValue(name) != null) {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
