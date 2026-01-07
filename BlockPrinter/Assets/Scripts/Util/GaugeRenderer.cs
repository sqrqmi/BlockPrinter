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
                    NewPosition = new Vector3((ContentSize - NewSize).x * +0.5f, 0.0f);
                    break;
                case FillDirection.Down:
                    NewPosition = new Vector3(0.0f, (ContentSize - NewSize).y * +0.5f);
                    break;
                case FillDirection.Right:
                    NewPosition = new Vector3((ContentSize - NewSize).x * -0.5f, 0.0f);
                    break;
                case FillDirection.Up:
                    NewPosition = new Vector3(0.0f, (ContentSize - NewSize).y * -0.5f);
                    break;

            }
            Rend.size = NewSize;
            Rend.transform.localPosition = Rend.transform.localRotation * (Vector3)NewPosition;
        }


    }
}