using UnityEngine;

namespace ITISKIRU
{
    public class Container : MonoBehaviour
    {
        public float FillData
        {
            get => currentCapacity/TotalCapacity;
            set
            {
                currentCapacity += value * TotalCapacity;
                OnFillChange();
            }
        }
        public int TotalCapacity = 20;
        [Range(0, 20)] public float currentCapacity = 0;
        [SerializeField] MeshRenderer fillMaterial;
        void OnFillChange()
        {
            if (fillMaterial) fillMaterial.sharedMaterial.SetFloat("_FillLevel", currentCapacity / TotalCapacity);
        }
        void OnValidate()
        {
            OnFillChange();
        }
    }
}
