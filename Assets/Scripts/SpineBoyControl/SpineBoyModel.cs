using Photon.Pun;
using System.Collections;
using UnityEngine;


namespace Net 
{
    [SelectionBase]
    public class SpineBoyModel : MonoBehaviour, IPunObservable
    {
        #region Inspector
        [Header("Current State")]
        public SpineBeginnerBodyState state;
        public bool facingLeft;

        [SerializeField] private PhotonView _photonView;

        [Range(-1f, 1f)]
        public float currentSpeed;

        [Header("Balance")]
        public float shootInterval = 0.12f;
        [Space, SerializeField] private Vector2 _gunPosition;
        [SerializeField] private GameObject _bulletPref;
        #endregion

        float lastShootTime;
        public event System.Action ShootEvent;  // Lets other scripts know when Spineboy is shooting. Check C# Documentation to learn more about events and delegates.
        public event System.Action DethEvent;
    

        #region API
        public void TryJump()
        {
            StartCoroutine(JumpRoutine());
        }

        public void TryDeth()
        {

            _photonView.RPC("Deth", RpcTarget.AllViaServer);

            
        }

        public void TryShoot()
        {
            float currentTime = Time.time;
 
            if (currentTime - lastShootTime > shootInterval)
            {
                lastShootTime = currentTime;
                
                _photonView.RPC("Fire", RpcTarget.AllViaServer);

            }
        }

        [PunRPC]
        public void Fire()
        {

            var vector = _gunPosition;
            var dir = Vector2.left;
            if (facingLeft) vector.x = -_gunPosition.x;
            else dir = Vector2.right;
            
            GameObject bullet = Instantiate(_bulletPref, transform.TransformPoint(vector), Quaternion.identity);
            bullet.GetComponent<ProjectileController>().InitializeBullet(_photonView.Owner, dir);

            if (ShootEvent != null) ShootEvent();

        }
        [PunRPC]
        public void Deth()
        {
            if (DethEvent != null && _photonView.IsMine) DethEvent();
        }

        public void TryMove(float speed)
        {
            
            if (speed != 0)
            {
                bool speedIsNegative = (speed < 0f);
                facingLeft = speedIsNegative; // Change facing direction whenever speed is not 0.
            }

            if (state != SpineBeginnerBodyState.Jumping)
            {
                state = (speed == 0) ? SpineBeginnerBodyState.Idle : SpineBeginnerBodyState.Running;
            }

        }
        #endregion

        IEnumerator JumpRoutine()
        {
            if (state == SpineBeginnerBodyState.Jumping) yield break;   // Don't jump when already jumping.

            state = SpineBeginnerBodyState.Jumping;

            // Fake jumping.
            {
                Vector3 pos = transform.localPosition;
                const float jumpTime = 1.2f;
                const float half = jumpTime * 0.5f;
                const float jumpPower = 30f;
                for (float t = 0; t < half; t += Time.deltaTime)
                {
                    float d = jumpPower * (half - t);
                    transform.Translate((d * Time.deltaTime) * Vector3.up);
                    yield return null;
                }
                for (float t = 0; t < half; t += Time.deltaTime)
                {
                    float d = jumpPower * t;
                    transform.Translate((d * Time.deltaTime) * Vector3.down);
                    yield return null;
                }
            }

            state = SpineBeginnerBodyState.Idle;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(state);
                stream.SendNext(facingLeft);
            }
            else
            {
                state = (SpineBeginnerBodyState) stream.ReceiveNext();
                facingLeft = (bool)stream.ReceiveNext();
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(_gunPosition, 0.2f);
        }
    }

    public enum SpineBeginnerBodyState
    {
        Idle,
        Running,
        Jumping
    }
}

