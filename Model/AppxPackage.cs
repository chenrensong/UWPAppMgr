using GalaSoft.MvvmLight;
using System;
using Windows.ApplicationModel;

namespace UWPAppMgr.Model
{
    public class AppxPackage : ObservableObject
    {
        public string AppxName
        {
            get;
            set;
        }

        public string FamilyName
        {
            get;
            set;
        }

        public string FullName
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string InstallLocation
        {
            get;
            set;
        }

        public AppxPackageProperties Properties
        {
            get;
            set;
        }

        public string Publisher
        {
            get;
            set;
        }

        public string PublisherId
        {
            get;
            set;
        }

        public Version Version
        {
            get;
            set;
        }

        public bool? _isCheck = false;
        public bool? IsCheck
        {
            get
            {
                return _isCheck;
            }
            set
            {
                _isCheck = value;
                base.RaisePropertyChanged<bool?>(() => IsCheck);
            }
        }

        public string UserName { get; set; }

        public AppxPackage()
            : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, default(PackageVersion), string.Empty, string.Empty, new AppxPackageProperties())
        {
        }


        public AppxPackage(string appxName, string name, string familyName, string fullName, string installLocation, PackageVersion version, string publisher, string publisherId, AppxPackageProperties properties)
        {
            AppxName = appxName;
            FamilyName = familyName;
            FullName = fullName;
            Name = name;
            InstallLocation = installLocation;
            Properties = properties;
            Publisher = publisher;
            PublisherId = PublisherId;
            Version = new Version(version.Major, version.Minor, version.Build, version.Revision);
        }
    }
}
