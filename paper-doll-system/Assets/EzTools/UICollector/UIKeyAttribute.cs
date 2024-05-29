using UnityEngine;

public class UIKeyAttribute : PropertyAttribute
{
    public string GroupBy { get; private set; } = string.Empty;

    public UIKeyAttribute()
    {
    }

    public UIKeyAttribute(string groupBy)
    {
        GroupBy = groupBy;
    }
}
