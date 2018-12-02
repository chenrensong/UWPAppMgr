namespace UWPAppMgr.Model
{
    public class AppxPackageProperties
    {
        public bool IsBundle
        {
            get;
            set;
        }

        public bool IsDevelopmentMode
        {
            get;
            set;
        }

        public bool IsFramework
        {
            get;
            set;
        }

        public bool IsOptional
        {
            get;
            set;
        }

        public bool IsResourcePackage
        {
            get;
            set;
        }


        public AppxPackageProperties(bool isBundle = false, bool isDevelopmentMode = false, 
            bool isFramework = false, bool isOptional = false, bool isResourcePackage = false)
        {
            IsBundle = isBundle;
            IsDevelopmentMode = isDevelopmentMode;
            IsFramework = isFramework;
            IsOptional = isOptional;
            IsResourcePackage = isResourcePackage;
        }
    }
}
