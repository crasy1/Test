using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;


public class FileUtil : Node
{
    public static string GetFieldTypeName<T>(string field) where T : BaseData
    {
        var type = typeof(T);
        FieldInfo[] fields = type.GetFields();
        foreach (FieldInfo fi in fields)
        {
            if (field.Equals(fi.Name))
            {
                return fi.FieldType.Name;
            }
        }
        return "";
    }

    //  dictinary to T
    public static T Dic2T<T>(Dictionary<string, string> dictionary) where T : BaseData, new()
    {
        var t = new T();
        var propertyList = t.GetPropertyList();
        foreach (var property in propertyList)
        {
            var prop = property as Godot.Collections.Dictionary;
            var propName = prop["name"].ToString();

            if (dictionary.ContainsKey(propName))
            {
                var propType = (int) prop["type"];
                var propTypeName = GetFieldTypeName<T>(propName);
                t.Set(propName, Convert(propType, dictionary[propName], propTypeName));
            }
        }

        return t;
    }

    public static object Convert(int type, string stringVal, string typeClass)
    {
        object res = null;
        switch (type)
        {
            case (int) Variant.Type.Int:
                res = int.Parse(stringVal);
                break;
            case (int) Variant.Type.Bool:
                res = bool.Parse(stringVal);
                break;
            case (int) Variant.Type.Real:
                res = float.Parse(stringVal);
                break;
            case (int) Variant.Type.String:
                res = stringVal;
                break;
            case (int) Variant.Type.Vector2:
                res = ParseVector2(stringVal);
                break;
            case (int) Variant.Type.Color:
                res = ParseColor(stringVal);
                break;
            case (int) Variant.Type.Object:
                if ("PackedScene".Equals(typeClass))
                {
                    res = GD.Load<PackedScene>(stringVal);
                }
                else if ("Texture".Equals(typeClass))
                {
                    res = GD.Load<StreamTexture>(stringVal);
                }
                else if ("Resource".Equals(typeClass))
                {
                    res = GD.Load<Resource>(stringVal);
                }

                break;
            default:
                break;
        }

        return res;
    }

    public static Color ParseColor(string color)
    {
        var strings = color.Split(" ");
        return new Color(int.Parse(strings[0]), int.Parse(strings[1]), int.Parse(strings[2]));
    }

    public static Vector2 ParseVector2(string vector2)
    {
        var strings = vector2.Split(" ");
        return new Vector2(float.Parse(strings[0]), float.Parse(strings[1]));
    }

    // 	load json file
    public static List<T> LoadJson<T>(string jsonPath) where T : ComboData, new()
    {
        var file = new File();
        file.Open(jsonPath, File.ModeFlags.Read);

        var dictionarys = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(file.GetAsText());
        List<T> list = new List<T>();
        foreach (var item in dictionarys)
        {
            list.Add(Dic2T<T>(item));
        }

        file.Close();
        return list;
    }

}