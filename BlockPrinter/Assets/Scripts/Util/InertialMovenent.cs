using System;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public class InertialMovenent : AnimationBase
    {
        [SerializeField] private Vector3 From;
        [SerializeField] private Vector3 To;
        [SerializeField] private Vector3 InitialVelocity;
        [SerializeField] private Vector3 Acceleration;

        public override void OnUpdate(float t)
        {
            this.transform.localPosition = InitialVelocity * t + Acceleration * (t * t * 0.5f) + From;
        }

        public override void OnStartAnimation()
        {
            this.transform.localPosition = From;
        }

        public static InertialMovenent CreateFromTime(GameObject Obj, Vector3 From, Vector3 To, Vector3 InitialVelocity, float Time)
        {
            InertialMovenent NewMovement = Obj.GetComponent<InertialMovenent>();
            if (NewMovement == null)
            {
                NewMovement = Obj.AddComponent<InertialMovenent>();
            }
            NewMovement.From = From;
            NewMovement.To = To;
            NewMovement.MoveTime = Time;
            NewMovement.InitialVelocity = InitialVelocity;
            float VelocityScalar = Time * Time * 0.5f;
            NewMovement.Acceleration = ((To - (From + InitialVelocity * Time)) / VelocityScalar);
            NewMovement.Mode = InterpolationMode.Linear;
            NewMovement.StartAnimation();
            return NewMovement;
        }

        public void StopAnimation()
        {
            this.enabled = false;
        }
    }
}
