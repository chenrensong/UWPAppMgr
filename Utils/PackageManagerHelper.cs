using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Xml;
using UWPAppMgr.Model;
using Windows.ApplicationModel;
using Windows.Management.Deployment;

namespace UWPAppMgr.Utils
{
    public static class PackageManagerHelper
    {
        [DllImport("shlwapi.dll", BestFitMapping = false, CharSet = CharSet.Unicode, ExactSpelling = true, ThrowOnUnmappableChar = true)]
        public static extern int SHLoadIndirectString(string pszSource, StringBuilder pszOutBuf, int cchOutBuf, IntPtr ppvReserved);

        private static string GetDisplayName(string installPath, string queryName, string appName)
        {
            if (queryName.Contains(":"))
            {
                StringBuilder stringBuilder = new StringBuilder(1024);
                string arg = queryName.ToLower().Contains(appName.ToLower()) ? queryName : $"ms-resource://{appName}/resources/{queryName.Split(':')[1].TrimStart('/')}";
                if (SHLoadIndirectString(string.Format("@{{{0}? {1}}}", Path.Combine(installPath, "resources.pri"), arg), stringBuilder, stringBuilder.Capacity, IntPtr.Zero) == 0)
                {
                    return stringBuilder.ToString();
                }
                arg = $"ms-resource://{appName}/{queryName.Split(':')[1].TrimStart('/')}";
                if (SHLoadIndirectString(string.Format("@{{{0}? {1}}}", Path.Combine(installPath, "resources.pri"), arg), stringBuilder, stringBuilder.Capacity, IntPtr.Zero) == 0)
                {
                    return stringBuilder.ToString();
                }
                return string.Empty;
            }
            if (string.IsNullOrEmpty(queryName))
            {
                return appName;
            }
            return queryName;
        }

        public static IEnumerable<AppxPackage> GetPackagesForUser(string userSid)
        {
            List<AppxPackage> list = new List<AppxPackage>();
            AddPackagesToList(new PackageManager().FindPackagesForUser(userSid), list);
            return list;
        }

        public static IEnumerable<AppxPackage> GetPackagesForUserWithPackageTypes(string userSid, PackageTypes packageType)
        {
            List<AppxPackage> list = new List<AppxPackage>();
            AddPackagesToList(new PackageManager().FindPackagesForUserWithPackageTypes(userSid, packageType), list);
            return list;
        }


        public static IEnumerable<AppxPackage> GetPackagesForAllUserWithPackageTypesOnlyRemovable(PackageTypes packageType)
        {
            var userName = WindowsIdentity.GetCurrent().Name;
            string userSid = WindowsIdentity.GetCurrent().User.ToString();
            var listForCurrentUser = GetPackagesForUserWithPackageTypesOnlyRemovable(userSid, packageType);
            var listForAllUser = GetPackagesWithPackageTypesOnlyRemovable(packageType);
            foreach (var item in listForAllUser)
            {
                if (listForCurrentUser.Any(m => m.FullName == item.FullName))
                {
                    item.UserName = userName;
                }
            }
            return listForAllUser;
        }

        //Get-AppxPackage -allusers -name *3718* | select Name,PackageFamilyName,PackageFullName
        public static IEnumerable<AppxPackage> GetPackagesForUserWithPackageTypesOnlyRemovable(string userSid, PackageTypes packageType)
        {
            List<AppxPackage> list = new List<AppxPackage>();
            AddPackagesToList(new PackageManager().FindPackagesForUserWithPackageTypes(userSid, packageType).Where(delegate (Package item)
            {
                PackageSignatureKind signatureKind = item.SignatureKind;
                return !string.Equals(signatureKind.ToString(), "System");
            }), list);
            return list;
        }

        public static IEnumerable<AppxPackage> GetPackagesWithPackageTypesOnlyRemovable(PackageTypes packageType)
        {
            List<AppxPackage> list = new List<AppxPackage>();
            AddPackagesToList(new PackageManager().FindPackagesWithPackageTypes(packageType).Where(delegate (Package item)
            {
                PackageSignatureKind signatureKind = item.SignatureKind;
                return !string.Equals(signatureKind.ToString(), "System");
            }), list);
            return list;
        }

