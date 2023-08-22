using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Net 
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private InputAction _quit;

        private void Start()
        {
            _quit.performed += OnQuit;
        }

        private void OnQuit(InputAction.CallbackContext context)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif !UNITY_EDITOR
            Application.Quit();   
#endif
        }
    }
}

