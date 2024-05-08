using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VARLive.Console
{
    public class ConsoleUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject logUIPrefab;
        [SerializeField]
        private Transform logRoot;
        [SerializeField]
        private InputField inputField;

        // Start is called before the first frame update
        void Start()
        {
            inputField.onEndEdit.AddListener(ApplyInput);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ApplyInput(string input)
        {
            if (string.IsNullOrEmpty(input))
                return;

            if (input.StartsWith('/'))
            {
                Commander.ExecuteCommand(input);
            }
            else
            {
                AddLog(input);
            }

            //Clear input
            inputField.text = string.Empty;
        }

        public void AddLog(string log)
        {
            GameObject logObj = Instantiate(logUIPrefab, logRoot);
            var logUI = logObj.GetComponent<LogUI>();
            if (logUI != null)
            {
                logUI.Log(log);
            }
        }
    }

}
