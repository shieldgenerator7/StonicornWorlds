using System;
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
            writer.WritePrivateField("grid", instance);
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
                    case "grid":
                        reader.SetPrivateField("grid", reader.Read<HexagonGrid<Pod>>(), instance);
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }
            instance.init();
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