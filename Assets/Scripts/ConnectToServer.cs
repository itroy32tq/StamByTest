using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

namespace Net 
{
    public class ConnectToServer : MonoBehaviourPunCallbacks
    {

        void Start()
        {
            PhotonNetwork.ConnectUsingSettings();
        }


        public override void OnConnectedToMaster()
        {

            Debugger.Log("Ready for connecting!");
            SceneManager.LoadScene("Lobby");
        }
    }
}

