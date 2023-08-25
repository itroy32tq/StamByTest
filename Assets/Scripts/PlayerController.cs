using Photon.Pun;
using Spine.Unity.Examples;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Net 
{
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
    {
        private Controls _controls;

        [SerializeField]  private SpineBoyModel model;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        [SerializeField] private Transform _bulletPool;
        [SerializeField] private GameObject _bulletPref;

        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private BoxCollider2D _boxCollider;

        [Space, SerializeField, Range(1f, 10f)] private float _moveSpeed = 1f;
        public float MoveSpeed => _moveSpeed;
        
        [SerializeField] private float _maxSpeed = 10f;

        [Space, SerializeField, Range(1f, 50f)] private float _health = 5f;
        public float Health { get => _health; set => _health = value; }

        [Space, SerializeField, Range(0.1f, 1f)] private float _attackDelay = 0.4f;

        [Space, SerializeField] private Vector2 _gunPosition;

        [Space, SerializeField] private TMP_Text _nickName;
        public string NickName { get => _nickName.text; set => _nickName.text = value; }

        //[SerializeField] private GameObject _gun;

        private bool _isGun = false;
        public bool IsGun { get => _isGun; set => _isGun = value; }

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
            }
            else
            {
                ((PlayerData)stream.ReceiveNext()).Set(this);
            }

        }

        private void OnFire(CallbackContext context)
        {
            if (!photonView.IsMine) return;

            model.TryShoot();

            _isGun = true;

            if (_isGun) Debugger.Log("достать пистолет");

            
            //StartCoroutine(Fire());
        }

        private void OnTriggerEnter(Collider other)
        {
            var bullet = other.GetComponent<ProjectileController>();

            if (bullet != null || bullet.transform.parent != transform) return;//todo сделать нормально без автоубивания

            _health -= bullet.Damage;

            Destroy(bullet.gameObject);

            if (_health < 0f) Debugger.Log($"Player with name {name} is dead");
        }

        private IEnumerator Fire()
        {
      
            var bullet = PhotonNetwork.Instantiate(_bulletPref.name, transform.TransformPoint(_gunPosition), new Quaternion());

            yield return new WaitForSeconds(_attackDelay);
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

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_gunPosition, 0.2f);
        }

        private void OnDestroy()
        {
            _controls.Player.Fire.performed -= OnFire;
            _controls.Player.Disable();
        }
    }
}

