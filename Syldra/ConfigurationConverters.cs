using BepInEx.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Syldra
{
    public static class ConfigurationConverters
    {
        private static void UnityColorConverter()
        {
            if (!TomlTypeConverter.CanConvert(typeof(Color)))
            {
                TomlTypeConverter.AddConverter(typeof(Color), new TypeConverter()
                {
                    ConvertToObject = (string s, Type t) =>
                    {
                        if (string.IsNullOrEmpty(s)) throw new FormatException("Color cannot be null");
                        bool check = ColorUtility.TryParseHtmlString(s, out var o);
                        //ModComponent.Log.LogInfo(check);
                        //ModComponent.Log.LogInfo(o);
                        if (!check) throw new FormatException("Color must follow #RRGGBBAA format, or match UnityEngine.color names");
                        return o;
                    },
                    ConvertToString = (object o, Type t) =>
                    {
                        var x = (Color)o;
                        //ToHtmlStringRGBA is broken for some reason, time to go old school
                        var r = (Byte)Mathf.Clamp(Mathf.RoundToInt(x.r * 255), 0, 255);
                        var g = (Byte)Mathf.Clamp(Mathf.RoundToInt(x.g * 255), 0, 255);
                        var b = (Byte)Mathf.Clamp(Mathf.RoundToInt(x.b * 255), 0, 255);
                        var a = (Byte)Mathf.Clamp(Mathf.RoundToInt(x.a * 255), 0, 255);

                        return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
                    }
                });
            }
        }

        public static void AddConverters()
        {
            UnityColorConverter();
        }
    }
}
