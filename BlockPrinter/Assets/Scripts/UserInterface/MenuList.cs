using System;
using UnityEngine;

namespace BlockPrinter.UserInterface
{
    [Serializable]
    public struct MenuElement
    {
        public string Label;
        public Action OnSelect;
    }

    public class MenuList : MonoBehaviour
    {
        public static MenuList BaseInstance;

        private MenuList SuperListInstance;

        [SerializeField] private MenuElementView MenuElementPrefab;
        [SerializeField] private GameObject CursorPrefab;
        [SerializeField] private GameObject MenuPivot;
        [SerializeField] private FieldLayout AlignmentLayout;
        [SerializeField] private MenuElement[] MenuElements;

        public GameObject CursorInstance;
        private MenuElementView[] MenuElementViewInstances;
        private int CurrentSelectingIndex;
        private bool IsActive;

        private void Update()
        {
            // if(Input.GetKeyDown())


            if(Input.GetKeyDown("Submit"))
            {
                Submit();
                ReturnToSuperList();
            }


        }

        public void Initialize(MenuList SuperList)
        {
            if(SuperList == null)
            {
                BaseInstance = this;
            }
            SuperListInstance = SuperList;  
            MenuElementViewInstances = new MenuElementView[MenuElements.Length];
            for(int i = 0; i < MenuElements.Length; i++)
            {
                MenuElementView NewView = Instantiate(MenuElementPrefab);
                NewView.transform.SetParent(MenuPivot.transform);
                NewView.transform.localPosition = AlignmentLayout.Transform(i * Vector2Int.right);
                NewView.Initialize(MenuElements[i]);
                MenuElementViewInstances[i] = NewView;
            }
            CursorInstance = Instantiate(CursorPrefab);
            CurrentSelectingIndex = 0;
            ChangeSelection(0);
            IsActive = true;
        }

        public void Override(MenuList NewList)
        {
            NewList.Initialize(this);
            IsActive = false;
        }

        public void ReturnToSuperList()
        {
            if(SuperListInstance != null)
            {
                SuperListInstance.SwitchActive(true);
            }
            DiscardInstances();
        }

        public void SwitchActive(bool Active)
        {
            if (IsActive != Active)
            {
                IsActive = Active;
                MenuElementViewInstances[CurrentSelectingIndex].OnSelectionChange(IsActive);
            }
        }

        public void ChangeSelection(int NewIndex)
        {
            MenuElementViewInstances[CurrentSelectingIndex].OnSelectionChange(false);
            CurrentSelectingIndex = NewIndex;
            MenuElementViewInstances[CurrentSelectingIndex].OnSelectionChange(true);
            CursorInstance.transform.localPosition = AlignmentLayout.Transform(CurrentSelectingIndex * Vector2Int.right);
        }

        public void Submit()
        {
            MenuElements[CurrentSelectingIndex].OnSelect();
        }

        public void DiscardInstances()
        {
            for(int i = 0; i < MenuElementViewInstances.Length; i++)
            {
                Destroy(MenuElementViewInstances[i].gameObject);
            }
            Destroy(CursorInstance.gameObject);

        }

    }
}
