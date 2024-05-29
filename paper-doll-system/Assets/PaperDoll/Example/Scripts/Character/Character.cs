using Ez.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Example
{
    public class Character : MonoBehaviour
    {
        /// <summary>
        /// 角色紙娃娃
        /// </summary>
        public PaperDoll PaperDoll => paperDoll;

        [SerializeField]
        private Transform character;
        [SerializeField]
        private PaperDoll paperDoll;
        [SerializeField]
        private float moveSpeed = 5.0f;

        // 鍵盤移動輸入
        private KeyBoardMoveInput keyBoardMoveInput = new KeyBoardMoveInput();

        public void Init()
        {
            // 建立角色紙娃娃
            PaperDollSystem.SetPaperDoll(paperDoll);

            // 建立角色移動功能
            var movement = new Movement();
            movement.SetTarget(character.transform);
            movement.SetPaperDoll(paperDoll);
            movement.Init();
            movement.Speed = moveSpeed;

            // 註冊角色移動輸入
            InputSystem.Register(keyBoardMoveInput, movement);
        }

        // Update is called once per frame
        void Update()
        {
        }

        /// <summary>
        /// 設定角色是否可移動
        /// </summary>
        public void SetMoveable(bool isMoveable)
        {
            keyBoardMoveInput.IsEnable = isMoveable;
        }

        /// <summary>
        /// 將紙娃娃重置到角色身上
        /// </summary>
        public void ResetPaperDoll()
        {
            paperDoll.transform.SetAndFitParent(character);
        }
    }
}