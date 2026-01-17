using UnityEngine;

namespace ITISKIRU
{
    public class Container : MonoBehaviour, Interactable, Containable, Fillable
    {
        float FillData
        {
            get => currentCapacity / TotalCapacity;
            set
            {
                currentCapacity = Mathf.Clamp(value * TotalCapacity, 0, TotalCapacity);
                OnFillChange();
            }
        }
        [SerializeField] int TotalCapacity = 20;
        [SerializeField, Range(0, 20)] float currentCapacity = 0;
        [SerializeField] Transform _canvasPoint;
        [SerializeField] MeshRenderer fillMaterial;
        void OnFillChange()
        {
            if (fillMaterial) fillMaterial.sharedMaterial.SetFloat("_FillLevel", FillData);
        }
        void OnValidate() => OnFillChange();

        public void OnInteract(int Mouse, Transform Script) { }

        public void OnInteractHand(Transform Item) { }
        void OnMouseEnter() => GameManager.gM.Set_boxUI("Container", $"Fill: {currentCapacity}/{TotalCapacity}", _canvasPoint.position);
        void OnMouseExit() => GameManager.gM.Off_boxUI();
        public bool Check(ItemName name)
        {
            KeyEvents._ke.SetUIActive(InteractionType.Putin);
            if (currentCapacity < TotalCapacity && name == ItemName.Batter) return true;
            return false;
        }
        public void Fill(GameObject Filler)
        {
            Batter batter = Filler.GetComponent<Batter>();
            if (batter && currentCapacity < TotalCapacity && batter.currentCapacity > 0)
            {
                batter.currentCapacity -= 1f;
                currentCapacity += 1f;
                currentCapacity = Mathf.Clamp(currentCapacity, 0, TotalCapacity);
                OnFillChange();
                batter.Status();
                OnMouseEnter();
            }
        }
    }
}
