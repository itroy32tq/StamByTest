using System;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Net 
{
    public class LobbiMenuManager : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TMP_Text _console;
        [SerializeField] private Button _createRoomButton;
        [SerializeField] private Button _joinRoomButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private TMP_InputField _createInput;
        [SerializeField] private TMP_InputField _joinInput;

        private void Start()
        {
            _createRoomButton.onClick.AddListener(OnCreateRoom);
            _joinRoomButton.onClick.AddListener(OnJoinRoom);
            _quitButton.onClick.AddListener(OnQuit);


            PhotonNetwork.NickName = "Player_" + UnityEngine.Random.Range(1, 5);
            PhotonNetwork.GameVersion = "1";
            PhotonNetwork.AutomaticallySyncScene = true;

        }


        private void OnCreateRoom()
        {
            PhotonNetwork.CreateRoom(_createInput.text, new Photon.Realtime.RoomOptions { MaxPlayers = 2 });
        }

        private void OnJoinRoom()
        {
            PhotonNetwork.JoinRoom(_joinInput.text);
        }

        public override void OnJoinedRoom()
        {
            Debugger.Log("Load scene Game");
            PhotonNetwork.LoadLevel("Game");
        }

        private void OnQuit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif !UNITY_EDITOR
            Application.Quit();   
#endif
        }
    }
}

