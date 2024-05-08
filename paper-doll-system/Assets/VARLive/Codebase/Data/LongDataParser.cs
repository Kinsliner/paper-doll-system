using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongDataParser
{
    public int MaxLength { get; private set; }
    public int SplitLength { get; private set; }

    public LongDataParser(int maxLength, int splitLength)
    {
        MaxLength = maxLength;
        SplitLength = splitLength;
    }

    public string[] Parse(string data)
    {
        string[] splitDatas;
        if (data.Length > MaxLength)
        {
            int splitLength = data.Length / SplitLength;
            int remainLength = data.Length % SplitLength > 0 ? 1 : 0;
            int dataLength = splitLength + remainLength;
            splitDatas = new string[dataLength];
            for (int i = 0; i < dataLength; i++)
            {
                string splitData;
                if (i == dataLength - 1)
                {
                    //最後一筆資料
                    splitData = data.Substring(i * SplitLength);
                }
                else
                {
                    splitData = data.Substring(i * SplitLength, SplitLength);
                }
                splitDatas[i] = splitData;
            }
        }
        else
        {
            splitDatas = new string[1];
            splitDatas[0] = data;
        }
        return splitDatas;
    }
}
