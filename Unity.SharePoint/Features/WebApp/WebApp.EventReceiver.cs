using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Unity.SharePoint.Features.WebApp
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("014d3118-844c-4cc8-aa51-27b940f893a5")]
    public class WebAppEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var wap = properties.Feature.Parent as SPWebApplication;
            if (wap == null)
                throw new ArgumentException("Missing parent SPWebApplication!", "properties");

            wap.CreateUnityConfig();

            // add the ihttpmodule
            wap.WebConfigModifications.Insert(0, Mod);

            // Commit modification additions to the specified web application
            wap.Update();

            // Push modifications through the farm
            SPFarm.Local.Services.GetValue<SPWebService>(wap.Parent.Id).ApplyWebConfigModifications();
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            var wap = properties.Feature.Parent as SPWebApplication;
            if (wap == null)
                throw new ArgumentException("Missing parent SPWebApplication!", "properties");


            wap.DeleteUnityConfig();
            
            // remove the ihttpmodule
            foreach (var mod in wap.WebConfigModifications.Where(mod => mod.Owner == Mod.Owner && mod.Name == Mod.Name))
            {
                wap.WebConfigModifications.Remove(mod);
                break;
            }

            // Commit modification additions to the specified web application
            wap.Update();

            // Push modifications through the farm
            SPFarm.Local.Services.GetValue<SPWebService>(wap.Parent.Id).ApplyWebConfigModifications();
        }


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //    var wap = properties.Feature.Parent as SPWebApplication;
        //    if (wap == null)
        //        throw new ArgumentException("Missing parent SPWebApplication!", "properties");

        //    wap.CreateUnityConfig();
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //    var wap = properties.Feature.Parent as SPWebApplication;
        //    if (wap == null)
        //        throw new ArgumentException("Missing parent SPWebApplication!", "properties");

        //    wap.DeleteUnityConfig();
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}

        static WebAppEventReceiver()
        {
            Type = typeof(UnityHttpHandler).AssemblyQualifiedName;
            Mod = new SPWebConfigModification
                {
                    Path = "configuration/system.webServer/modules",
                    Name = String.Format("add[@name='{0}'][@type='{1}']", Name, Type),
                    Sequence = 0,
                    Owner = Name,
                    Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode,
                    Value = String.Format("<add name='{0}' type='{1}' />", Name, Type)
                };
        }


        public static readonly SPWebConfigModification Mod;
        private const string Name = "Unity.SharePoint";
        private static readonly string Type;
    }
}
