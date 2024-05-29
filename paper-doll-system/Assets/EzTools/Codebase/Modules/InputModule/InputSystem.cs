using System.Collections.Generic;

namespace Ez.Input
{
    public static class InputSystem
    {
        private static Dictionary<Inputter, List<InputBehavior>> inputBehaviors = new Dictionary<Inputter, List<InputBehavior>>();
   
        public static void Register(Inputter inputter, InputBehavior behavior)
        {
            inputBehaviors.TryAdd(inputter, new List<InputBehavior>());
            inputBehaviors[inputter].TryAdd(behavior);
        }

        public static void Unregister(Inputter inputter, InputBehavior behavior)
        {
            if (inputBehaviors.ContainsKey(inputter))
            {
                inputBehaviors[inputter].Remove(behavior);
            }
        }

        
        public static void Update()
        {
            foreach (var inputter in inputBehaviors.Keys)
            {
                if (inputter.IsCheck())
                {
                    foreach (var behavior in inputBehaviors[inputter])
                    {
                        if (!behavior.IsExecuting)
                        {
                            behavior.Execute(inputter);
                            behavior.IsExecuting = true;
                        }
                    }

                    // Update Behavior
                    foreach (var behavior in inputBehaviors[inputter])
                    {
                        behavior.UpdateInput(inputter);
                    }
                }
                else
                {
                    foreach (var behavior in inputBehaviors[inputter])
                    {
                        if (behavior.IsExecuting)
                        {
                            behavior.Terminate(inputter);
                            behavior.IsExecuting = false;
                        }
                    }
                }
            }
        }
    }

}

