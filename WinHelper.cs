using Microsoft.Win32;

using System;
using System.Security.AccessControl;

namespace WinClean {
    public class WinHelper {

        public static string environmentUser = Environment.UserDomainName + "\\" + Environment.UserName;

        public static bool IsWindows10() {
            var reg = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");

            string productName = (string)reg.GetValue("ProductName");

            return productName.StartsWith("Windows 10");
        }

        // === Registry Functions ===

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

        public static void RegistryUpdateValue(string name, string newValue) {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\WinClean", RegistryKeyPermissionCheck.ReadWriteSubTree)) {
                if (key != null) {
                    key.SetValue(name, newValue);
                }
            }
        }

        public static string RegistryReadValue(string name) {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\WinClean", RegistryKeyPermissionCheck.ReadWriteSubTree)) {
                if (key != null) {
                    return key.GetValue(name).ToString();
                } else {
                    return null;
                }
            }
        }

        public static void RegistryDeleteValue(string name) {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\WinClean", RegistryKeyPermissionCheck.ReadWriteSubTree)) {
                if (key != null) {
                    key.DeleteValue(name);
                }
            }
        }
    }
}
