using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{

    public class LinearMovement : AnimationBase
    {


        public Vector3 From;
        public Vector3 To;

        private Vector3 Pivot;
        private Vector3 Delta;

        public override void OnUpdate(float t)
        {
            this.transform.localPosition = Delta * t + Pivot;
        }

        public override void OnStartAnimation()
        {
            Pivot = From;
            Delta = To - From;
            this.transform.localPosition = From;
        }


        public static LinearMovement Create(GameObject Obj, Vector3 From, Vector3 To, float Time, InterpolationMode Mode)
        {
            LinearMovement NewMovement = Obj.GetComponent<LinearMovement>();
            if(NewMovement == null)
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
