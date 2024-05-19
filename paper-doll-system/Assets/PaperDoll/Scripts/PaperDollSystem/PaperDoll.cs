using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 部位節點
/// </summary>
public enum BodyNode
{
    Head,
    Body,
}

[System.Serializable]
public struct BodyPart
{
    /// <summary>
    /// 部位節點
    /// </summary>
    public BodyNode node;

    /// <summary>
    /// 位置
    /// </summary>
    public Transform root;

    /// <summary>
    /// 部位物件
    /// </summary>
    [HideInInspector]
    public GameObject attachObject;
}

public class PaperDoll : MonoBehaviour
{
    [SerializeField]
    private List<BodyPart> parts = new List<BodyPart>(); // 部位列表

    void Start()
    {
    }

    public void Attach(GameObject part, BodyNode node)
    {
        // 找到對應的部位
        var bodyPart = parts.Find(x => x.node == node);

        // 移除舊的物件
        if (bodyPart.attachObject != null)
        {
            Destroy(bodyPart.attachObject);
        }

        // 產生新的物件
        var clone = Instantiate(part, bodyPart.root);

        // 附加新的物件
        bodyPart.attachObject = clone;
    }
}
