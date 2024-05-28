using Ez.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardMoveInput : Inputter
{
    public override bool IsCheck()
    {
        return true;
    }

    public override T ReadInput<T>()
    {
        if (typeof(T) == typeof(Vector2))
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            return (T)(object)new Vector2(horizontalInput, verticalInput);
        }
        else
        {
            // 非法的泛型類型，返回預設值
            return default(T);
        }
    }
}

public class Movement : InputBehavior
{
    private struct AnimName
    {
        public string Idle;
        public string Move;
    }

    public float Speed { get; set;} = 5.0f;

    private Transform target;
    private PaperDoll paperDoll;
    private Dictionary<BodyDirection, AnimName> animNames = new Dictionary<BodyDirection, AnimName>();

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void SetPaperDoll(PaperDoll paperDoll)
    {
        this.paperDoll = paperDoll;
    }

    public void Init()
    {
        animNames.Add(BodyDirection.Front, new AnimName { Idle = "IdleFront", Move = "MoveFront" });
        animNames.Add(BodyDirection.Back, new AnimName { Idle = "IdleBack", Move = "MoveBack" });
        animNames.Add(BodyDirection.Left, new AnimName { Idle = "IdleLeft", Move = "MoveLeft" });
        animNames.Add(BodyDirection.Right, new AnimName { Idle = "IdleRight", Move = "MoveRight" });
    }

    public override void Update(Inputter inputter)
    {
        base.Update(inputter);

        Vector2 move = inputter.ReadInput<Vector2>();

        target.Translate(move * Speed * Time.deltaTime);

        BodyDirection direction = paperDoll.CurrentDirection;
        // 更新角色方向
        if (move.x > 0)
        {
            direction = BodyDirection.Right;
        }
        else if (move.x < 0)
        {
            direction = BodyDirection.Left;
        }
        if(move.y > 0)
        {
            direction = BodyDirection.Back;
        }
        else if(move.y < 0)
        {
            direction = BodyDirection.Front;
        }

        paperDoll.SetDirection(direction);

        // 更新角色動畫
        if (move != Vector2.zero)
        {
            // 角色移動
            string animName = animNames[direction].Move;
            paperDoll.PlayAnim(animName);
        }
        else
        {
            // 角色停止
            string animName = animNames[direction].Idle;
            paperDoll.PlayAnim(animName);
        }
    }
}
