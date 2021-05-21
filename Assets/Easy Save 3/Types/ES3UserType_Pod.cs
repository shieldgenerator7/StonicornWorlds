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

            writer.WriteProperty("pos", instance.pos, ES3Type_Vector2.Instance);
            writer.WriteProperty("podTypeName", instance.podTypeName, ES3Type_string.Instance);
        }

        protected override void ReadObject<T>(ES3Reader reader, object obj)
        {
            var instance = (Pod)obj;
            foreach (string propertyName in reader.Properties)
            {
                switch (propertyName)
                {

                    case "pos":
                        instance.pos = reader.Read<UnityEngine.Vector2>(ES3Type_Vector2.Instance);
                        break;
                    case "podTypeName":
                        instance.podTypeName = reader.Read<System.String>(ES3Type_string.Instance);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
            instance.podType = Managers.PodTypeBank.getPodType(instance.podTypeName);
        }

        protected override object ReadObject<T>(ES3Reader reader)
        {
            var instance = new Pod();
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