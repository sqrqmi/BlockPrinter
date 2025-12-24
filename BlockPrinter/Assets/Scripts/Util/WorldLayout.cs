using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class WorldLayout : MonoBehaviour
    {
        
        public WorldLayout ParentLayout;
        public bool IsInheritStyle;
        public Vector2 ContentSize;



        public void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(this.transform.position, (Vector3)ContentSize);
        }

        public void Awake()
        {

            OnLayoutChanged();
        }

        protected virtual void OnLayoutChanged()
        {
            if(this.transform.parent != null)
            {
                ParentLayout = this.transform.parent.GetComponentInParent<WorldLayout>();
            }
            if(ParentLayout != null && IsInheritStyle)
            {
                ContentSize = ParentLayout.ContentSize;
            }
        }

        [ContextMenu("Apply Appearence")]
        public void Editor_ApplyAppearence()
        {
            OnLayoutChanged();
        }
    }
}
