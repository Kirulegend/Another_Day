using UnityEngine;
namespace ITISKIRU
{
    public class ItemObj : MonoBehaviour, Interactable
    {
        public ItemName _itemName;
        public Transform _canvasPoint;
        public string _status;
        public Renderer _material;
        void OnMouseEnter()
        {
            if(_canvasPoint) GameManager.gM.Set_boxUI(_itemName.ToString(), _status, _canvasPoint.position);
            if (!Player._isHold) KeyEvents._ke.SetUIActive(InteractionType.Take);
        }
        void OnMouseExit()
        {
            GameManager.gM.Off_boxUI();
            KeyEvents._ke.SetUIActive(InteractionType.None);
        }   
        public string GetData()
        {
            return _status;
        }

        public void OnInteract(int Mouse, Transform Player)
        {
            if (Mouse == 0) Player.GetComponent<Player>().GrabObjHand(gameObject);
        }

        public void OnInteractHand(Transform Item)
        {
            
        }
    }
}
