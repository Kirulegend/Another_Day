using UnityEngine;

namespace ITISKIRU
{
    public class HeadLook : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] float bodyWeight = 0.3f; 
        [SerializeField] float headWeight = 1.0f;
        [SerializeField] Animator anim;
        void OnAnimatorIK(int layerIndex)
        {
            anim.SetLookAtPosition(target.position);
            anim.SetLookAtWeight(1f, bodyWeight, headWeight);
        }
    }
}
