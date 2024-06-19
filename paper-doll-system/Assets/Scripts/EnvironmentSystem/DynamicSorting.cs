using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicSorting : MonoBehaviour
{
    [SerializeField]
    private Transform pointer;

    private static List<DynamicSortObject> dynamicSortObjects = new List<DynamicSortObject>();

    public static void AddDynamicSortObject(DynamicSortObject dynamicSortObject)
    {
        if (dynamicSortObject != null)
        {
            dynamicSortObjects.Add(dynamicSortObject);
        }
    }

    public static void RemoveDynamicSortObject(DynamicSortObject dynamicSortObject)
    {
        if (dynamicSortObject != null)
        {
            dynamicSortObjects.Remove(dynamicSortObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // check if pointer is not null
        if (pointer != null)
        {
            // loop through all dynamic sort objects
            foreach (DynamicSortObject dynamicSortObject in dynamicSortObjects)
            {
                // check if dynamic sort object is not null
                if (dynamicSortObject != null)
                {
                    // check if pointer is behind the dynamic sort object
                    if (pointer.position.y < dynamicSortObject.Anchor.position.y)
                    {
                        // set dynamic sort object to background
                        dynamicSortObject.SetToBackground();
                    }
                    else
                    {
                        // set dynamic sort object to foreground
                        dynamicSortObject.SetToForeground();
                    }
                }
            }
        }
    }
}
