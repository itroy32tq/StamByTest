using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Net 
{
    public class GameManager : MonoBehaviourPunCallbacks
    {

        public static GameManager Instance;
        [SerializeField] private JoystickController joystickController;
        [SerializeField] private JampComponent _jampComponent;
        [SerializeField] private GameObject _plPref;
        [SerializeField] TMP_Text _console;
        [SerializeField] GameObject _finishPanel;
        [SerializeField] TMP_Text finishText;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [SerializeField] private InputAction _quit;

        private List<PlayerController> _players = new List<PlayerController>();
        [SerializeField] private int _levelCoinsCaunt;
        public int LevelCoinCaunt { get => _levelCoinsCaunt; }


        #region UNITY
        public override void OnEnable()
        {
            _quit.Enable();
            _quit.performed += OnQuit;
        }

        private void Awake()
        {
            Instance = this;

            _finishPanel.SetActive(false);

            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {

            Spawn();

            PhotonPeer.RegisterType(typeof(PlayerData), 100, PlayerData.SerializePlayerData, PlayerData.DeSerializePlayerData);
        }

        private void OnDestroy()
        {
            _quit.performed -= OnQuit;
            _quit.Disable();
        }
        #endregion

        private async void Spawn()
        {
            await Task.Delay(TimeSpan.FromSeconds(1f));

            Vector2 pos = new(UnityEngine.Random.Range(-13, 13), -3);
            GameObject character = PhotonNetwork.Instantiate(_plPref.name, pos, new Quaternion());
            PlayerController player = character.GetComponent<PlayerController>();
            string playerID = player.GetComponent<PhotonView>().OwnerActorNr.ToString();
            joystickController.StickName = AndroidIosInput.RegisterJoystick(playerID);
            _jampComponent.Model = character.GetComponent<SpineBoyModel>();
        }

        public void AddPlayer(PlayerController player)
        {
            _players.Add(player);
        }
        public void Log(string massage)
        {
            _console.text += " " + massage;
        }

        public void SetDamagePlayer(PhotonView photonPlayer, float damage)
        {
            int index = _players.FindIndex(x => x.photonView == photonPlayer);
            if (index != -1)
            {
                PlayerController pl = _players[index];
                pl.Health -= damage;

                pl.UpdateHelthBar();

                if (pl.Health < 0f)
                {
                    Debugger.Log($"Player with name {name} is dead");
                    photonPlayer.RPC("OnDeth", RpcTarget.AllViaServer);
                    _players.Remove(pl);
                    if (_players.Count == 1) FinishGame();
                    
                }
            }
        }

        private void FinishGame()
        {
            _finishPanel.SetActive(true);
            PlayerController pl = _players[0];
            finishText.text = "Победил " + pl.NickName + " собрано " + pl.CoinCount + " монет";
        }

        public void AddCoin(PhotonView photonPlayer)
        {
            int index = _players.FindIndex(x => x.photonView == photonPlayer);
            if (index != -1)
            {
                PlayerController pl = _players[index];
                pl.CoinCount += 1;

                pl.UpdateCoinBar();

            }
        }
        #region PUN CALLBACKS
        public override void OnDisconnected(DisconnectCause cause)
        {
            SceneManager.LoadScene(1);
        }
        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
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
        #endregion
       
    }
}

