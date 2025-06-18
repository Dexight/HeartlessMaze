using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Text.RegularExpressions;

public class CommandManager : MonoBehaviour
{   
    [Serializable]
    public class Command
    {
        public string name;
        public int eventId;

        //command == command
        public static bool operator ==(Command c1, Command c2)
        {
            if (ReferenceEquals(c1, c2)) return true;
            if (c1 is null || c2 is null) return false;
            return c1.name == c2.name;
        }

        //command == string
        public static bool operator ==(Command c, string s)
        {
            if (c is null || s is null) return false;
            return c.name == s;
        }

        //command != command
        public static bool operator !=(Command c1, Command c2) => !(c1 == c2);

        //command != string
        public static bool operator !=(Command c, string s) => !(c == s);
        
        //string == command
        public static bool operator ==(string s, Command c) => c == s;
        
        //string != command
        public static bool operator !=(string s, Command c) => !(c == s);

        public override bool Equals(object obj)
        {
            return obj switch
            {
                Command other => this == other,
                string name => this == name,
                _ => false
            };
        }

        public override int GetHashCode()
        {
            return name?.GetHashCode() ?? 0;
        }
    }

    public List<Command> commands;
    
    public List<UnityEvent> events;

    /// <summary>
    /// ������������ ����� ���������� �� ������������
    /// </summary>
    /// <param name="input">���������� �����</param>
    public void procedureText(string input)
    {
        Command c = commands.Find(x => input.Contains(x.name));
        if (c != (Command)null)
        {
            if (c.eventId >= 0 || c.eventId < events.Count)
                events[c.eventId].Invoke();
            else Debug.Log($"����� � ��������������� \"{c.eventId}\" �� ����������");
        }
        else
        {
            Debug.Log("������� �� �������");
        }
    }

    void Start()
    {
        MicrophoneListener.Instance.GetComponent<Client>().setCommandManager(this);

        //Normalization
        for (int i = 0; i < commands.Count; i++)
        {
            var cmd = commands[i];
            cmd.name = Regex.Replace(cmd.name.ToLower(), @"([�-��-߸�])\1", "$1");
            commands[i] = cmd;
        }
    }
}
