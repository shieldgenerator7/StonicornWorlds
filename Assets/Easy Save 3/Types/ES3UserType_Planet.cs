using System;
using System.Collections.Generic;
using UnityEngine;

namespace ES3Types
{
    [UnityEngine.Scripting.Preserve]
    [ES3PropertiesAttribute("position", "grid")]
    public class ES3UserType_Planet : ES3ObjectType
    {
        public static ES3Type Instance = null;

        public ES3UserType_Planet() : base(typeof(Planet)) { Instance = this; priority = 1; }


        protected override void WriteObject(object obj, ES3Writer writer)
        {
            var instance = (Planet)obj;

            writer.WriteProperty("position", instance.position, ES3Type_Vector2.Instance);
            writer.WriteProperty("pods", instance.PodsAll);
        }

        protected override void ReadObject<T>(ES3Reader reader, object obj)
        {
            var instance = (Planet)obj;
            foreach (string propertyName in reader.Properties)
            {
                switch (propertyName)
                {

                    case "position":
                        instance.position = reader.Read<UnityEngine.Vector2>(ES3Type_Vector2.Instance);
                        break;
                    case "pods":
                        instance.init(reader.Read<List<Pod>>("pods"));
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
        }

        protected override object ReadObject<T>(ES3Reader reader)
        {
            var instance = new Planet();
            ReadObject<T>(reader, instance);
            return instance;
        }
    }


    public class ES3UserType_PlanetArray : ES3ArrayType
    {
        public static ES3Type Instance;

        public ES3UserType_PlanetArray() : base(typeof(Planet[]), ES3UserType_Planet.Instance)
        {
            Instance = this;
        }
    }
}