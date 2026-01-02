using System.Collections.Generic;
using UnityEngine;
namespace ITISKIRU
{
    public class Box : MonoBehaviour, Interactable
    {
        public bool _opened = false;
        public bool _grabbed = false;
        public int _quantity;
        public Transform _canvasPoint;
        [SerializeField] List<Spot> _spots = new List<Spot>();
        [SerializeField] Transform _camera;
        [SerializeField] GameObject defaultObj;
        public ItemName defaultItem;
        void Start()
        {
            foreach (Transform child in transform)
            {
                if (child.name == "Spot")
                {
                    Spot newSpot = new Spot
                    {
                        _spot = child,
                        _isOccupied = false
                    };
                    _spots.Add(newSpot);
                }
            }
            if (_quantity > _spots.Count) _quantity = _spots.Count;
            for (int i = 0; i < _quantity; i++)
            {
                GameObject temp = Instantiate(defaultObj, _spots[i]._spot.position, Quaternion.identity, _spots[i]._spot);
                _spots[i]._isOccupied = true;
                temp.GetComponent<ItemObj>()._itemName = defaultItem;
            }
            _spots.Reverse();
            _canvasPoint = transform.Find("Point").transform;
            GameInput.GI_Instance.RMB_Down += GameInput_RMB_Down;
            GameInput.GI_Instance.LMB_Down += GameInput_LMB_Down;
        }
        void OnDisable()
        {
            GameInput.GI_Instance.RMB_Down -= GameInput_RMB_Down;
            GameInput.GI_Instance.LMB_Down -= GameInput_LMB_Down;
        }
        void GameInput_RMB_Down()
        {

        }
        void GameInput_LMB_Down()
        {

        }
        void OnMouseOver()
        {
            if (!Player.isHolding && !Player.isHoldingSmall)
            {
                if (!_grabbed && _opened) KeyEvents._ke.SetUIActive(InteractionType.Take, InteractionType.Close);
                else if (!_grabbed && !_opened) KeyEvents._ke.SetUIActive(InteractionType.Pick, InteractionType.Open);
                else if (_grabbed) KeyEvents._ke.SetUIActive(InteractionType.Place);
            }
        }
        void OnMouseExit()
        {
            if (!_grabbed && !Player.isHolding) KeyEvents._ke.SetUIActive(InteractionType.None);
        }
        public GameObject GetItem()
        {
            foreach (Spot spot in _spots) if (spot._isOccupied && spot._spot.childCount > 0)
                {
                    spot._isOccupied = false;
                    _quantity--;
                    return spot._spot.GetChild(0).gameObject;
                }
            return null;
        }
        public bool SetItem(GameObject item)
        {
            for (int i = _spots.Count - 1; i >= 0; i--)
            {
                Spot spot = _spots[i];
                if (!spot._isOccupied)
                {
                    spot._isOccupied = true;
                    _quantity++;
                    item.transform.SetParent(spot._spot);
                    item.transform.localPosition = Vector3.zero;
                    item.transform.localRotation = Quaternion.identity;
                    return true;
                }
            }
            return false;
        }
        public string GetQuantity()
        {
            return "Quantity " + _quantity.ToString("D2") + "/" + _spots.Count.ToString("D2");
        }

        public void OnInteract(string Msg)
        {
            Debug.Log("Interacted with Box: " + Msg);
        }
    }
}