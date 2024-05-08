using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageController
{
    private Dictionary<string, StageBase> stages = new Dictionary<string, StageBase>();
    private StageBase currentStage;

    public void AddStage(string name, StageBase stage)
    {
        if (stage == null)
        {
            Debug.LogError("加入的場景無效");
            return;
        }
        stage.SetName(name);
        string key = name.ToLower();
        if (stages.ContainsKey(key) == false)
        {
            stages.Add(key, stage);
        }
        else
        {
            Debug.LogError("加入場景失敗，偵測到重複的Key值");
        }
    }

    public void RemoveStage(string name)
    {
        string key = name.ToLower();
        if (stages.ContainsKey(key))
        {
            stages.Remove(key);
        }
    }

    public void SetStartStage(string name)
    {
        string key = name.ToLower();
        if (stages.ContainsKey(key))
        {
            currentStage = stages[key];
            currentStage.StartStage();
        }
        else
        {
            Debug.LogError("設定起始場景失敗，找不到Key值");
        }
    }

    public void UpdateStages()
    {
        if (currentStage == null)
        {
            Debug.LogError("找不到當前場景，請確認是否有正確設定起始場景");
        }

        if (currentStage.IsNeedChangeStage)
        {
            StageChangeData stageChangeData = currentStage.GetStageChangeData();

            string key = stageChangeData.nextStageName.ToLower();
            if (stages.ContainsKey(key))
            {
                currentStage = stages[key];
                currentStage.OnChangeStage(stageChangeData);
                currentStage.StartStage();
            }
            else
            {
                Debug.LogError("切換場景失敗，請確認是否有正確設定切換場景");
            }
        }

        currentStage.Update();
    }

    public void Quit()
    {
        if (currentStage != null)
        {
            currentStage.OnQuit();
        }
        foreach (var stage in stages.Values)
        {
            if (stage == currentStage)
                continue;
            stage.OnQuit();
        }
    }
}
