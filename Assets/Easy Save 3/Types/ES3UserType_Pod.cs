using System;
using UnityEngine;

namespace ES3Types
{
    [UnityEngine.Scripting.Preserve]
    [ES3PropertiesAttribute("pos", "podTypeName")]
    public class ES3UserType_Pod : ES3ObjectType
    {
        public static ES3Type Instance = null;

        public ES3UserType_Pod() : base(typeof(Pod)) { Instance = this; priority = 1; }


        protected override void WriteObject(object obj, ES3Writer writer)
        {
            var instance = (Pod)obj;

            writer.WriteProperty("pos", instance.worldPos, ES3Type_Vector2.Instance);
            writer.WriteProperty("podTypeName", instance.typeName, ES3Type_string.Instance);
        }

        protected override void ReadObject<T>(ES3Reader reader, object obj)
        {
            var instance = (Pod)obj;
            foreach (string propertyName in reader.Properties)
            {
                switch (propertyName)
                {

                    case "pos":
                        // /!\ NRE here? obj is probably being passed in as null
                        // (check ReadObject() method below)
                        instance.worldPos = reader.Read<UnityEngine.Vector2>(ES3Type_Vector2.Instance);
                        break;
                    case "podTypeName":
                        instance.typeName = reader.Read<System.String>(ES3Type_string.Instance);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
            instance.objectType = Managers.Constants.getPodType(instance.typeName);
        }

        protected override object ReadObject<T>(ES3Reader reader)
        {
            Pod instance = null;// new Pod();
            ReadObject<T>(reader, instance);
            return instance;
        }
    }


    public class ES3UserType_PodArray : ES3ArrayType
    {
        public static ES3Type Instance;

        public ES3UserType_PodArray() : base(typeof(Pod[]), ES3UserType_Pod.Instance)
        {
            Instance = this;
        }
    }
}