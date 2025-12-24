using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class LinearGradient : AnimationBase
    {
        public SpriteRenderer Rend;
        public Color StartColor;
        public Color EndColor;


        public void Awake()
        {
            Rend = GetComponent<SpriteRenderer>();
        }

        public override void OnUpdate(float t)
        {
            Rend.color = Color.Lerp(StartColor, EndColor, t);
        }

        public override void OnStartAnimation()
        {
            Rend.color = StartColor;
        }
    }
}
