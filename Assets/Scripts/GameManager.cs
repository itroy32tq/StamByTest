using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Net 
{
    public class GameManager : MonoBehaviourPunCallbacks
    {

        [SerializeField] private string _playerPrefName;

        [SerializeField] private InputAction _quit;

        public override void OnEnable()
        {
            _quit.Enable();
            _quit.performed += OnQuit;
        }

        private void Start()
        {
            _quit.performed += OnQuit;

            Vector2 pos = new(Random.Range(-5, 5), 1);

            var GO = PhotonNetwork.Instantiate(_playerPrefName + PhotonNetwork.NickName, pos, new Quaternion());

            PhotonPeer.RegisterType(typeof(PlayerData), 100, PlayerData.SerializePlayerData, PlayerData.DeSerializePlayerData);
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
        private void OnDestroy()
        {
            _quit.performed -= OnQuit;
            _quit.Disable();
        }
    }
}

