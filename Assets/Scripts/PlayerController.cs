using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Spine.Unity.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public LayerMask LayerMask;
        public Vector2 Velocity;
        public float MinGroundNormalY = 0.65f;
        public float GravityModifier = 1f;

        private Vector2 targetVelocity;
        private bool grounded;
        private Vector2 groundNormal;
        private ContactFilter2D contactFilter;
        private RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
        private List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>();
        private const float minMoveDistance = 0.001f;
        private const float shellRadius = 0.01f;

        [Space, SerializeField, Range(1f, 10f)] private float _moveSpeed = 1f;
        public float MoveSpeed => _moveSpeed;

        [Space, SerializeField, Range(1f, 6f)] private float _jampForce = 6f;

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

            contactFilter.useTriggers = false;
            contactFilter.SetLayerMask(LayerMask);
            contactFilter.useLayerMask = true;


            GameManager.Instance.AddPlayer(this);


        }
     
        private void OnJump(CallbackContext context)
        {
            if (!photonView.IsMine) return;
            
            if (grounded)
            {
                Velocity.y = _jampForce;
                model.TryJump();

            }
            
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

        private void Update()
        {
            if (!photonView.IsMine /*|| PhotonNetwork.PlayerList.Length < 2*/)
            {
                return;
            }

#if UNITY_EDITOR
            targetVelocity = _controls.Player.Movement.ReadValue<Vector2>();
#elif UNITY_ANDROID
            targetVelocity = AndroidIosInput.GetJoystickValue(photonView.OwnerActorNr.ToString());   
#endif    

        }

        private void FixedUpdate()
        {

            if (!photonView.IsMine /*|| PhotonNetwork.PlayerList.Length < 2*/)
            {
                return;
            }

            Velocity += GravityModifier * Time.deltaTime * Physics2D.gravity;
            Velocity.x = targetVelocity.x;

            grounded = false;

            Vector2 deltaPosition = Velocity * Time.deltaTime;
            Vector2 moveAlongGround = new(groundNormal.y, -groundNormal.x);
            Vector2 move = moveAlongGround * deltaPosition.x * _moveSpeed;
            
            Movement(move, false);

            move = Vector2.up * deltaPosition.y;

            Movement(move, true);

            model.TryMove(Velocity.normalized.x);
        }

        private void Movement(Vector2 move, bool yMovement)
        { 
            float distance = move.magnitude;

            if (distance > minMoveDistance)
            {
                int count = _rigidbody.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
                hitBufferList.Clear();

                for (int i = 0; i < count; i++)
                {
                    hitBufferList.Add(hitBuffer[i]);
                }

                for (int i = 0; i < hitBufferList.Count; i++)
                {
                    
                    Vector2 currentNormal = hitBufferList[i].normal;
                    if (currentNormal.y > MinGroundNormalY)
                    { 
                        grounded = true;
                        if (yMovement)
                        {
                            groundNormal = currentNormal;
                            currentNormal.x = 0;
                        }
                    }
                    float projection = Vector2.Dot(Velocity, currentNormal);

                    if (projection < 0)
                    {
                        Velocity -= projection * currentNormal;
                    }

                    float modifiedDistance = hitBufferList[i].distance - shellRadius;
                    distance = modifiedDistance < distance ? modifiedDistance : distance;
                }
            }
            _rigidbody.position += move.normalized * distance;
        }

        private void OnDestroy()
        {
            _controls.Player.Fire.performed -= OnFire;
            _controls.Player.Fire.performed -= OnJump;
            _controls.Player.Disable();
        }
    }
}

