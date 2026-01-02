using UnityEngine;

namespace ITISKIRU
{
    public class Mirror : MonoBehaviour
    {
        [SerializeField] Transform Player;
        void Update()
        {
            Vector3 Refvector = Vector3.Reflect(transform.position - Player.position, -Vector3.right);
            transform.rotation = Quaternion.LookRotation((new Vector3(Refvector.x, -Refvector.y/2, Refvector.z)), Vector3.up);
        }
    }
}
