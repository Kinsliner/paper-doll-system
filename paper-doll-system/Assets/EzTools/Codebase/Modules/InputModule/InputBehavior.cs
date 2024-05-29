using System.Collections;
using UnityEngine;

namespace Ez.Input
{

    public abstract class InputBehavior
    {
        public bool IsExecuting { get; set; }

        public virtual void Execute(Inputter inputter)
        {
            
        }

        public virtual void Terminate(Inputter inputter)
        {

        }

        public virtual void Update(Inputter inputter)
        {

        }
    }

}

