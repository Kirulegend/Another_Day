using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace ITISKIRU
{
    public class IdliSteamer : MonoBehaviour, Interactable, Containable
    {
        [SerializeField] Transform _canvasPoint;
        [SerializeField] Vector3 capPos = new Vector3(0f, 0.55f, 0f);
        [SerializeField] Transform cap;
        [SerializeField] bool isSteaming = false;
        [SerializeField] List<Spot> idliPlates = new List<Spot>();
        void OnMouseEnter()
        {
            GameManager.gM.Set_boxUI("Idli Steamer", GetQuantity(), _canvasPoint.position);
            if(cap.localPosition != capPos) KeyEvents._ke.SetUIActive(InteractionType.TakeR);
        }
        void OnMouseExit() => GameManager.gM.Off_boxUI();
        public string GetQuantity()
        {
            if(isSteaming) return "Steaming";
            else return "Idle";
        }
        public GameObject GetItem()
        {
            if (!isSteaming && capPos != cap.localPosition)
            {
                foreach (Spot spot in idliPlates) if (spot._isOccupied && spot._spot.childCount > 0)
                {
                    spot._isOccupied = false;
                    return spot._spot.GetChild(0).gameObject;
                }
            }
            return null;
        }
        public void OnInteract(int Mouse, Transform Player)
        {
            if(Mouse == 1)
            {
                GameObject plate = GetItem();
                if (plate) Player.GetComponent<Player>().GrabObjHand(plate);
            }
            OnMouseEnter();
        }
        public void OnInteractHand(Transform Item)
        {
            if (Item == cap)
            {
                Item.GetComponent<Collider>().enabled = true;
                Item.SetParent(this.transform);
                Item.localPosition = capPos;
                Item.rotation = Quaternion.identity;
            }
            else
            {
                for (int i = idliPlates.Count - 1; i >= 0; i--)
                {
                    Spot spot = idliPlates[i];
                    if (!spot._isOccupied)
                    {
                        spot._isOccupied = true;
                        Item.SetParent(spot._spot);
                        Item.localPosition = Vector3.zero;
                        Item.localRotation = Quaternion.identity;
                        OnMouseEnter();
                        return;
                    }
                }
            }

        }

        public bool Check(ItemName name)
        {
            if (cap.position != capPos && name == ItemName.Idli) return true;            
            else return false;
        }
    }
}
