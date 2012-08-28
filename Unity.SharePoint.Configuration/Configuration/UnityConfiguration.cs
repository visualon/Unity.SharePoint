using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.SharePoint.Administration;

namespace Unity.SharePoint.Configuration
{
    internal class UnityConfiguration : SPPersistedObject, IUnityConfiguration
    {
        public static readonly string Key = "UnityConfiguration";

        [Persisted]
        private readonly List<UnityConfigurationFile> _sections = new List<UnityConfigurationFile>();


        public UnityConfiguration()
        {
        }

        private UnityConfiguration(SPPersistedObject parent)
            : base(Key, parent)
        {

        }

        IEnumerator<UnityConfigurationFile> IEnumerable<UnityConfigurationFile>.GetEnumerator()
        {
            return _sections.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _sections.GetEnumerator();
        }

        void ICollection<UnityConfigurationFile>.Add(UnityConfigurationFile item)
        {
            _sections.Add(item);
        }

        void ICollection<UnityConfigurationFile>.Clear()
        {
            _sections.Clear();
        }

        bool ICollection<UnityConfigurationFile>.Contains(UnityConfigurationFile item)
        {
            return _sections.Contains(item);
        }

        void ICollection<UnityConfigurationFile>.CopyTo(UnityConfigurationFile[] array, int arrayIndex)
        {
            _sections.CopyTo(array, arrayIndex);
        }

        bool ICollection<UnityConfigurationFile>.Remove(UnityConfigurationFile item)
        {
            return _sections.Remove(item);
        }

        int ICollection<UnityConfigurationFile>.Count
        {
            get { return _sections.Count; }
        }

        bool ICollection<UnityConfigurationFile>.IsReadOnly
        {
            get { return false; }
        }

        int IList<UnityConfigurationFile>.IndexOf(UnityConfigurationFile item)
        {
            return _sections.IndexOf(item);
        }

        void IList<UnityConfigurationFile>.Insert(int index, UnityConfigurationFile item)
        {
            _sections.Insert(index, item);
        }

        void IList<UnityConfigurationFile>.RemoveAt(int index)
        {
            _sections.RemoveAt(index);
        }

        UnityConfigurationFile IList<UnityConfigurationFile>.this[int index]
        {
            get { return _sections[index]; }
            set { _sections[index] = value; }
        }

        public void Configure(IUnityContainer container)
        {
            foreach (var sec in _sections.OrderBy(s => s.Sequence).ThenBy(s => s.Name).Select(section => section.MapSourcePath())
                .Select(ConfigurationManager.OpenExeConfiguration).Select(cfg => cfg.GetSection("unity")).OfType<UnityConfigurationSection>())
            {
                sec.Configure(container);
            }
        }

        public static IUnityConfiguration Create(SPWebApplication wap)
        {
            var res = wap.GetChild<UnityConfiguration>();
            if (res == null)
            {
                res = new UnityConfiguration(wap);
                res.Update();
            }

            return res;
        }
    }
}
