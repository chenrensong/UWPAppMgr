using System.Runtime.InteropServices;
using System.Security;

namespace UWPAppMgr.Utils
{
    internal static class Pinvoke
	{
		[SuppressUnmanagedCodeSecurity]
		private static class NativeMethods
		{
			[DllImport("AppXDeploymentClient.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern long AppxRequestRemovePackageForUser(string packageFullName, string userStringSid);
		}

		internal static long AppxRequestRemovePackageForUser(string packageFullName, string userStringSid)
		{
			return NativeMethods.AppxRequestRemovePackageForUser(packageFullName, userStringSid);
		}
	}
}
