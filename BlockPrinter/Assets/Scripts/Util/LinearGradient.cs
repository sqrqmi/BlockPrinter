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

        public static LinearMovement Create(GameObject Obj, Vector3 From, Vector3 To, float Time, InterpolationMode Mode)
        {
            LinearMovement NewMovement = Obj.GetComponent<LinearMovement>();
            if (NewMovement == null)
            {
                NewMovement = Obj.AddComponent<LinearMovement>();
            }
            NewMovement.From = From;
            NewMovement.To = To;
            NewMovement.MoveTime = Time;
            NewMovement.Mode = Mode;
            NewMovement.StartAnimation();
            return NewMovement;
        }
    }
}
