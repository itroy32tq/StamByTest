using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Net
{

    public class CoinComponent : MonoBehaviour
    {

        [SerializeField, Range(20f, 50f)] public float speed;

        private void Start()
        {
                
        }

        private void Update()
        {
            transform.Rotate(0, speed * Time.deltaTime, 0, Space.Self);
        }
    }
}
