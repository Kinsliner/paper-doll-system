using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StageState
{
    Enter,
    Update,
    Exit,
    Finish
}

public struct StageChangeData
{
    public string prevStageName;
    public string nextStageName;
    public Dictionary<string, object> transferData;

    public void AddTransferData(string key, object data)
    {
        if (transferData.ContainsKey(key) == false)
        {
            transferData.Add(key, new object());
        }
        transferData[key] = data;
    }
}

public abstract class StageBase
{
    private string name;
    internal bool IsNeedChangeStage { get; private set; }
    private StageState currentState;
    private StageChangeData stageChangeData;

    internal void SetName(string name)
    {
        this.name = name;
    }

    internal void StartStage()
    {
        currentState = StageState.Enter;
        IsNeedChangeStage = false;
    }

    internal void Update()
    {
        switch (currentState)
        {
            case StageState.Enter:
                EnterStage();
                break;
            case StageState.Update:
                UpdateStage();
                break;
            case StageState.Exit:
                ExitStage();
                break;
            case StageState.Finish:
                //這個Stage工作階段已結束，不再處理任何事情
                break;
        }
    }

    private void NextState()
    {
        if (currentState != StageState.Finish)
        {
            currentState += 1;
        }
    }

    private void EnterStage()
    {
        OnEnter();
        if (currentState != StageState.Exit)
        {
            NextState();
        }
    }

    private void UpdateStage()
    {
        OnUpdate();
    }

    private void ExitStage()
    {
        OnExit();
        NextState();
        IsNeedChangeStage = true;
    }

    protected virtual void OnEnter() { }
    protected virtual void OnUpdate() { }
    protected virtual void OnExit() { }
    public virtual void OnChangeStage(StageChangeData stageChangeData) { }
    public virtual void OnQuit() { }

    protected void ChangeStage(string stageName)
    {
        var defaultChangeData = GetStageChangeData(stageName);
        ChangeStage(defaultChangeData);
    }

    protected StageChangeData GetStageChangeData(string nextStageName)
    {
        var changeData = new StageChangeData()
        {
            prevStageName = name,
            nextStageName = nextStageName,
            transferData = new Dictionary<string, object>()
        };
        return changeData;
    }

    protected void ChangeStage(StageChangeData changeData)
    {
        if (changeData.nextStageName == name)
        {
            Debug.LogError($"換場景錯誤，欲切換的場景和當前場景名稱相同 {changeData.nextStageName}");
        }
        else
        {
            currentState = StageState.Exit;
            stageChangeData = changeData;
        }
    }

    public StageChangeData GetStageChangeData()
    {
        return stageChangeData;
    }
}
