using Photon.Pun;
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



        #region Canvas
        [Space, SerializeField] private TMP_Text _nickName;
        public string NickName { get => _nickName.text; set => _nickName.text = value; }
        [SerializeField]  private Image bar;
        private float fill;
        private float fill_max;

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
            fill = 1f;
            _controls.Player.Fire.performed += OnFire;
            _controls.Player.Jump.performed += OnJump;

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
                stream.SendNext(fill);
            }
            else
            {
                ((PlayerData)stream.ReceiveNext()).Set(this);
                NickName = (string)stream.ReceiveNext();
                fill = (float)stream.ReceiveNext();
            }

        }

        private void OnFire(CallbackContext context)
        {
            if (!photonView.IsMine) return;

            model.TryShoot();

        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            
            var bullet = other.GetComponent<ProjectileController>();

            Debug.Log(bullet.name);
            Debug.Log(_health);

            _health -= bullet.Damage;
            Debug.Log(_health);

            bar.fillAmount = fill;
            fill = _health / fill;


            if (_health < 0f) Debugger.Log($"Player with name {name} is dead");
        }
   
        private void FixedUpdate()
        {

            if (!photonView.IsMine)
            {
                return;
            } 

            Vector2 direction = _controls.Player.Movement.ReadValue<Vector2>();

            model.TryMove(direction.normalized.x);
            _rigidbody.velocity += direction * Time.deltaTime * _moveSpeed;
            _rigidbody.velocity = Vector2.ClampMagnitude(_rigidbody.velocity, _maxSpeed);
        }

        private void OnDestroy()
        {
            _controls.Player.Fire.performed -= OnFire;
            _controls.Player.Disable();
        }
    }
}

