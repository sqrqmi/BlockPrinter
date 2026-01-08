using System.Linq.Expressions;
using UnityEngine;
using Util;

namespace BlockPrinter.Effect
{
    public class RainbowEffects : MonoBehaviour
    {
        [SerializeField] BreakEffect Effect;

        private BreakEffect[] Effects;

        [SerializeField] int MaxEffectsCount = 80;

        //エフェクトの発射する数
        [SerializeField] private int DirectionCount = 8;

        private Vector3 Position = Vector3.zero;

        [SerializeField] private float LifeTime = 3.0f;
        [SerializeField] private float IntervalTime = 0.05f;
        private float time = 0.0f;

        private int EffectIndex = 0;
        private EffectColor EffectColor = EffectColor.Red;

        private void SetPosition(Vector3 newPosition)
        {
            this.Position = newPosition;
        }

        private void SetDirectionCount(int newDirectionCount)
        {
            this.DirectionCount = newDirectionCount;

            this.MaxEffectsCount = this.DirectionCount * 10;

            this.Effects = new BreakEffect[MaxEffectsCount];
        }

        //引数で送られてきたエフェクトを動かす
        public void Run(int effectIndex)
        {
            this.Effects[effectIndex] = Instantiate(this.Effect);
            this.Effects[effectIndex].transform.SetParent(this.transform);
            this.Effects[effectIndex].SetAppearence(this.EffectColor);

            float angle = 360f / (float)this.DirectionCount * (effectIndex % this.DirectionCount);
            Vector3 toPos = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);
            toPos += this.Position;

            Vector3 rotation = new Vector3(0f, 0f, angle - 90f);

            this.Effects[effectIndex].transform.rotation = Quaternion.Euler(rotation);
            this.Effects[effectIndex].GetComponent<LinearMovement>().From = this.Position;
            this.Effects[effectIndex].GetComponent<LinearMovement>().To = toPos;
        }

        public void Initialize(int newDirectionCount, Vector3 newPosition)
        {
            SetDirectionCount(newDirectionCount);
            SetPosition(newPosition);
            this.time = this.IntervalTime;
        }

        //エフェクト(単体)の生存時間のチェック
        private void CheckMoveTime()
        {
            for( int i = 0; i < this.MaxEffectsCount; i++)
            {
                if(this.Effects[i] != null)
                {
                    float moveTime = this.Effects[i].GetComponent<LinearMovement>().MoveTime;
                    float elapsedTime = this.Effects[i].GetComponent<LinearMovement>().ElapsedTime;
                    if (moveTime < elapsedTime)
                    {
                        Destroy(this.Effects[i].gameObject);
                    }
                }
            }
        }

        private void Start()
        {
            Initialize(16, new Vector3(0f, 0f, 0f));
        }

        // Update is called once per frame
        void Update()
        {
            CheckMoveTime();
            this.time += Time.deltaTime;
            this.LifeTime -= Time.deltaTime;

            if( this.LifeTime < 0 )
            {
                Destroy(gameObject);
            }

            //生成間隔の時間を超えていなかったら生成しない
            if (this.time < this.IntervalTime)
            {
                return;
            }

            if (this.Effects[EffectIndex] == null)
            {
                Run(EffectIndex);

                this.time = 0f;

                EffectIndex++;
                if( EffectIndex >= this.MaxEffectsCount) { EffectIndex = 0; }

                EffectColor++;
                if( EffectColor >= EffectColor.Gray) { EffectColor = EffectColor.Red; }
            }
        }
    }
}
