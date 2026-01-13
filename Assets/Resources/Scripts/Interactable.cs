using UnityEngine;

namespace ITISKIRU
{
    public interface Interactable
    {
        void OnInteract(int Mouse, Transform Script);
        void OnInteractHand(Transform T);
    }

    public interface Storable
    {
        void OnStore(string Msg);
    }
}
