using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Net 
{
    public class ProjectileController : MonoBehaviour
    {


        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField, Range(1f, 20f)] private float _moveSpeed = 3f;
        public float MoveSpeed => _moveSpeed;
        
        [SerializeField, Range(1f, 10f)] private float _damage = 3f;
        public float Damage => _damage;

        [SerializeField, Range(1f, 10f)] private float _lifeTime = 7f;
        public float LifeTime => _lifeTime;

        public Player Owner { get; private set; }

        private void Start()
        {
            Destroy(gameObject, _lifeTime);
        }

        public void InitializeBullet(Player owner, Vector2 originalDirection)
        {
            Owner = owner;

            _rigidbody.velocity = originalDirection * _moveSpeed;
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            var pl = other.GetComponent<PhotonView>();

            if (pl!=null && pl.Owner == Owner) return;

            GameManager.Instance.SetDamagePlayer(pl, _damage);

            Destroy(gameObject);
        }

    }
}

