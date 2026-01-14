using UnityEngine;

namespace ITISKIRU
{
    public class Container : MonoBehaviour, Interactable, Containable
    {
        public float FillData
        {
            get => currentCapacity / TotalCapacity;
            set
            {
                currentCapacity = Mathf.Clamp(value * TotalCapacity, 0, TotalCapacity);
                OnFillChange();
            }
        }
        public int TotalCapacity = 20;
        [Range(0, 20)] public float currentCapacity = 0;
        [SerializeField] MeshRenderer fillMaterial;
        void OnFillChange()
        {
            if (fillMaterial) fillMaterial.sharedMaterial.SetFloat("_FillLevel", FillData);
        }
        void OnValidate()
        {
            OnFillChange();
        }

        public void OnInteract(int Mouse, Transform Script) { }

        public void OnInteractHand(Transform Item)
        {
            Debug.Log("Checking Container for " + name);
            Batter batter = Item.GetComponent<Batter>();
            if (batter && currentCapacity < TotalCapacity && batter.currentCapacity > 0)
            {
                batter.currentCapacity -= 1f;
                currentCapacity += 1f;
                currentCapacity = Mathf.Clamp(currentCapacity, 0, TotalCapacity);
                OnFillChange();
                batter.Status();
            }
        }

        public bool Check(ItemName name)
        {
            KeyEvents._ke.SetUIActive(InteractionType.Putin);
            if (currentCapacity < TotalCapacity && name == ItemName.Batter) return true;
            return false;
        }
    }
}
