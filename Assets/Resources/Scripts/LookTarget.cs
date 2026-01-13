using Unity.VisualScripting;
using UnityEngine;

namespace ITISKIRU
{
    public class LookTarget : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] float lerpSpeed = 20f;
        void LateUpdate()
        {
            Vector3 direction = target.position - transform.position;
            if (direction.magnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(-direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lerpSpeed * Time.deltaTime);
            }
        }
    }
}
