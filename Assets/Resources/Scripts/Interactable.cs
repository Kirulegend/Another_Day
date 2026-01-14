using UnityEngine;

namespace ITISKIRU
{
    public interface Interactable
    {
        void OnInteract(int Mouse, Transform Script);
        void OnInteractHand(Transform Item);
    }

    public interface Containable
    {
        bool Check(ItemName name);
    }
    public interface Fillable
    {
        void Fill(GameObject Filler);
    }
}
