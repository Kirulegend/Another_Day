using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;

namespace ITISKIRU
{
    public class Batter : MonoBehaviour, Interactable
    {

        public float TotalCapacity = 10;
        public void OnInteract()
        {
            
        }
        public void OnInteractHand(Transform Container)
        {
            Container container = Container.GetComponent<Container>();
            if (container && container.currentCapacity < container.TotalCapacity)
            {
                TotalCapacity -= .01f;
                container.FillData += 01f;
            }
        }


    }
}
