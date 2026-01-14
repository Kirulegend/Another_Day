using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;

namespace ITISKIRU
{
    public class Batter : MonoBehaviour, Interactable
    {
        public float currentCapacity = 10;
        [SerializeField] float totalCapacity = 10;
        public Player currentPlayer;
        void Start() => Status();
        public void Status() => GetComponent<ItemObj>()._status = $"{currentCapacity} / {totalCapacity}";

        public void OnInteract(int Mouse, Transform Script)
        {
            Debug.Log("Batter Interact");
            Player player = Script.GetComponent<Player>();
            if(player) currentPlayer = player;
        }
        public void OnInteractHand(Transform Item){}
    }   
}
