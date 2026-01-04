using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class GaugeRenderer : WorldLayout
    {
        public enum FillDirection
        {
            Right,
            Left,
            Up,
            Down,
        }

        [SerializeField] private SpriteRenderer Rend;

        [SerializeField] private FillDirection Direction;
        [Range(0.0f, 1.0f)]
        [SerializeField] private float AmountRatio;


        protected override void OnLayoutChanged()
        {
            base.OnLayoutChanged();
            SetLength(AmountRatio);
        }

        public void SetLength(float NewAmountRatio)
        {
            AmountRatio = NewAmountRatio;
            Vector2 NewSize = Vector2.zero;
            Vector2 NewPosition = Vector2.zero;
            switch (Direction)
            {
                case FillDirection.Left:
                case FillDirection.Right:
                    NewSize = new Vector2(ContentSize.x * AmountRatio, ContentSize.y);
                    break;
                case FillDirection.Up:
                case FillDirection.Down:
                    NewSize = new Vector2(ContentSize.x, ContentSize.y * AmountRatio);
                    break;

            }
            switch (Direction)
            {
                case FillDirection.Left:
                case FillDirection.Down:
                    NewPosition = (NewSize - ContentSize) / 2.0f;
                    break;
                case FillDirection.Right:
                case FillDirection.Up:
                    NewPosition = NewSize / 2.0f;
                    break;
            }
            Rend.size = NewSize;
            Rend.transform.localPosition = Rend.transform.localRotation * (Vector3)NewPosition;
        }


    }
}