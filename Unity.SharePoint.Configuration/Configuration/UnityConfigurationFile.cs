using System;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;

namespace Unity.SharePoint.Configuration
{
    public class UnityConfigurationFile : SPAutoSerializingObject
    {

        public static readonly string SpRoot = "SPROOT:";

        [Persisted]
        private string _name;

        [Persisted]
        private string _ds;

        [Persisted] private int _sequence;

        public string DataSource { get { return _ds; } set { _ds = value; } }

        public int Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual string MapSourcePath()
        {
            var file = DataSource;
            if (file.StartsWith(SpRoot, StringComparison.InvariantCultureIgnoreCase))
                file = SPUtility.GetGenericSetupPath(file.Substring(SpRoot.Length));

            return file;
        }
    }
}
