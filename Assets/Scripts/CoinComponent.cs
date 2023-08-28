using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Net
{

    public class CoinComponent : MonoBehaviour
    {

        [SerializeField, Range(20f, 50f)] private float speed;
        [SerializeField] private AudioSource _source;

        
        private void Update()
        {
            transform.Rotate(0, speed * Time.deltaTime, 0, Space.Self);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            var pl = other.GetComponent<PhotonView>();

            if (pl == null) return;

            _source.Play();

            GameManager.Instance.AddCoin(pl);

            Destroy(gameObject);
        }
    }
}
