using System.Collections.Generic;
using Unity.Hierarchy;
using UnityEngine;

namespace ITISKIRU
{
    public class PlateSlot : MonoBehaviour, Containable
    {
        [SerializeField] MeshRenderer meshRenderer;
        [SerializeField] Material sambar;
        [SerializeField] Material chutney;
        [SerializeField] Material empty;
        [SerializeField] int index;
        public bool Check(ItemName name)
        {
            if(name == ItemName.Sambar) meshRenderer.materials[index] = sambar;
            else if(name == ItemName.Chutney) meshRenderer.materials[index] = chutney;
            return true;
        }
    }
}
