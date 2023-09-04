using System;
using System.Collections.Generic;
using UnityEngine;

namespace epoHless.Logger
{
    [DisallowMultipleComponent]
    public class Logger : MonoBehaviour
    {
        [Header("SETTINGS")]
        [SerializeField, Range(5, 50)] private int entries = 15;
        private int Entries
        {
            get => entries;
            set
            {
                entries = value;
                if (entries < 5) entries = 5;
                else if (entries > 50) entries = 50;
            }
        }

        #region Bools

        [SerializeField] private bool showLog = true;
        private string LogBtnText => showLog ? "Hide Log" : "Show Log";

        [SerializeField] private bool showStack = false;
        private string StackBtnText => showStack ? "Hide Stack" : "Show Stack";
        private int _stackIndex = 0;
        
        #endregion

        #region Colors

        [Header("COLORS")]
        [SerializeField] private Color log = Color.green;
        [SerializeField] private Color warning = Color.yellow;
        [SerializeField] private Color error = Color.red;
        [SerializeField] private Color exception = Color.red;
        [SerializeField] private Color assert = Color.magenta;

        #endregion

        #region Text

        [Header("Labels")]
        [SerializeField, Range(10, 50)] private int genericSize = 10;

        #endregion
        
        private readonly List<LogEntry> _logs = new List<LogEntry>();

        #region Unity Methods

        private void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }

        #endregion

        private void HandleLog(string condition, string stacktrace, LogType type)
        {
            _logs.Add(new LogEntry(condition, stacktrace, type));
            if (_logs.Count > Entries) _logs.RemoveRange(0, _logs.Count - entries);
        }

        private void OnGUI()
        {
            var logButtonRect = new Rect(5, 5, 75, 25);
            if (GUI.Button(logButtonRect, LogBtnText)) { showLog = !showLog; }
            
            if (!showLog) return;

            var stackButtonRect = new Rect(85, 5, 85, 25);
            if (GUI.Button(stackButtonRect, StackBtnText)) showStack = !showStack;
            
            var infoLabelRect = new Rect(185, 5, 85, 25);
            GUI.Label(infoLabelRect, $"Entries: {Entries}");
            
            var plusCapacityButtonRect = new Rect(285, 5, 25, 25);
            if (GUI.Button(plusCapacityButtonRect, "-")) 
            {
                Entries -= 5;
                if (_logs.Count > Entries) _logs.RemoveRange(0, _logs.Count - entries);
            }
            
            var minusCapacityButtonRect = new Rect(325, 5, 25, 25);
            if (GUI.Button(minusCapacityButtonRect, "+")) { Entries += 5; }
            
            var clearLogButtonRect = new Rect(365, 5, 100, 25);
            if (GUI.Button(clearLogButtonRect, "Clear Log")) { _logs.Clear(); }

            for (int i = _logs.Count - 1; i >= 0; i--)
            {
                RenderLog(_logs[i], i);
            }

            if (showStack && _logs.Count > 0)
            {
                var position = Screen.height - 155;
                var stackRect = new Rect(5, position, 1000, 150);
                
                GUI.Box(stackRect, "");
                GUI.Label(stackRect, _logs[_stackIndex].StackTrace);
            }
        }

        private void RenderLog(LogEntry entry, int i)
        {
            var color = GetLogColor(entry);
            var style = GetGUIStyle(color);

            var yPosition = 35 + (genericSize * i);
            
            var labelPosition = new Vector2(25 + genericSize, yPosition);
            
            var labelRect = new Rect(labelPosition, new Vector2(1000, 20));
            GUI.Label(labelRect, $"{entry.Message}", style);
            
            var buttonPosition = new Vector2(10, yPosition);

            var buttonRect = new Rect(buttonPosition, new Vector2(genericSize, genericSize));
            if (GUI.Button(buttonRect, ">")) { _stackIndex = i; }
        }

        private GUIStyle GetGUIStyle(Color color)
        {
            return new GUIStyle()
            {
                normal = new GUIStyleState()
                {
                    textColor = color
                },
                fontStyle = FontStyle.Bold,
                fontSize = genericSize
            };
        }

        private Color GetLogColor(LogEntry entry)
        {
            return (entry.Type) switch
            {
                LogType.Error => error,
                LogType.Assert => assert,
                LogType.Warning => warning,
                LogType.Log => log,
                LogType.Exception => exception,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}