using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PoolManager
{
    private static Transform root;
    private static Dictionary<GameObject, ObjectPool<GameObject>> prefabPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
    private static Dictionary<GameObject, ObjectPool<GameObject>> usingPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
    private static Dictionary<ObjectPool<GameObject>, Transform> poolRoots = new Dictionary<ObjectPool<GameObject>, Transform>();

    private static bool isInit;

    public static void Init()
    {
        if (isInit == false)
        {
            GameObject rootObj = new GameObject("[Pool Manager]");
            root = rootObj.transform;
            isInit = true;
        }
    }

    public static void CreatePool(GameObject prefab, int size)
    {
        if (prefabPools.ContainsKey(prefab))
        {
            Debug.LogWarning("Pool for prefab " + prefab.name + " has already been created");
            return;
        }

        var root = GetPoolRoot(prefab.name);
        var pool = new ObjectPool<GameObject>(() => { return InstantiatePrefab(prefab, root); }, size);
        prefabPools[prefab] = pool;
        poolRoots.Add(pool, root);
    }

    private static GameObject InstantiatePrefab(GameObject prefab, Transform root)
    {
        var obj = UnityEngine.Object.Instantiate(prefab);
        obj.transform.SetParent(root);
        obj.SetActive(false);

        return obj;
    }

    private static Transform GetPoolRoot(string name)
    {
        if (root == null)
        {
            Init();
        }

        var poolRoot = root.Find(name);
        if (poolRoot == null)
        {
            GameObject newRoot = new GameObject(name);
            newRoot.transform.SetParent(root);
            poolRoot = newRoot.transform;
        }

        return poolRoot;
    }

    public static GameObject SpawnObject(GameObject prefab)
    {
        return SpawnObject(prefab, Vector3.zero, Quaternion.identity);
    }

    public static GameObject SpawnObject(GameObject prefab, Vector3 position)
    {
        return SpawnObject(prefab, position, Quaternion.identity);
    }

    public static GameObject SpawnObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (!prefabPools.ContainsKey(prefab))
        {
            CreatePool(prefab, 1);
        }

        var pool = prefabPools[prefab];

        var clone = pool.GetItem();
        clone.transform.SetPositionAndRotation(position, rotation);
        clone.SetActive(true);

        usingPools.Add(clone, pool);
        return clone;
    }

    public static void ReleaseObject(GameObject clone)
    {
        if (usingPools.ContainsKey(clone))
        {
            Transform root = GetPoolRoot(usingPools[clone]);
            clone.SetActive(false);
            clone.transform.SetParent(root);
            usingPools[clone].ReleaseItem(clone);
            usingPools.Remove(clone);
        }
        else
        {
            Debug.LogWarning("No pool contains the object: " + clone.name);
        }
    }

    private static Transform GetPoolRoot(ObjectPool<GameObject> pool)
    {
        if (poolRoots.ContainsKey(pool))
        {
            return poolRoots[pool];
        }
        return null;
    }
}
