using UnityEngine;
using Util;

namespace BlockPrinter
{
    public class MenuCursor : MonoBehaviour
    {
        public void Initialize(Vector3 NewPosition)
        {
            this.transform.localPosition = NewPosition;
        }

        public void OnChangeSelection(Vector3 NewPosition)
        {
            LinearMovement.Create(this.gameObject, this.transform.localPosition, NewPosition, 0.2f, InterpolationMode.QuadraticBrake);
        }

        public void OnSubmit()
        {
            this.gameObject.SetActive(false);
        }
    }
}
