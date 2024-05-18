using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ez.Console
{
    public interface ICommand
    {
        bool IsMatch(string cmd);

        void Execute(string cmd);
    }

    public static class Commander
    {
        private static List<ICommand> commands = new List<ICommand>();

        public static void AddCommand(ICommand command)
        {
            if (commands.Contains(command) == false)
            {
                commands.Add(command);
            }
        }

        public static void RemoveCommand(ICommand command)
        {
            if (commands.Contains(command))
            {
                commands.Remove(command);
            }
        }

        public static void ExecuteCommand(string cmd)
        {
            foreach (var command in commands)
            {
                if (command.IsMatch(cmd))
                {
                    command.Execute(cmd);
                }
            }
        }
    }
}