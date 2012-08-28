using System;
using System.Web;
using Microsoft.Practices.Unity;
using Microsoft.SharePoint;

namespace Unity.SharePoint
{
    class UnityHttpHandler : IHttpModule
    {
        private const string UnityContainer = "UnityContainer";
        private IUnityContainer _container;

        public void Init(HttpApplication context)
        {
            _container = new UnityContainer();
            var cfg = SPContext.Current.GetUnityConfig();
            if (cfg != null)
                cfg.Configure(_container);

            context.BeginRequest += ContextOnBeginRequest;
            context.Disposed += ContextDisposed;
        }

        static void ContextDisposed(object sender, EventArgs e)
        {
            var context = sender as HttpApplication;
            if (context == null)
                throw new ArgumentException("Missing parent HttpApplication!", "sender");

            if (!context.Context.Items.Contains(UnityContainer)) return;
            var container = context.Context.Items[UnityContainer] as IUnityContainer;
            context.Context.Items.Remove(UnityContainer);
            if (container != null)
                container.Dispose();
        }

        private void ContextOnBeginRequest(object sender, EventArgs eventArgs)
        {
            var context = sender as HttpApplication;
            if (context == null)
                throw new ArgumentException("Missing parent HttpApplication!", "sender");
            if (!context.Context.Items.Contains(UnityContainer))
                context.Context.Items[UnityContainer] = _container.CreateChildContainer();
        }

        public void Dispose()
        {
            _container.Dispose();
            _container = null;
        }
    }
}
