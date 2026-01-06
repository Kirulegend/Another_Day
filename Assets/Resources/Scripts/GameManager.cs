using System;
using TMPro;
using UnityEngine;
namespace ITISKIRU
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] Canvas _canvas;
        [SerializeField] Canvas _box;
        public static Action<bool> isInteractAction;
        void OnEnable() => Player.OnInteraction += CanvasCheck;

        void OnDisable() => Player.OnInteraction -= CanvasCheck;
        void CanvasCheck(bool isInteract)
        {
            if (isInteract) _canvas.enabled = false;
            else _canvas.enabled = true;
        }
        public Transform Set_boxUI(string Name, string Quantity, Vector3 Position)
        {
            _box.transform.Find("Panel").Find("Box Name").GetComponent<TextMeshProUGUI>().text = Name;
            _box.transform.Find("Panel").Find("Box Details").GetComponent<TextMeshProUGUI>().text = Quantity;
            _box.transform.position = Position;
            _box.gameObject.SetActive(true);
            return _box.transform;
        }
    }
    [Serializable]
    public class Spot
    {
        public Transform _spot;
        public bool _isOccupied = false;
    }
}