using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class LinearStretch : AnimationBase
    {
        public SpriteRenderer Rend;

        public Vector2 StartSize;
        public Vector2 EndSize;

        public void Awake()
        {
            Rend = GetComponent<SpriteRenderer>();
        }

        public override void OnUpdate(float t)
        {
            Rend.size = Vector2.Lerp(StartSize, EndSize, t);
        }

        public override void OnStartAnimation()
        {
            Rend.size = StartSize;
        }
    }
}
