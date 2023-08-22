using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Net 
{
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField, Range(1f, 20f)] private float _moveSpeed = 3f;
        public float MoveSpeed => _moveSpeed;
        
        [SerializeField, Range(1f, 10f)] private float _damage = 3f;
        public float Damage => _damage;

        [SerializeField, Range(1f, 10f)] private float _lifeTime = 7f;
        public float LifeTime => _lifeTime;

        private void Start()
        {
            StartCoroutine(OnDie());
        }

        private IEnumerator OnDie()
        { 
            yield return new WaitForSeconds(_lifeTime);
            Destroy(gameObject);
        }
        private void Update()
        {
            transform.position += transform.right * _moveSpeed * Time.deltaTime;
        }
    }
}

