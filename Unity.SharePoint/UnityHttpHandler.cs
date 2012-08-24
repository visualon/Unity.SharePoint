using System;
using System.Web;
using Microsoft.Practices.Unity;

namespace Unity.SharePoint
{
    class UnityHttpHandler : IHttpModule
    {
        private const string UnityContainer = "UnityContainer";
        private IUnityContainer _container;

        public void Init(HttpApplication context)
        {
            _container = new UnityContainer();

            context.BeginRequest += ContextOnBeginRequest;
        }

        private void ContextOnBeginRequest(object sender, EventArgs eventArgs)
        {
            var context = sender as HttpApplication;
            if (context == null)
                throw new ArgumentException("Missing parent HttpApplication!", "sender");
            if (!context.Context.Items.Contains(UnityContainer))
                context.Context.Items[UnityContainer] = _container;
        }

        public void Dispose()
        {
            _container.Dispose();
            _container = null;
        }
    }
}
