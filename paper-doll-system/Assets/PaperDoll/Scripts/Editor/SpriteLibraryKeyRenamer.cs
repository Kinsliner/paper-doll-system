using UnityEngine;
using UnityEditor;
using UnityEngine.U2D.Animation;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using System;
using System.Linq;

public class SpriteLibCategoryCache
{
    public Sprite sprite;
    public string category;
    public string label;
}

public class SplitData
{
    public string categoryName;
    public int startIndex;
    public int endIndex;
}

public class SpriteLibraryKeyRenamer : EditorWindow
{
    private static List<SplitData> splitDatas = new List<SplitData>();
    private SpriteLibraryAsset selectedSpriteLibrary;
    private Texture2D sprite;

    [MenuItem("Tools/Rename Sprite Library Keys", priority = 3000)]
    public static void ShowWindow()
    {
        GetWindow<SpriteLibraryKeyRenamer>("Rename Sprite Library Keys");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Select Sprite Library Asset", EditorStyles.boldLabel);
        var newAsset = (SpriteLibraryAsset)EditorGUILayout.ObjectField(selectedSpriteLibrary, typeof(SpriteLibraryAsset), true);
        if (selectedSpriteLibrary != newAsset)
        {
            selectedSpriteLibrary = newAsset;
            RefreshSplitDatas();
        }

        if(GUILayout.Button("Refresh Split Data"))
        {
            RefreshSplitDatas();
        }

        DrawSplitDatas();

        // draw sprite asset
        sprite = (Texture2D)EditorGUILayout.ObjectField("Sprite", sprite, typeof(Texture2D), false);

        // draw sprite asset
        if (GUILayout.Button("Split"))
        {
            SplitSprite();
        }
    }

    private void RefreshSplitDatas()
    {
        if(selectedSpriteLibrary == null)
        {
            return;
        }

        // create split data from sprite library category
        splitDatas.Clear();
        foreach (var category in selectedSpriteLibrary.GetCategoryNames())
        {
            splitDatas.Add(new SplitData
            {
                categoryName = category,
                startIndex = 0,
                endIndex = 0,
            });
        }

        LoadSplitDatas();
    }

    private void SplitSprite()
    {
        if (sprite == null)
        {
            Debug.LogError("Sprite is null");
            return;
        }

        List<SpriteLibCategoryCache> spriteLibCategoryCache = GetSpriteLibCategoryCache(selectedSpriteLibrary);

        string path = AssetDatabase.GetAssetPath(sprite);

        var objects = AssetDatabase.LoadAllAssetsAtPath(path);
        var sprites = objects.Where(q => q is Sprite).Cast<Sprite>();

        Debug.Log($"Total sprites: {sprites.Count()}");
        Debug.Log($"Total cache: {spriteLibCategoryCache.Count}");

        foreach (var item in splitDatas)
        {
            var category = item.categoryName;
            var startIndex = item.startIndex;
            var endIndex = item.endIndex;

            if (startIndex < 0 || endIndex < 0)
            {
                Debug.LogError("Invalid start or end index");
                return;
            }

            if (startIndex > endIndex)
            {
                Debug.LogError("Start index must be less than end index");
                return;
            }

            var categoryCache = spriteLibCategoryCache.Where(q => q.category == category).ToList();
            if (categoryCache.Count == 0)
            {
                Debug.LogError($"Category {category} not found in sprite library");
                return;
            }

            sprites.ToList().ForEach(q => Debug.Log(q.name));

            var splitSprites = sprites.Skip(startIndex).Take(endIndex - startIndex + 1).ToList();

            int i = 0;
            foreach (var splitSprite in splitSprites)
            {
                var cache = categoryCache[i];
                cache.sprite = splitSprite;
                i++;
            }
        }

        // match split data to sprite library
        for (int i = 0; i < spriteLibCategoryCache.Count; i++)
        {
            var cache = spriteLibCategoryCache[i];
            var category = cache.category;
            var label = cache.label;
            var sprite = cache.sprite;

            selectedSpriteLibrary.AddCategoryLabel(sprite, category, label);
        }

        SaveSplitDatas();

        string spriteLibraryPath = AssetDatabase.GetAssetPath(selectedSpriteLibrary);
        selectedSpriteLibrary.SaveAsSourceAsset(spriteLibraryPath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void SaveSplitDatas()
    {
        foreach (var item in splitDatas)
        {
            EditorPrefs.SetString($"{item.categoryName}_start", item.startIndex.ToString());
            EditorPrefs.SetString($"{item.categoryName}_end", item.endIndex.ToString());
        }
    }

    private void LoadSplitDatas()
    {
        foreach (var item in splitDatas)
        {
            item.startIndex = int.Parse(EditorPrefs.GetString($"{item.categoryName}_start", "0"));
            item.endIndex = int.Parse(EditorPrefs.GetString($"{item.categoryName}_end", "0"));
        }
    }

    private void DrawSplitDatas()
    {
        using (new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            foreach (var item in splitDatas)
            {
                using (new GUILayout.VerticalScope("box"))
                {
                    item.categoryName = EditorGUILayout.TextField("Category", item.categoryName);
                    item.startIndex = EditorGUILayout.IntField("Start", item.startIndex);
                    item.endIndex = EditorGUILayout.IntField("End", item.endIndex);
                }
                
            }
        }
    }

    private static List<SpriteLibCategoryCache> GetSpriteLibCategoryCache(SpriteLibraryAsset spriteLibraryAsset)
    {
        var cache = new List<SpriteLibCategoryCache>();

        foreach (var category in spriteLibraryAsset.GetCategoryNames())
        {
            foreach (var label in spriteLibraryAsset.GetCategoryLabelNames(category))
            {
                var sprite = spriteLibraryAsset.GetSprite(category, label);

                cache.Add(new SpriteLibCategoryCache
                {
                    sprite = sprite,
                    category = category,
                    label = label,
                });
            }
        }


        

        return cache;
    }

    private static void RenameKeys(SpriteLibraryAsset spriteLibraryAsset)
    {
        List<SpriteLibCategoryCache> spriteLibCategoryCache = GetSpriteLibCategoryCache(spriteLibraryAsset);

        foreach (var cache in spriteLibCategoryCache)
        {
            // rename all label with remove start with "head_"
            if (cache.label.StartsWith("head_"))
            {
                string newLabel = cache.label.Replace("head_", "");
                cache.label = newLabel;
            }
        }

        ApplyChanges(spriteLibraryAsset, spriteLibCategoryCache);

        string path = AssetDatabase.GetAssetPath(spriteLibraryAsset);
        spriteLibraryAsset.SaveAsSourceAsset(path);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private static void ApplyChanges(SpriteLibraryAsset spriteLibraryAsset, List<SpriteLibCategoryCache> spriteLibCategoryCache)
    {
        foreach (var cache in spriteLibCategoryCache)
        {
            spriteLibraryAsset.AddCategoryLabel(cache.sprite, cache.category, cache.label);
        }
    }
}
