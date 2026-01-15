using UnityEngine;
using Util;

namespace BlockPrinter.UserInterface
{
    public class MenuElementView : MonoBehaviour
    {
        [SerializeField] private BitmapText LabelText;


        public void Initialize(MenuElement NewMenu)
        {
            LabelText.UpdateText(NewMenu.Label);

        }

        public void OnSelectionChange(bool IsSelecting)
        {

        }

        public void OnSubmit()
        {

        }

        public void DiscardInstances()
        {

        }
    }
}
