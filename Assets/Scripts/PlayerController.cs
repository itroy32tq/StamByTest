using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Spine.Unity.Examples;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

namespace Net 
{
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
    {
        private Controls _controls;

        [Header("Components")]
        [SerializeField]  private SpineBoyModel model;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private BoxCollider2D _boxCollider;

        [Space, SerializeField, Range(1f, 10f)] private float _moveSpeed = 1f;
        public float MoveSpeed => _moveSpeed;
        
        [SerializeField] private float _maxSpeed = 10f;

        [Space, SerializeField, Range(1f, 50f)] private float _health = 5f;
        public float Health { get => _health; set => _health = value; }

        private int _coinCount = 0;
        public int CoinCount { get => _coinCount; set => _coinCount = value; }

        public string StickName { get; set; }

        #region Canvas
        [Space, SerializeField] private TMP_Text _nickName;
        public string NickName { get => _nickName.text; set => _nickName.text = value; }
        [SerializeField] private Image HealthBar;
        [SerializeField] private Image CoinBar;
        private float fill;
        private float fill_max;
        private float fill_coin;
        private float fill_max_coin;
        #endregion

        public void Awake()
        {

            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
                NickName = PhotonNetwork.NickName;
            }
            DontDestroyOnLoad(gameObject);
        }

        public override void OnEnable()
        {
            _controls = new Controls();
            _controls.Player.Enable();
        }

        public void Start()
        {
            fill_max = Health;
            fill_max_coin = GameManager.Instance.LevelCoinCaunt;
            fill = 1f;
            fill_coin = 0f;
            CoinBar.fillAmount = fill_coin;
            _controls.Player.Fire.performed += OnFire;
            _controls.Player.Jump.performed += OnJump;
            GameManager.Instance.AddPlayer(this);


        }
     
        private void OnJump(CallbackContext context)
        {
            if (!photonView.IsMine) return;

            model.TryJump();
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            
            if (stream.IsWriting)
            {
                stream.SendNext(new PlayerData(this));
                stream.SendNext(NickName);
            }
            else
            {
                ((PlayerData)stream.ReceiveNext()).Set(this);
                NickName = (string)stream.ReceiveNext();
            }

        }

        private void OnFire(CallbackContext context)
        {
            if (!photonView.IsMine) return;

            model.TryShoot();

        }


        [PunRPC]
        public void OnDeth()
        {
            model.TryDeth();
        }

        public void UpdateHelthBar()
        {
            
            fill = _health / fill_max;
            HealthBar.fillAmount = fill;
        }

        public void UpdateCoinBar()
        {
            
            fill_coin = _coinCount / fill_max_coin;
            CoinBar.fillAmount = fill_coin;
        }

        private void FixedUpdate()
        {

            if (!photonView.IsMine /*|| PhotonNetwork.PlayerList.Length < 2*/)
            {
                return;
            }
            Vector2 direction;
#if UNITY_EDITOR
            direction = _controls.Player.Movement.ReadValue<Vector2>();
#elif UNITY_ANDROID
         direction = AndroidIosInput.GetJoystickValue(photonView.OwnerActorNr.ToString());   
#endif

            model.TryMove(direction.normalized.x);
            _rigidbody.velocity += direction * Time.deltaTime * _moveSpeed;
            _rigidbody.velocity = Vector2.ClampMagnitude(_rigidbody.velocity, _maxSpeed);
        }

        private void OnDestroy()
        {
            _controls.Player.Fire.performed -= OnFire;
            _controls.Player.Fire.performed -= OnJump;
            _controls.Player.Disable();
        }
    }
}

