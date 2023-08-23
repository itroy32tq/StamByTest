using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Net 
{
    public class Debugger
    {
        private static TMP_Text _console;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void OnStart()
        {
#if UNITY_EDITOR
            Debug.Log("Console not found!");
            return;
#endif
            _console = GameObject.FindObjectsOfType<TMP_Text>().FirstOrDefault(t => t.name == "Console");

        } 

        public static void Log(object message)
        {
            
#if UNITY_EDITOR
            Debug.Log(message);
#elif !UNITY_EDITOR
            _console.text += message;
#endif
        }
    }
}

