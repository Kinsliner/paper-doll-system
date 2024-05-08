using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

public class CheckAssetReorderableList : ConfiguredReorderableList<CheckAsset>
{
    public Action<CheckAsset> OnFocusing;
    private CheckAsset focusingAsset;

    public CheckAssetReorderableList(List<CheckAsset> elements, Configurator configurator) : base(elements, configurator)
    {
    }
    protected override void DrawElementBackgroundCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        base.DrawElementBackgroundCallback(rect, index, isActive, isFocused);

        if (isFocused)
        {
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.5f));

            var elenent = elements[index];
            if (elenent != null)
            {
                OnFocusing?.Invoke(elenent);

                if (focusingAsset != null)
                {
                    focusingAsset.IsSelected = false;
                }

                elenent.IsSelected = true;
                focusingAsset = elenent;
            }
        }
    }

    public CheckAsset GetFocusingElement()
    {
        return focusingAsset;
    }
}
