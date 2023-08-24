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
        public float posX;
        public float posY;
        public float hp;

        public static PlayerData Create(PlayerController player)
        {
            return new PlayerData
            {
                posX = player.transform.position.x,
                posY = player.transform.position.y,
                hp = player.Health
            };
        }

        public void Set(PlayerController player)
        { 
            Vector2 pos = player.transform.position;
            pos.x = posX; pos.y = posY;
            player.Health = hp;
        }

        public static byte[] SerializePlayerData(object data)
        {
            var player = (PlayerData)data;
            var array = new List<byte>(12);

            array.AddRange(BitConverter.GetBytes(player.posX));
            array.AddRange(BitConverter.GetBytes(player.posY));
            array.AddRange(BitConverter.GetBytes(player.hp));

            return array.ToArray();
        }

        public static object DeSerializePlayerData(byte[] data)
        {
            PlayerData pl = new PlayerData
            {
                posX = BitConverter.ToSingle(data, 0),
                posY = BitConverter.ToSingle(data, 4),
                hp = BitConverter.ToSingle(data, 8)

            };
            Debugger.Log(pl.posX);
            return pl;
        }
    }
}

