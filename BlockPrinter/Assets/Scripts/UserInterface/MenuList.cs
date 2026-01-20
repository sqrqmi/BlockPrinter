using System;
using UnityEngine;

namespace BlockPrinter.UserInterface
{
    [Serializable]
    public struct MenuElement
    {
        public string Label;
        public GameObject EventHandlerObject;
        public string OnSelectMethodName;
    }

    public class MenuList : MonoBehaviour
    {
        public static MenuList RootInstance;

        private MenuList SuperListInstance;

        [SerializeField] private MenuElementView MenuElementPrefab;
        [SerializeField] private GameObject CursorPrefab;
        [SerializeField] private GameObject MenuPivot;
        [SerializeField] private FieldLayout AlignmentLayout;
        [SerializeField] private Util.Direction SortDirection;
        [SerializeField] private MenuElement[] MenuElements;
        [SerializeField] private bool AllowCancel;

        private GameObject CursorInstance;
        private MenuElementView[] MenuElementViewInstances;
        private int CurrentSelectingIndex;
        private bool IsActive;

        private void Update()
        {
            if(!IsActive)
            {
                return;
            }
            if(Input.GetAxis("Vertical") < -0.5f)
            {
                ChangeSelection(CurrentSelectingIndex + 1);
            }
            if(Input.GetAxis("Vertical") > +0.5f)
            {
                ChangeSelection(CurrentSelectingIndex - 1);
            }
            if(AllowCancel && Input.GetKeyDown(KeyCode.Escape))
            {
                ReturnToSuperList();
            }
            if(Input.GetKeyDown(KeyCode.Return))
            {
                Submit();
                ReturnToSuperList();
            }


        }

        public void Initialize(MenuList SuperList)
        {
            if(SuperList == null)
            {
                RootInstance = this;
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
            CursorInstance.transform.SetParent(MenuPivot.transform);
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
            SwitchActive(false);
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
            if(NewIndex < 0 || MenuElements.Length <= NewIndex)
            {
                return;
            }
            MenuElementViewInstances[CurrentSelectingIndex].OnSelectionChange(false);
            CurrentSelectingIndex = NewIndex;
            MenuElementViewInstances[CurrentSelectingIndex].OnSelectionChange(true);
            CursorInstance.transform.localPosition = AlignmentLayout.Transform(CurrentSelectingIndex * Vector2Int.right);
        }

        public void Submit()
        {
            MenuElements[CurrentSelectingIndex].EventHandlerObject.SendMessage(MenuElements[CurrentSelectingIndex].OnSelectMethodName);
        }

        public void DiscardInstances()
        {
            for(int i = 0; i < MenuElementViewInstances.Length; i++)
            {
                if(MenuElementViewInstances[i] != null)
                {
                    Destroy(MenuElementViewInstances[i].gameObject);
                }
            }
            Destroy(CursorInstance.gameObject);

        }

    }
}
