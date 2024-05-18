﻿using System.Collections.Generic;

[System.Serializable]
public class PaperDollData
{
    public int id;
    public string name;
    public BodyNode node;
    public int assetID;
}

[System.Serializable]
public class PaperDollDataTable
{
    public List<PaperDollData> datas = new List<PaperDollData>();
}