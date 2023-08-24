using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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
#elif !UNITY_EDITOR
            _console = GameObject.FindObjectsOfType<TMP_Text>().FirstOrDefault(t => t.name == "Console");
#endif
        }

        public static void Log(object message)
        {
            
#if UNITY_EDITOR
            Debug.Log(message);
#elif !UNITY_EDITOR
            _console.text += message;
#endif
        }
        public static void LogFormat(string format, params object[] args)
        {

#if UNITY_EDITOR
            Debug.LogFormat(format, args);
#elif !UNITY_EDITOR
            _console.text += format;
#endif
        }
    }

    public struct PlayerData
    {
        
        private float hp;
        private bool isGun;


        public PlayerData(PlayerController player)
        {
            
            hp = player.Health;
            isGun = player.IsGun;
        }
      
        public void Set(PlayerController player)
        { 
            
            player.Health = hp;
            player.IsGun = isGun;
        }

        public static byte[] SerializePlayerData(object data)
        {
            var playerData = (PlayerData)data;
            var array = new List<byte>(5);

            array.AddRange(BitConverter.GetBytes(playerData.hp));
            array.AddRange(BitConverter.GetBytes(playerData.isGun));

            return array.ToArray();
        }

        public static object DeSerializePlayerData(byte[] data)
        {
            PlayerData pl = new PlayerData
            {

                hp = BitConverter.ToSingle(data, 0),
                isGun = BitConverter.ToBoolean(data, 4)

            };
            return pl;
        }
    }
}

