using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;

namespace ITISKIRU
{
    public class Batter : MonoBehaviour
    {
        public float currentCapacity = 10;
        [SerializeField] float totalCapacity = 10;
        void Start() => Status();
        public void Status() => GetComponent<ItemObj>()._status = $"{currentCapacity} / {totalCapacity}";
    }   
}
