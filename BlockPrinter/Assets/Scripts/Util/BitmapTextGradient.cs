using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class BitmapTextGradient : AnimationBase
    {
        public BitmapText Text;
        public Color StartColor;
        public Color EndColor;

        public override void OnStartAnimation()
        {
            Text.SetColor(StartColor);
        }

        public override void OnUpdate(float t)
        {
            Text.SetColor(Color.Lerp(StartColor, EndColor, t));
        }
    }
}
