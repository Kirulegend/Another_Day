using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace ITISKIRU
{
    public class Item : MonoBehaviour
    {
        public Image _itemImg;
        public Image _itemTypeImg;
        public TextMeshProUGUI _itemName;
        public TextMeshProUGUI _itemPrice;
        public TextMeshProUGUI _itemType;
        public TextMeshProUGUI _itemQuantity;
        public TextMeshProUGUI _itemBoxQuantity;
        public int _price;
        public int _quantity;

        void Awake()
        {
            _itemImg = transform.Find("Item").GetComponent<Image>();
            _itemTypeImg = transform.Find("Type").GetComponent<Image>();
            _itemName = transform.Find("Name").GetComponent<TextMeshProUGUI>();
            _itemPrice = transform.Find("PriceTag").Find("Price").GetComponent<TextMeshProUGUI>();
            _itemType = transform.Find("Type").Find("TypeName").GetComponent<TextMeshProUGUI>();
            _itemQuantity = transform.Find("BuyQuantity").GetComponent<TextMeshProUGUI>();
            _itemBoxQuantity = transform.Find("Quantity").GetComponent<TextMeshProUGUI>();
        }

        public void AddQuantity()
        {
            if (_quantity < 10)
            {
                _quantity++;
                Mart.Instance._toPay += _price;
                Mart.Instance._cartItems++;
                Mart.Instance.Cart();
            }
            if (_quantity != 10) _itemQuantity.text = "0" + _quantity.ToString();
            else _itemQuantity.text = _quantity.ToString();
        }
        public void SubQuantity()
        {
            if (_quantity > 0)
            {
                _quantity--;
                Mart.Instance._toPay -= _price;
                Mart.Instance._cartItems--;
                Mart.Instance.Cart();
            }
            _itemQuantity.text = "0" + _quantity.ToString();
        }
    }
}