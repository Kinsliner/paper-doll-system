using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ObjectField : ConfiguredReorderableList<CheckAsset>.Column
{
    public ObjectField(string label, float width) : base(label, width)
    {
    }

    public override List<CheckAsset> OrderBy(bool isAscending, List<CheckAsset> elements)
    {
        if(isAscending)
        {
            return elements.OrderBy(x => x.asset.name).ToList();
        }
        else
        {
            return elements.OrderByDescending(x => x.asset.name).ToList();
        }
    }

    public override void DrawColumn(Rect rect, CheckAsset element)
    {
        base.DrawColumn(rect, element);

        EditorGUI.ObjectField(rect, element.asset, typeof(Object), false);
    }
}
