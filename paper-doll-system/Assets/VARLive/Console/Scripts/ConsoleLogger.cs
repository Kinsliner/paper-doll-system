using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VARLive.Console
{
    public class ConsoleLogger : ILogger
    {
        private ConsoleUI consoleUI;

        public ConsoleLogger(ConsoleUI consoleUI)
        {
            this.consoleUI = consoleUI;
        }

        public void Log(string log)
        {
            if (consoleUI != null)
            {
                consoleUI.AddLog(log);
            }
        }

        public void LogError(string log)
        {
            if (consoleUI != null)
            {
                consoleUI.AddLog(log);
            }
        }

        public void LogWarning(string log)
        {
            if (consoleUI != null)
            {
                consoleUI.AddLog(log);
            }
        }
    }
}
