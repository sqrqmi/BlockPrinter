using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class GaugeRenderer : WorldLayout
    {
        public SpriteRenderer Rend;

        [Range(0.0f, 1.0f)]
        public float AmountRatio;


        protected override void OnLayoutChanged()
        {
            base.OnLayoutChanged();
            SetLength(AmountRatio);
        }

        public void SetLength(float NewAmountRatio)
        {
            AmountRatio = NewAmountRatio;
            Vector2 NewSize = new Vector2(ContentSize.x * AmountRatio, ContentSize.y);
            Rend.size = NewSize;
            Rend.transform.localPosition = this.transform.TransformVector((Vector3)(NewSize - ContentSize) / 2.0f);
        }


    }
}
