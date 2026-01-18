using System.Collections.Generic;
using UnityEngine;

namespace ITISKIRU
{
    public class IdliPlate : MonoBehaviour, Interactable, Containable
    {
        [SerializeField] Transform _canvasPoint;
        [SerializeField] int quantity;
        [SerializeField] List<Spot> idlis = new List<Spot>();
        void OnMouseEnter()
        {
            GameManager.gM.Set_boxUI("Idli Plate", GetQuantity(), _canvasPoint.position);
            KeyEvents._ke.SetUIActive(InteractionType.Take, InteractionType.Pick);
        }
        void OnMouseExit() => GameManager.gM.Off_boxUI();
        public string GetQuantity()
        {
            return "Quantity : " + quantity + " / 4"; 
        }
        public GameObject GetItem()
        {
            foreach (Spot spot in idlis) if (spot._isOccupied && spot._spot.childCount > 0 && GetComponent<Collider>().enabled)
                {
                    spot._isOccupied = false;
                    quantity--;
                    return spot._spot.GetChild(0).gameObject;
                }
            return null;
        }
        public void OnInteract(int Mouse, Transform Player)
        {
            if (Mouse == 0)
            {
                GameObject plate = GetItem();
                if (plate) Player.GetComponent<Player>().GrabObjHand(plate);
            }
            if (Mouse == 1) Player.GetComponent<Player>().GrabObjHand(gameObject);
            OnMouseEnter();
        }
        public void OnInteractHand(Transform Item)
        {
            for (int i = idlis.Count - 1; i >= 0; i--)
            {
                Spot spot = idlis[i];
                if (!spot._isOccupied)
                {
                    quantity++;
                    spot._isOccupied = true;
                    Item.SetParent(spot._spot);
                    Item.localPosition = Vector3.zero;
                    Item.localRotation = Quaternion.identity;
                    OnMouseEnter();
                    return;
                }
            }
        }

        public bool Check(ItemName name)
        {
            if (name == ItemName.Idli) return true;
            else return false;
        }
    }
}
