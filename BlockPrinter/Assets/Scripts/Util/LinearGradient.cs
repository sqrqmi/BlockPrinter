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

        public static LinearGradient Create(GameObject Obj, Color From, Color To, float Time, InterpolationMode Mode)
        {
            LinearGradient NewGradient = Obj.GetComponent<LinearGradient>();
            if (NewGradient == null)
            {
                NewGradient = Obj.AddComponent<LinearGradient>();
            }
            NewGradient.StartColor = From;
            NewGradient.EndColor = To;
            NewGradient.MoveTime = Time;
            NewGradient.Mode = Mode;
            NewGradient.StartAnimation();
            return NewGradient;
        }
    }
}
