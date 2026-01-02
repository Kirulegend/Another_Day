using UnityEngine;
namespace ITISKIRU
{
    public class ItemObj : MonoBehaviour
    {
        public ItemName _itemName;
        public Transform _canvasPoint;
        public string _status;
        void OnMouseOver() => KeyEvents._ke.SetUIActive(InteractionType.Take);
        void OnMouseExit() => KeyEvents._ke.SetUIActive(InteractionType.None);
        public string GetData()
        {
            return _status;
        }
    }
}
