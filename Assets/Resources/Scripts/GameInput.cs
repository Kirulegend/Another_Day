using System;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
namespace ITISKIRU
{
    public class GameInput : MonoBehaviour
    {
        InputSystem inputActions;
        public static GameInput GI_Instance;
        public event Action LMB_Down;
        public event Action LMB_Hold;
        public event Action RMB_Down;
        public event Action RMB_Up;
        public event Action LSK_Down;
        public event Action ESC_Down;
        void Awake()
        {
            if (!GI_Instance) GI_Instance = this;
            else Destroy(gameObject);

            inputActions = new InputSystem();
            inputActions.Enable();

            inputActions.Player.LMB_Interact.performed += _ => LMB_Down?.Invoke();
            inputActions.Player.RMB_Interact.performed += _ => RMB_Down?.Invoke();
            inputActions.Player.RMB_Interact.canceled += _ => RMB_Up?.Invoke();
            inputActions.Player.LSK_Interact.performed += _ => LSK_Down?.Invoke();
            inputActions.Player.ESC_Interact.performed += _ => ESC_Down?.Invoke();
        }
        void OnDisable() => inputActions.Disable();
    }
}
