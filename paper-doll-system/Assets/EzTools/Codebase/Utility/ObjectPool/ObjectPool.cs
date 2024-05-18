using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolContainer<T>
{
    public bool Used { get; private set; }
    public T Item { get; set; }

    public void Consume()
    {
        Used = true;
    }
    public void Release()
    {
        Used = false;
    }
}

public class ObjectPool<T>
{
    public int Count { get { return containers.Count; } }
    public int CountUsingItems { get { return usingContainers.Count; } }

    private List<ObjectPoolContainer<T>> containers;
    private Dictionary<T, ObjectPoolContainer<T>> usingContainers;
    private Func<T> createFunction;
    private int lastIndex = 0;

    public ObjectPool(Func<T> createFunction, int capacity)
    {
        this.createFunction = createFunction;

        containers = new List<ObjectPoolContainer<T>>(capacity);
        usingContainers = new Dictionary<T, ObjectPoolContainer<T>>(capacity);

        Init(capacity);
    }

    private void Init(int capacity)
    {
        for (int i = 0; i < capacity; i++)
        {
            CreateContainer();
        }
    }

    private ObjectPoolContainer<T> CreateContainer()
    {
        var container = new ObjectPoolContainer<T>();
        container.Item = createFunction.Invoke();
        containers.Add(container);

        return container;
    }

    public T GetItem()
    {
        ObjectPoolContainer<T> container = null;
        for (int i = 0; i < containers.Count; i++)
        {
            lastIndex++;
            if (lastIndex > containers.Count - 1) lastIndex = 0;

            if (containers[lastIndex].Used)
            {
                continue;
            }
            else
            {
                container = containers[lastIndex];
                break;
            }
        }

        if (container == null)
        {
            container = CreateContainer();
        }

        container.Consume();
        usingContainers.Add(container.Item, container);
        return container.Item;
    }

    public void ReleaseItem(T item)
    {
        if (usingContainers.ContainsKey(item))
        {
            var container = usingContainers[item];
            container.Release();
            usingContainers.Remove(item);
        }
        else
        {
            Debug.LogWarning("This object pool does not contain the item provided: " + item);
        }
    }
}
