using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ez.MapID
{
    public class MapAttribute : Attribute
    {
        public string ShowName { get; private set; }

        public MapAttribute(string showName)
        {
            ShowName = showName;
        }
    }

    public class MapID
    {
        public string ShowName;
        public string BelongName;
        public int ID;
        public Type BelongType;
        public FieldInfo Field;

        public static List<MapID> GenerateMap(Type type)
        {
            var result = new List<MapID>();
            var fields = type.GetFields().ToList();
            var nestTypes = type.GetNestedTypes();
            foreach (var nestType in nestTypes)
            {
                result.AddRange(GenerateMap(nestType));
            }
            foreach (var field in fields)
            {

                MapID map = new MapID();
                map.ShowName = field.Name;
                map.BelongName = type.Name;
                var fieldAttrs = field.GetCustomAttributes(true);
                foreach (var fieldAttr in fieldAttrs)
                {
                    if (fieldAttr is MapAttribute propertyAttr)
                    {
                        map.ShowName = propertyAttr.ShowName;
                    }
                }
                map.ID = (int)field.GetValue(field.Name);
                map.BelongType = type;
                map.Field = field;
                result.Add(map);
            }

            return result;
        }
    }
}
