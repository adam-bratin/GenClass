using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace GenClass
{
    [Serializable]
    class Feature : IComparable, ISerializable
    {
        public static SortedList<String, Feature> featureList = new SortedList<String, Feature>();
        private String name;
        private List<float> values = new List<float>();
        private String originatedFilename;

        public Feature(String inName, String inFilename)
        {
            name = inName;
            originatedFilename = inFilename;
            featureList.Add(this.name, this);
        }


        public void setName(String name)
        {
            this.name = name;
        }

        public String getName()
        {
            return name;
        }

        public void setOriginatedFilename(String originatedFilename)
        {
            this.originatedFilename = originatedFilename;
        }

        public String getOriginatedFilename()
        {
            return originatedFilename;
        }

        public List<float> getValues()
        {
            return values;
        }

        public void addValue(float newValue)
        {
            values.Add(newValue);
        }

        public int CompareTo(Feature inFeature)
        {
            return name.CompareTo(inFeature.ToString());
        }

        public String ToString()
        {
            return name;
        }

        protected Feature(SerializationInfo info, StreamingContext context)
        {
            name = info.GetString("name");
            values = (List<float>)info.GetValue("values", values.GetType());
            originatedFilename = info.GetString("filename");
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", name);
            info.AddValue("values", values);
            info.AddValue("filename", originatedFilename);
        }
    }
}
