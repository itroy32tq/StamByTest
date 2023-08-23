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

        private void Start()
        {
            _createRoomButton.onClick.AddListener(OnCreateRoom);
            _joinRoomButton.onClick.AddListener(OnJoinRoom);
            _quitButton.onClick.AddListener(OnQuit);

#if UNITY_EDITOR
            PhotonNetwork.NickName = "1";
#elif !UNITY_EDITOR
            PhotonNetwork.NickName = "2";
#endif
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = "0.0.1";
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster()
        {

            Debugger.Log("Ready for connecting!");
        }

        public override void OnJoinedRoom()
        {
            PhotonNetwork.LoadLevel("Game");
            Debugger.Log("Load scene Game");
        }


        private void OnJoinRoom()
        {
            PhotonNetwork.JoinRandomRoom();
        }

        private void OnCreateRoom()
        {
            PhotonNetwork.CreateRoom(null, new Photon.Realtime.RoomOptions {MaxPlayers = 2 });
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

