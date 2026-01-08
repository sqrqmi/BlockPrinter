using System.Linq.Expressions;
using UnityEngine;
using Util;

namespace BlockPrinter.Effect
{
    public class BreakEffects : MonoBehaviour
    {
        [SerializeField] BreakEffect Effect;

        private BreakEffect[] Effects;

        //エフェクトの発射する数
        [SerializeField] private int DirectionCount = 8;

        private Vector3 Position = Vector3.zero;

        private void SetPosition(Vector3 newPosition)
        {
            this.Position = newPosition;
        }

        private void SetDirectionCount(int newDirectionCount)
        {
            this.DirectionCount = newDirectionCount;

            this.Effects = new BreakEffect[DirectionCount];

            for (int i = 0; i < this.DirectionCount; i++)
            {
                this.Effects[i] = Instantiate(this.Effect);
                this.Effects[i].transform.SetParent(this.transform);
            }

        }

        private void SetAppearence(EffectColor NewColor)
        {
            for( int i = 0; i < Effects.Length; i++)
            {
                Effects[i].SetAppearence(NewColor);
            }
        }

        public void Run()
        {
            for (int i = 0; i < this.DirectionCount; i++)
            {
                float angle = 360f / (float)this.DirectionCount * i;
                Vector3 toPos = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
                toPos += this.Position;

                Vector3 rotation = new Vector3(0f, 0f, angle - 90f);

                this.Effects[i].transform.rotation = Quaternion.Euler(rotation);
                this.Effects[i].GetComponent<LinearMovement>().From = this.Position;
                this.Effects[i].GetComponent<LinearMovement>().To = toPos;
            }
        }

        public void Initialize(int newDirectionCount, EffectColor newColor, Vector3 newPosition)
        {
            SetDirectionCount(newDirectionCount);
            SetAppearence(newColor);
            SetPosition(newPosition);
        }

        // Update is called once per frame
        void Update()
        {
            float moveTime = this.Effects[0].GetComponent<LinearMovement>().MoveTime;
            float elapsedTime = this.Effects[0].GetComponent<LinearMovement>().ElapsedTime;
            if ( moveTime < elapsedTime )
            {
                Destroy(gameObject);
            }
        }
    }
}
