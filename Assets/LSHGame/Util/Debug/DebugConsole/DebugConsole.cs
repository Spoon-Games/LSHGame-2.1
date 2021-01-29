using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LSHGame.Util
{
    public class DebugConsole : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text textField;

        [SerializeField]
        private Button clearButton;

        [SerializeField]
        private Button closeButton;

        [SerializeField]
        private Color wariningColor = Color.yellow;

        [SerializeField]
        private Color errorColor = Color.red;

        [SerializeField]
        private GameObject content;

        private string log = "";

        private void Awake()
        {
            Application.logMessageReceived += Log;

            GameInput.ToggleDebugConsole += ToggleVisible;

            closeButton.onClick.AddListener(() => SetVisible(false));
            clearButton.onClick.AddListener(() =>
            {
                log = "";
                textField.text = log;
            });

            SetVisible(false);
        }

        public void Log(string logString, string stackTrace, LogType logType)
        {
            //if (!string.IsNullOrEmpty(stackTrace))
            //    logString += "\n" + stackTrace;

            logString = "[" + DateTime.Now.ToString("HH:mm:ss") + "]: " + logString;

            switch (logType)
            {
                case LogType.Error:
                    logString = encloseWithColor("Error " + logString,errorColor);
                    break;
                case LogType.Assert:
                    logString = "Assert " + logString;
                    break;
                case LogType.Warning:
                    logString = encloseWithColor("Warning " + logString, wariningColor);
                    break;
                case LogType.Log:
                    logString = "Log " + logString;
                    break;
                case LogType.Exception:
                    logString = encloseWithColor("Exception " + logString, errorColor);
                    break;
            }

            log += logString + "\n";

            if(log.Length > 5000)
            {
                log = log.Substring(2000);
            }
            textField.text = log;
        }

        public void ToggleVisible()
        {
            SetVisible(!content.activeSelf);
        }

        public void SetVisible(bool visible)
        {
            content.SetActive(visible);    
        }

        private string encloseWithColor(string text,Color color)
        {
            return "<color=#" + ColorUtility.ToHtmlStringRGB(color) + ">" + text + "</color>";
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= Log;
            GameInput.ToggleDebugConsole -= ToggleVisible;
        }

    }
}