using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Net 
{
    public class GameManager : MonoBehaviourPunCallbacks
    {

        [SerializeField] private string _playerPrefName;
        [SerializeField] private GameObject _plPref;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [SerializeField] private InputAction _quit;

        public override void OnEnable()
        {
            _quit.Enable();
            _quit.performed += OnQuit;
        }

        private void Awake()
        {
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {

            Spawn();

            //PhotonPeer.RegisterType(typeof(PlayerData), 100, PlayerData.SerializePlayerData, PlayerData.DeSerializePlayerData);
        }

        private async void Spawn()
        {
            await Task.Delay(System.TimeSpan.FromSeconds(1f));

            Vector2 pos = new(Random.Range(-5, 5), 1);
            GameObject GO = PhotonNetwork.Instantiate(_plPref.name, pos, new Quaternion());
            
        }


        public override void OnPlayerEnteredRoom(Player newPlayer)
        {

            Debug.LogFormat("Player {0} entered room", newPlayer.NickName);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.LogFormat("Player {0} left room", otherPlayer.NickName);
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

