using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace ITISKIRU
{
    public class KeyEvents : MonoBehaviour
    {
        public static KeyEvents _ke;
        public GameObject[] _keys;
        Dictionary<InteractionType, GameObject> uiButtons = new Dictionary<InteractionType, GameObject>();
        private void Awake() => _ke = this;
        void Start()
        {
            uiButtons.Clear();
            foreach (GameObject keyButton in _keys)
            {
                if (!keyButton) continue;
                string keyName = keyButton.name;
                if (Enum.IsDefined(typeof(InteractionType), keyName))
                {
                    try
                    {
                        InteractionType type = (InteractionType)Enum.Parse(typeof(InteractionType), keyName);
                        uiButtons.Add(type, keyButton);
                        keyButton.SetActive(false);
                    }
                    catch (ArgumentException) { }
                }
            }
            SetUIActive();
        }
        public void SetUIActive(params InteractionType[] typesToActivate)
        {
            InteractionType combinedFlags = InteractionType.None;
            foreach (InteractionType type in typesToActivate) combinedFlags |= type;
            foreach (var pair in uiButtons)
            {
                InteractionType buttonType = pair.Key;
                GameObject button = pair.Value;
                bool shouldBeActive = (combinedFlags & buttonType) == buttonType;
                if (button.activeSelf != shouldBeActive) button.SetActive(shouldBeActive);
            }
        }
    }
}