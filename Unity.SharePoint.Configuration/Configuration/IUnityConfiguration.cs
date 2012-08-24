using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace Unity.SharePoint.Configuration
{
    public interface IUnityConfiguration : IList<UnityConfigurationFile>
    {
        void Update();
        void Configure(IUnityContainer container);
    }
}
