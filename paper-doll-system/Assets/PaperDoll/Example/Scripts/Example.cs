using Ez.Input;
using Ez.SystemModule;
using Ez.Tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Example
{
    public class Example : MonoBehaviour
    {
        public KeyCode openKey = KeyCode.P;
        public Character character;

        private bool isActivated = false;

        public void Start()
        {
            Installer.Install<SceneLoader>();
            SceneLoader.LoadSceneAdditive(Scenes.Scenes_PaperDollSystem, OnLoadScene);
        }

        private void OnLoadScene()
        {
            character.Init();
        }

        public void Uninit()
        {
        }

        public void Update()
        {
            InputSystem.Update();

            if (Input.GetKeyDown(openKey))
            {
                isActivated = !isActivated;
                if(isActivated)
                {
                    character.SetMoveable(false);
                    PaperDollSystem.ActivePanel();
                    PaperDollSystem.SetPaperDoll(character.PaperDoll);
                }
                else
                {
                    character.SetMoveable(true);
                    PaperDollSystem.DeactivePanel();
                    character.ResetPaperDoll();
                }
            }
        }
    }

}
