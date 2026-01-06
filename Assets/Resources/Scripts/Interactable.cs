using UnityEngine;

namespace ITISKIRU
{
    public interface Interactable
    {
        void OnInteract(string Msg);
    }

    public interface Storable
    {
        void OnStore(string Msg);
    }
}
