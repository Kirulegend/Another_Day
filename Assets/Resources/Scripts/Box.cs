using System.Collections.Generic;
using UnityEngine;
namespace ITISKIRU
{
    public class Box : MonoBehaviour, Interactable, Containable
    {
        [SerializeField] bool _opened = false;
        public bool _grabbed = false;
        [SerializeField] int _quantity;
        [SerializeField] Transform _canvasPoint;
        [SerializeField] List<Spot> _spots = new List<Spot>();
        [SerializeField] Transform _camera;
        [SerializeField] GameObject defaultObj;
        [SerializeField] Animator animator;
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
        void OnMouseEnter() => GameManager.gM.Set_boxUI(defaultItem.ToString(), GetQuantity(), _canvasPoint.position);
        void OnMouseExit()
        {
            GameManager.gM.Off_boxUI();
            if (!_grabbed && !Player._isHold) KeyEvents._ke.SetUIActive(InteractionType.None);
        }
        void OnMouseOver()
        {
            if (!Player._isHold)
            {
                if (!_grabbed && _opened) KeyEvents._ke.SetUIActive(InteractionType.Take, InteractionType.Close);
                else if (!_grabbed && !_opened) KeyEvents._ke.SetUIActive(InteractionType.Pick, InteractionType.Open);
                else if (_grabbed) KeyEvents._ke.SetUIActive(InteractionType.Place);
            }
        }
        GameObject GetItem()
        {
            foreach (Spot spot in _spots) if (spot._isOccupied && spot._spot.childCount > 0)
                {
                    spot._isOccupied = false;
                    _quantity--;
                    return spot._spot.GetChild(0).gameObject;
                }
            return null;
        }
        public bool Check(ItemName name)
        {
            KeyEvents._ke.SetUIActive(InteractionType.Putin);
            if (_quantity < _spots.Count && name == defaultItem) return true;
            return false;
        }
        public string GetQuantity()
        {
            return "Quantity " + _quantity.ToString("D2") + "/" + _spots.Count.ToString("D2");
        }
        public void OnInteract(int Mouse, Transform Player)
        {
            Debug.Log("Box OnInteract");
            if (Mouse == 0 && _grabbed) _grabbed = false;
            else if (Mouse == 0 && !_opened)
            {
                animator.SetTrigger("Move");
                animator.SetBool("Open", true);
                _opened = true;
            }
            else if (Mouse == 0 && _opened)
            {
                GameObject Temp = GetItem();
                if (Temp) Player.GetComponent<Player>().GrabObjHand(Temp);
            }
            else if (Mouse == 1 && !_opened)
            {
                Player.GetComponent<Player>().GrabObj(gameObject);
                _grabbed = true;
            }
            else if (Mouse == 1 && _opened)
            {
                animator.SetTrigger("Move");
                animator.SetBool("Open", false);
                _opened = false;
            }
        }

        public void OnInteractHand(Transform Item)
        {
            for (int i = _spots.Count - 1; i >= 0; i--)
            {
                Spot spot = _spots[i];
                if (!spot._isOccupied)
                {
                    spot._isOccupied = true;
                    _quantity++;
                    Item.SetParent(spot._spot);
                    Item.localPosition = Vector3.zero;
                    Item.localRotation = Quaternion.identity;
                    OnMouseEnter();
                    return;
                }
            }
        }
    }
}