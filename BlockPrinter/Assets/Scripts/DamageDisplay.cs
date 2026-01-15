using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using Util;

namespace BlockPrinter.UserInterface
{
    public class DamageDisplay : MonoBehaviour
    {
        [SerializeField] private int damageBlockCount = 6;
        [SerializeField] private float BlockScale = 1f;
        [SerializeField] private float LimitTime = 10f;
        [SerializeField] private GaugeRenderer LimitGauge;
        [SerializeField] private GameObject RemainingBlocks;
        private BlockAppearence BlockPrefab;
        private BlockAppearence[] DamageBlocks;
        private float[] AttackEffectTimes = { 0.125f, 0.25f };

        public void Initialize(BlockAppearence BlockPrefab)
        {
            this.BlockPrefab = BlockPrefab;

            GenerateDamageBlock(this.damageBlockCount);

            for( int i = 0; i < this.DamageBlocks.Length; i++ )
            {
                this.DamageBlocks[i].SetAppearence(BlockColor.None);
            }

            UpdateRemainingTime(0f);
            SetRemainingTimeVisible(false);
        }

        //リセット処理
        public void DiscardInstances()
        {
            this.BlockPrefab = null;
            DestroyAll();
        }

        //ダメージブロックの更新
        public void UpdateBlocks(BlockColor[] blockColors)
        {
            for (int i = 0; i < this.damageBlockCount; i++)
            {
                //受けた攻撃のブロックを次の色に指定
                this.DamageBlocks[i].SetAppearence(blockColors[i]);

                //ブロックの移動(見た目)
                Vector3 startPos = new Vector3(-1.5f, 0.25f * (i + 1), 0f);
                Vector3 endPos = this.DamageBlocks[i].transform.localPosition;
                this.DamageBlocks[i].MoveNextBlock(startPos, endPos);
            }
        }

        //ダメージを受けたときの処理
        public void UpComingDamagedBlocks(BlockColor[] blockColors, int[] colors)
        {
            int damagedSum = 0;
            for( int i = 0; i < colors.Length; i++ )
            {
                damagedSum += colors[i];
            }

            this.DamageBlocks = new BlockAppearence[damagedSum];

            float r = 3f;

            for ( int i = 0; i < damagedSum; i++ )
            {
                //ブロックの発射方向をランダムに指定
                Vector3 start = Vector3.zero;
                float rad = Mathf.Deg2Rad * (Random.Range(0f, 360f));
                Vector3 end = new Vector3(Mathf.Cos(rad) * r, Mathf.Sin(rad) * r, 0f);                

                this.DamageBlocks[i].MoveBrakeAttack(start, end, this.AttackEffectTimes[0]);

                //受けた攻撃のブロックの色を指定
                this.DamageBlocks[i].SetAppearence(blockColors[i]);
            }

            Invoke("StartAttackMovement", this.AttackEffectTimes[0]);
        }

        //攻撃を受けた時のブロックの移動処理(後半)
        private void StartAttackMovement()
        {
            for( int i = 0; i < this.DamageBlocks.Length; i++ )
            {
                Vector3 start = this.DamageBlocks[i].transform.position;
                Vector3 end;

                if (i < 6){ end = new Vector3(-1.5f, 0.25f * i, 0f); }
                else{ end = new Vector3(-1.5f, 0.25f * 6, 0f); }

                this.DamageBlocks[i].MoveSircleAttack(start, end, this.AttackEffectTimes[1]);
            }
        }

        public void UpdateRemainingTime(float second)
        {
            float percent = second / this.LimitTime;

            if( percent < 0 ) { percent = 0f; }

            LimitGauge.SetLength(percent);
        }

        public void SetRemainingTimeVisible(bool visible)
        {
            if (visible) { return; }

            LimitGauge.SetLength(0f);
        }

        private void GenerateDamageBlock(int generateCount)
        {
            this.DamageBlocks = new BlockAppearence[generateCount];

            for( int i = 0; i < generateCount; i++)
            {
                this.DamageBlocks[i] = Instantiate(this.BlockPrefab);
                this.DamageBlocks[i].transform.SetParent(this.RemainingBlocks.transform);   
                this.DamageBlocks[i].transform.localPosition = Vector3.zero;
                this.DamageBlocks[i].OnMove(new Vector3(0, 1f, 0f), new Vector3(-1.5f, 0.25f * i, 0f), 1f);
                this.DamageBlocks[i].transform.localScale = new Vector3(this.BlockScale, this.BlockScale, this.BlockScale);
            }
        }

        private void DestroyAll()
        {
            foreach (var block in this.DamageBlocks)
            {
                Destroy(block.gameObject);
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if( GameMode.playerMode == PlayerMode.Single)
            {
                Destroy(gameObject);
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
