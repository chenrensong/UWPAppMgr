using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;

namespace UWPAppMgr
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            CheckAdministrator();
            StartupUri = new Uri("MainWindow.xaml", UriKind.RelativeOrAbsolute);
        }

        private void CheckAdministrator()
        {
            var userSid = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(userSid);

            bool runAsAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (!runAsAdmin)
            {
                var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";
                try
                {
                    Process.Start(processInfo);
                }
                catch (Exception ex)
                {
                    //None
                }
                Environment.Exit(0);
            }
        }

    }
}
