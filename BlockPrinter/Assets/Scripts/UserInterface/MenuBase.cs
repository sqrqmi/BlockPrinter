using UnityEngine;

namespace BlockPrinter
{
    public abstract class MenuBase : MonoBehaviour
    {
        public static MenuBase RootInstance;
        protected MenuBase SuperInstance;
        protected bool IsActive;

        public abstract void Initialize(MenuBase SuperMenu);

        public abstract void SwitchActive(bool Active);

        public abstract void ReturnToSuperMenu();

        public abstract void DiscardInstances();
    }
}
