using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class BoxRenderer : WorldLayout
    {
        public SpriteRenderer Rend;

        protected override void OnLayoutChanged()
        {
            base.OnLayoutChanged();
            Rend = this.GetComponent<SpriteRenderer>();
            Rend.size = ContentSize;
        }
    }
}
