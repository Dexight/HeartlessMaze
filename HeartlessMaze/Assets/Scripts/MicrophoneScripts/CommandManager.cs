using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Text.RegularExpressions;
using UnityEngine.Windows;

public class CommandManager : MonoBehaviour
{   
    [Serializable]
    public struct Command
    {
        public string name;
        public int eventId;

        public Command(string name, int eventId)
        {
            name = name.ToLower();
            string pattern = @"([а-яА-ЯёЁ])\1";
            string result = Regex.Replace(name, pattern, "$1");
            this.name = result;

            this.eventId = eventId;
        }

        // Command != Command
        public static bool operator ==(Command left, Command right)
        {
            return left.name == right.name;
        }

        // Command != Command
        public static bool operator !=(Command left, Command right)
        {
            return left.name != right.name;
        }

        // Command == string
        public static bool operator ==(Command command, string name)
        {
            return command.name == name;
        }

        // Command != string
        public static bool operator !=(Command command, string name)
        {
            return command.name != name;
        }

        // string == Command
        public static bool operator ==(string name, Command command)
        {
            return name == command.name;
        }

        // string != Command
        public static bool operator !=(string name, Command command)
        {
            return name != command.name;
        }

        public override readonly bool Equals(object obj)
        {
            return obj switch
            {
                Command other => this == other,
                string name => this == name,
                _ => false
            };
        }

        public override readonly int GetHashCode()
        {
            return name?.GetHashCode() ?? 0;
        }
    }

    public List<Command> commands;
    
    public List<UnityEvent> events;

    /// <summary>
    /// Обрабатывает текст полученный от пользователя
    /// </summary>
    /// <param name="input">полученный текст</param>
    public void procedureText(string input)
    {
        Command c = commands.Find(x => input.Contains(x.name));
        if (c != null)
        {
            events[c.eventId].Invoke();
        }
    }

    //void Start()
    //{

    //}

    //void Update()
    //{

    //}
}
