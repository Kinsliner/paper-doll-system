using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Step
{
	public virtual void OnStart() { }
	public virtual void OnUpdate() { }
	public abstract bool IsComplete();
}

public class Stepper
{
	private List<Step> steps = new List<Step>();
	private int currentStepIndex = 0;
	private Step currentStep = null;

	public void ClearSteps()
	{
		currentStepIndex = 0;
		currentStep = null;
		steps.Clear();
	}

	public void AddStep(Step step)
	{
		steps.Add(step);
	}

	public void Start()
	{
		currentStepIndex = 0;
		StartStep();
	}

	private void NextStep()
	{
		currentStepIndex += 1;
		if(currentStepIndex < steps.Count)
		{
			StartStep();
		}else
		{
			currentStep = null;
		}
	}

	private void StartStep()
	{
		currentStep = steps[currentStepIndex];
		currentStep.OnStart();
	}

	public void UpdateStep()
	{
		if (currentStep != null)
		{
			currentStep.OnUpdate();
			if (currentStep.IsComplete())
			{
				NextStep();
			}
		}
	}

	public bool IsAllStepsComplete()
	{
		return currentStepIndex >= steps.Count;
	}
}
