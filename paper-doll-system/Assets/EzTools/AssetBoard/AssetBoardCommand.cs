using System.Collections.Generic;
using UnityEngine;

public abstract class AssetBoardCommand
{
    public abstract string CommandName { get; }

    public abstract void Execute(List<Object> objects);
}
