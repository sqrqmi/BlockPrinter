using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public abstract class AnimationBase : MonoBehaviour
    {
        public InterpolationMode Mode;

        public float MoveTime;
        public float ElapsedTime;
        public bool IsStartOnAwake;
        public bool IsActiveAnimation;

        public void Start()
        {
            if(IsStartOnAwake)
            {
                StartAnimation();
            }
        }


        public void Update()
        {
            if(IsActiveAnimation)
            {
                ElapsedTime += Time.deltaTime;
                if(ElapsedTime >= MoveTime)
                {
                    OnUpdate(1.0f);
                    IsActiveAnimation = false;
                }
                else
                {
                    OnUpdate(Util.ApplyInterpolation(Mode, ElapsedTime / MoveTime));
                }
            }
        }

        public void StartAnimation()
        {
            IsActiveAnimation = true;
            ElapsedTime = 0.0f;
            OnStartAnimation();
        }

        public abstract void OnUpdate(float t);
        public abstract void OnStartAnimation();

        public static void StartAllAnimation(GameObject TargetObj)
        {
            foreach(AnimationBase ani in TargetObj.GetComponentsInChildren<AnimationBase>())
            {
                ani.StartAnimation();
            }
        }

    }
}
