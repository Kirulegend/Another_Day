using UnityEngine;

namespace ITISKIRU
{
    public interface Interactable
    {
        void OnInteract();
        void OnInteractHand(Transform T);
    }

    public interface Storable
    {
        void OnStore(string Msg);
    }
}
