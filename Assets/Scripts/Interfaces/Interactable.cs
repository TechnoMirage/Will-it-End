using UnityEngine;

namespace Interfaces
{
    public abstract class Interactable : MonoBehaviour
    {
        public void baseInteract()
        {
            Interact();
        }
        protected abstract void Interact();
    }
}
