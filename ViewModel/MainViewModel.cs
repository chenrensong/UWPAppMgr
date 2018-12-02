using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Principal;
using System.Windows.Input;
using UWPAppMgr.Model;
using UWPAppMgr.Utils;

namespace UWPAppMgr.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        public ObservableCollection<AppxPackage> _appxPackages;
        public string _systemInfo;
        public ObservableCollection<AppxPackage> AppxPackages
        {
            get
            {
                return _appxPackages;
            }
            set
            {
                _appxPackages = value;
                base.RaisePropertyChanged<ObservableCollection<AppxPackage>>(() => AppxPackages);
            }
        }

        public string SystemInfo
        {
            get
            {
                return _systemInfo;
            }
            set
            {
                _systemInfo = value;
                base.RaisePropertyChanged<string>(() => SystemInfo);
            }
        }
        public ICommand SelectAllCmd
        {
            get; set;
        }

        public ICommand RemovePackageCmd
        {
            get; set;
        }

        public ICommand RefreshCmd
        {
            get; set;
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                return;
            }
            RefreshCmd = new RelayCommand(() =>
            {
                this.AppxPackages.Clear();
                this.Load();
            });

            SelectAllCmd = new RelayCommand(() =>
            {
                foreach (var item in this.AppxPackages)
                {
                    item.IsCheck = !item.IsCheck;
                }
            });

            RemovePackageCmd = new RelayCommand(() =>
            {
                var removedList = new List<AppxPackage>();
                foreach (var item in this.AppxPackages)
                {
                    if (item.IsCheck.HasValue && item.IsCheck.Value)
                    {
                        try
                        {
                            PackageManagerHelper.RemovePackage(item.FullName);
                            if (string.IsNullOrEmpty(item.UserName))//非当前用户
                            {
                                //TODO
                            }
                            removedList.Add(item);
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
                foreach (var item in removedList)
                {
                    this.AppxPackages.Remove(item);
                }
            });
        }

        public void Load()
        {
            var list = PackageManagerHelper.GetPackagesForAllUserWithPackageTypesOnlyRemovable(Windows.Management.Deployment.PackageTypes.Main);
            this.AppxPackages = new ObservableCollection<AppxPackage>(list);
            SystemInfo = $"Installed packages: {AppxPackages.Count} | User: {GetUserName()} | Build: {GetBuildNumber()}";
        }

        private static string GetBuildNumber()
        {
            string str = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "ReleaseId", string.Empty).ToString();
            return Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "ProductName", string.Empty).ToString() + " " + str;
        }

        private static string GetUserName()
        {
            return WindowsIdentity.GetCurrent().Name;
        }
    }
}