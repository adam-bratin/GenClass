using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace GenClass
{
    [Serializable]
    class Feature : IComparable, ISerializable
    {
        private String name;
        private String originatedFilename;
        private List<float> values = new List<float>();

        public Feature(String inName, String inFilename)
        {
            this.name = inName;
            this.originatedFilename = inFilename;
        }

        public void setName(String inName)
        {
            this.name = inName;
        }

        public String getName()
        {
            return this.name;
        }

        public void setOriginatedFilename(String inOriginatedFilename)
        {
            this.originatedFilename = inOriginatedFilename;
        }

        public String getOriginatedFilename()
        {
            return this.originatedFilename;
        }

        public List<float> getValues()
        {
            return this.values;
        }

        public void addValue(float newValue)
        {
            this.values.Add(newValue);
        }

        public int CompareTo(Object obj)
        {
            if (obj.Equals(null))
            {
                return 1;
            }
            Feature otherFeature = obj as Feature;
            if (otherFeature != null)
            {
                return String.Compare(this.name,otherFeature.getName());
            }
            else
            {
                throw new ArgumentException("Object is not a Feature");
            }
        }

        public override String ToString()
        {
            return this.name;
        }

        protected Feature(SerializationInfo info, StreamingContext context)
        {
            this.name = info.GetString("name");
            this.values = (List<float>)info.GetValue("values", values.GetType());
            this.originatedFilename = info.GetString("filename");
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", this.name);
            info.AddValue("values", this.values);
            info.AddValue("filename", this.originatedFilename);
        }
    }
}
