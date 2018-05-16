using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Linq;

namespace LBE.Assets
{
    public class AssetDefinition
    {
        public Dictionary<String, Object> Fields { get; set; }
        public String Name { get; set; }
        public String Type { get; set; }

        public Object this[String s]
        {
            get { return Fields[s]; }
            set { Fields[s] = value; }
        }

        public AssetDefinition(String name, String type)
        {
            Name = name;
            Type = type;
            Fields = new Dictionary<String, Object>();
        }

        public AssetDefinition AsTable(String key)
        {
            return this[key] as AssetDefinition;
        }

        public float AsFloat(String key)
        {
            return Convert.ToSingle((Double)this[key]);
        }

        public int AsInt(String key)
        {
            return Convert.ToInt32((Double)this[key]);
        }

        public bool AsBool(String key)
        {
            int val = Convert.ToInt32((Double)this[key]);
            return val != 0;
        }

        public String AsString(String key)
        {
            return this[key] as String;
        }

        public Point AsPoint(String key)
        {
            return new Point(
                AsTable(key).AsInt("x"),
                AsTable(key).AsInt("y"));
        }
    }

    public static class AssetDefinitionXMLHelper
    {
        public static AssetDefinition FromXml(XElement element)
        {
            String name = element.Name.ToString();
            String type = "";

            XAttribute typeAttribute = element.Attribute("Type");
            if (typeAttribute != null) type = typeAttribute.Value;

            AssetDefinition assetDef = new AssetDefinition(name, type);

            foreach (var child in element.Elements())
            {
                
            //if (element.Name == "Item")
            //    {
            //        int key = Int32.Parse(element.Attribute("id").Value);
            //        assetDef.Fields[key] = ValueFromXML(element.Value
            //    }
            }

            return assetDef;
        }

        //public static Object ValueFromXML(XElement element)
        //{
        //    if(element.HasElements)
        //    {
        //    }
        //    else
        //    {
        //        if (element.Name == "AssetReference")
        //            return new AssetReference(element.Value);
        //        else
        //            return element.Value;
        //    }
        //}

        public static String ToXml(AssetDefinition assetDef)
        {
            XElement root = new XElement(assetDef.Name, ToXElement(assetDef));
            FromXml(root);
            return root.ToString();
        }

        private static XElement ToXElement(String key, Object value)
        {
            XElement el;
            int intKey;
            if (int.TryParse(key, out intKey))
            {
                el = new XElement("Item");
                el.SetAttributeValue("id", intKey);
            }
            else
            {
                el = new XElement(key);
            }

            if (value is AssetDefinition)
            {
                el.Add(ToXElement((AssetDefinition)value));
            }
            else if (value is AssetReference)
            {
                el.Add(
                    new XElement(key,
                    new XElement("AssetReference", ((AssetReference)value).Path)));
            }
            else
            {
                el.Add(value);
            }

            return el;
        }

        private static XElement[] ToXElement(AssetDefinition assetDef)
        {
            if (assetDef.Fields.Keys.Count == 0)
                return new XElement[] { };

            List<XElement> els = new List<XElement>();
            foreach (var key in assetDef.Fields.Keys)
            {
                els.Add(ToXElement(key, assetDef.Fields[key]));
            }
            return els.ToArray();
        }
    }
}
