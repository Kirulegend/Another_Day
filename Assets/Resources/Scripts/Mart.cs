using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace ITISKIRU
{
    public class Mart : MonoBehaviour
    {
        public static Mart Instance;
        [SerializeField] GameObject _itemPrefab;
        [SerializeField] Transform _itemParent;
        [SerializeField] ItemInfo[] itemInfos;
        [SerializeField] List<Item> items;
        [SerializeField] TextMeshProUGUI _cartItemQuantity;
        [SerializeField] TextMeshProUGUI _currencyText;
        [SerializeField] TextMeshProUGUI _toPayText;
        [SerializeField] TMP_Dropdown _types;
        [SerializeField] TMP_InputField searchInput;
        public int _cartItems = 0;
        public int _currency = 0;
        public int _toPay = 0;
        [SerializeField] List<string> optionTexts = new List<string>();
        [SerializeField] List<Item> filteredItems = new List<Item>();
        void Awake() => Instance = this;
        void Start()
        {
            filteredItems = new List<Item>(items);
            _currencyText.text = _currency.ToString("N2");
            Create();
            foreach (var option in _types.options) optionTexts.Add(option.text);
        }
        void Create()
        {
            foreach (ItemInfo info in itemInfos)
            {
                Item newItem = Instantiate(_itemPrefab, _itemParent).GetComponent<Item>();
                items.Add(newItem);
                newItem._price = info._price;
                newItem._itemName.text = info._name;
                newItem._itemImg.sprite = info._icon;
                newItem._itemPrice.text = info._price.ToString("N2") + " rs";
                newItem._itemType.text = info._type.ToString();
                newItem._itemTypeImg.sprite = info._typeIcon;
                newItem._itemBoxQuantity.text = info.itemQuantity.ToString();
            }
        }
        public void Pay()
        {
            if (_toPay <= _currency)
            {
                _cartItems = 0;
                _currency -= _toPay;
                _toPay = 0;
                Cart();
                foreach (Item item in items)
                {
                    item._itemQuantity.text = "00";
                    item._quantity = 0;
                }
                _types.value = 0;
            }
        }
        public void Cart()
        {
            _cartItemQuantity.text = _cartItems.ToString();
            if (_toPay > _currency) _toPayText.color = Color.red;
            else _toPayText.color = ColorUtility.TryParseHtmlString("#2D2C2D", out var c) ? c : _toPayText.color;
            _toPayText.text = _toPay.ToString("N2") + " Click To Pay";
            _currencyText.text = _currency.ToString("N2");
        }
        public void Type()
        {
            searchInput.text = string.Empty;
            if (_types.options[_types.value].text == "None") foreach (Item item in items) item.gameObject.SetActive(true);
            else
            {
                foreach (Item item in items)
                {
                    if (item._itemType.text != _types.options[_types.value].text) item.gameObject.SetActive(false);
                    else item.gameObject.SetActive(true);
                }
            }
        }
        public void Search(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
            {
                foreach (Item Item in items) Item.gameObject.SetActive(true);
                return;
            }
            else _types.value = 0;
            foreach (Item Item in items)
            {
                Item I = Item.GetComponent<Item>();
                if (I) I.gameObject.SetActive(I._itemName.text.ToLower().Contains(searchText.ToLower()));
            }
        }
    }
}
