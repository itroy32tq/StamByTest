using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace Net 
{
    public class PlayerController : MonoBehaviour
    {
        private Controls _controls;

        [SerializeField] private Transform _bulletPool;

        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private CapsuleCollider2D _capsuleCollider;
        [SerializeField] private ProjectileController _bulletPref;

        [Space, SerializeField, Range(1f, 10f)] private float _moveSpeed = 1f;
        public float MoveSpeed => _moveSpeed;
        
        [SerializeField] private float _maxSpeed = 10f;

        [Space, SerializeField, Range(1f, 50f)] private float _health = 5f;
        public float Health => _health;

        [Space, SerializeField, Range(0.1f, 1f)] private float _attackDelay = 0.4f;

        [Space, SerializeField] Vector2 _gunPosition;

        
        private void OnEnable()
        {
            _controls = new Controls();
            _controls.Player.Enable();
        }

        private void Start()
        {

            _controls.Player.Fire.performed += OnFire;
        }

        private void OnFire(CallbackContext context)
        {
            StartCoroutine(Fire());
        }

        private void OnTriggerEnter(Collider other)
        {
            var bullet = other.GetComponent<ProjectileController>();

            if (bullet != null || bullet.transform.parent != transform) return;//todo сделать нормально без автоубивания

            _health -= bullet.Damage;
            Destroy(bullet.gameObject);

            if (_health < 0f) Debug.Log($"Player with name {name} is dead");
        }

        private IEnumerator Fire()
        {
            var bullet = Instantiate(_bulletPref, _bulletPool);
            bullet.transform.position = transform.TransformPoint(_gunPosition);
            yield return new WaitForSeconds(_attackDelay);
        }

        private void FixedUpdate()
        {
            Vector2 direction = _controls.Player.Movement.ReadValue<Vector2>();
            
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

