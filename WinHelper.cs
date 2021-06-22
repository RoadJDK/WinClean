using Microsoft.Win32;

using System;
using System.Security.AccessControl;

namespace WinClean {
    public class WinHelper {

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
