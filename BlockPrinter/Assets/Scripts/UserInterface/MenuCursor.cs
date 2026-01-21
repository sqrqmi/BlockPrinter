using UnityEngine;

namespace BlockPrinter
{
    public class MenuCursor : MonoBehaviour
    {

        public void OnChangeSelection(Vector3 NewPosition)
        {
            this.transform.localPosition = NewPosition;
        }

        public void OnSubmit()
        {
            this.gameObject.SetActive(false);
        }
    }
}