        private static void AddPackagesToList(IEnumerable<Package> packages, ICollection<AppxPackage> appxPackages)
        {
            foreach (Package package in packages)
            {
                try
                {
                    appxPackages.Add(new AppxPackage(GetDisplayName(package.InstalledLocation.Path,
                        GetManifestName(package.InstalledLocation.Path), package.Id.Name),
                        package.Id.Name, package.Id.FamilyName, package.Id.FullName,
                        package.InstalledLocation.Path.ToString(),
                        package.Id.Version, package.Id.Publisher, package.Id.PublisherId,
                        new AppxPackageProperties(package.IsBundle, package.IsDevelopmentMode, package.IsFramework,
                        package.IsOptional, package.IsResourcePackage)));
                }
                catch (Exception ex)
                {

                }
            }
        }

        public static void RemovePackages(IEnumerable<string> packageFullNameList)
        {
            PackageManager packageManager = new PackageManager();
            foreach (string packageFullName in packageFullNameList)
            {
                RemovePackageInternal(packageFullName, packageManager);
            }
        }

        public static void RemovePackages(IEnumerable<string> packageFullNameList, string userSid)
        {
            foreach (string packageFullName in packageFullNameList)
            {
                RemovePackageForUser(packageFullName, userSid);
            }
        }

        public static void RemovePackageForUser(string packageFullName, string userSid)
        {
            Pinvoke.AppxRequestRemovePackageForUser(packageFullName, userSid);
        }

        public static void RemovePackage(string packageFullName)
        {
            PackageManager packageManager = new PackageManager();
            RemovePackageInternal(packageFullName, packageManager);
        }

        private static void RemovePackageInternal(string packageFullName, PackageManager packageManager)
        {
            packageManager.RemovePackageAsync(packageFullName);
        }

        public static void RegisterPackages(IEnumerable<string> packageFullNameList)
        {
            PackageManager packageManager = new PackageManager();
            foreach (string packageFullName in packageFullNameList)
            {
                RegisterPackageInternal(packageFullName, packageManager);
            }
        }

        public static void RegisterPackage(string packageFullName)
        {
            PackageManager packageManager = new PackageManager();
            RegisterPackageInternal(packageFullName, packageManager);
        }

        private static void RegisterPackageInternal(string packageFullName, PackageManager packageManager)
        {
            packageManager.RegisterPackageByFullNameAsync(packageFullName, null, 0);
        }

        public static Package FindPackageForUser(string packageFullName, string userSid)
        {
            return new PackageManager().FindPackageForUser(userSid, packageFullName);
        }

        private static string GetManifestName(string path)
        {
            XmlDocument xmlDocument = new XmlDocument();
            string filename = Path.Combine(path, "AppxManifest.xml");
            if (File.Exists(filename))
            {
                xmlDocument.Load(filename);
                XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDocument.NameTable);
                xmlNamespaceManager.AddNamespace("ns", "http://schemas.microsoft.com/appx/manifest/foundation/windows10");
                xmlNamespaceManager.AddNamespace("mp", "http://schemas.microsoft.com/appx/2014/phone/manifest");
                xmlNamespaceManager.AddNamespace("wincap", "http://schemas.microsoft.com/appx/manifest/foundation/windows10/windowscapabilities");
                xmlNamespaceManager.AddNamespace("uap", "http://schemas.microsoft.com/appx/manifest/uap/windows10");
                if (xmlDocument.SelectSingleNode("/ns:Package/ns:Applications/ns:Application/uap:VisualElements", xmlNamespaceManager) != null)
                {
                    return xmlDocument.SelectSingleNode("/ns:Package/ns:Applications/ns:Application/uap:VisualElements/@DisplayName", xmlNamespaceManager).InnerText;
                }
            }
            return string.Empty;
        }
    }
}
