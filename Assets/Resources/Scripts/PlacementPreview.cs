using UnityEngine;
using System.Collections.Generic;
namespace ITISKIRU
{
    public class PlacementPreview : MonoBehaviour
    {
        [SerializeField] int collisionCount = 0;
        public bool HasCollision()
        {
            return collisionCount > 0;
        }
        void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger) return;
            collisionCount++;
        }
        void OnTriggerExit(Collider other)
        {
            if (other.isTrigger) return;
            if (collisionCount > 0) collisionCount--;
        }
        void OnEnable() => collisionCount = 0;
    }
}
