using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Unity.SharePoint.Configuration;

namespace Unity.SharePoint
{
    public static class SpContextFactory
    {
        public static IUnityConfiguration GetUnityConfig(this SPContext self)
        {
            if (self == null || self.Site == null)
                return null;

            return GetUnityConfig(new Uri(self.Site.Url));
        }

        public static IUnityConfiguration GetUnityConfig(Uri uri)
        {
            var wap = SPWebApplication.Lookup(uri);

            return wap.GetChild<UnityConfiguration>();
        }

        public static IUnityConfiguration CreateUnityConfig(this SPWebApplication wap)
        {
            return wap.GetChild<UnityConfiguration>() ?? UnityConfiguration.Create(wap);
        }

        public static void DeleteUnityConfig(this SPWebApplication wap)
        {
            var cfg = wap.GetChild<UnityConfiguration>();

            if (cfg != null)
            {
                cfg.Delete();
            }
        }

    }
}
