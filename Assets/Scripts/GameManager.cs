using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Net 
{
    public class GameManager : MonoBehaviourPunCallbacks
    {

        [SerializeField] private string _playerPrefName;

        [SerializeField] private InputAction _quit;
        
        private void Start()
        {
            _quit.performed += OnQuit;

            Vector2 pos = new(Random.Range(-5, 5), 1);

            var GO = PhotonNetwork.Instantiate(_playerPrefName + PhotonNetwork.NickName, pos, new Quaternion());
        }

        private void OnQuit(InputAction.CallbackContext context)
        {
            PhotonNetwork.LeaveRoom();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif !UNITY_EDITOR
            Application.Quit();   
#endif
        }
    }
}

