using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ez.Console
{
    public interface ILogger
    {
        void Log(string log);
        void LogWarning(string log);
        void LogError(string log);
    }

    public static class ULog
    {
        private class UnityDebugLogger : ILogger
        {
            public void Log(string log)
            {
                Debug.Log(log);
            }

            public void LogError(string log)
            {
                Debug.LogError(log);
            }

            public void LogWarning(string log)
            {
                Debug.LogWarning(log);
            }
        }

        private static ILogger logger = new UnityDebugLogger();

        public static void SetLogger(ILogger logger)
        {
            ULog.logger = logger;
        }

        public static void Log(string log)
        {
            if (logger != null)
            {
                logger.Log(log);
            }
        }

        public static void LogWarning(string log)
        {
            if (logger != null)
            {
                logger.LogWarning(log);
            }
        }


        public static void LogError(string log)
        {
            if (logger != null)
            {
                logger.LogError(log);
            }
        }
    }
}

