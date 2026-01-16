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
        [SerializeField] private BlockAppearence[] DamageBlocks;    //ダメージ用
        [SerializeField] private BlockAppearence[] DirectionBlocks;       //演出用
        private BlockColor[] BlockColors;                           //追加攻撃検知用
        private float[] AttackEffectTimes = { 0.5f, 0.75f };
        private int AddedAttack = 0;
        private bool GameOvered = false;

        public void Initialize(BlockAppearence BlockPrefab)
        {
            this.BlockPrefab = BlockPrefab;

            GenerateDamageBlock(this.damageBlockCount, ref this.DamageBlocks);

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
            DestroyAllDamageBlock();
            DestroyAllDirectionBlock();
            this.BlockPrefab = null;
        }

        //ダメージブロックの更新
        public void UpdateBlocks(BlockColor[] blockColors)
        {
            this.BlockColors = new BlockColor[blockColors.Length];
            for( int i = 0; i < this.BlockColors.Length; i++ )
            {
                this.BlockColors[i] = blockColors[i];
            }

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
        public void UpComingDamagedBlocks(BlockColor[] blockColors, Vector3 startPosition, int attackSum)
        {
            int damagedSum = 0;
            for( int i = 0; i < blockColors.Length; i++ )
            {
                damagedSum++;
                if (blockColors[i+1] == BlockColor.None)
                {
                    break;
                }
            }
            Debug.Log($"Damaged Total Blocks {damagedSum} / Add Attacks {attackSum}");

            this.AddedAttack = 0;
            if( damagedSum != attackSum ){ AddedAttack = damagedSum - attackSum; }

            DestroyAllDirectionBlock();
            GenerateDamageBlock(damagedSum, ref this.DirectionBlocks);

            for( int i = 0; i < damagedSum; i++)
            {
                this.DirectionBlocks[i].SetAppearence(BlockColor.None);
            }

            float r = 5f;

            for ( int i = AddedAttack; i < damagedSum; i++ )
            {
                //ブロックの発射方向をランダムに指定
                Vector3 start = startPosition;
                float rad = Mathf.Deg2Rad * (Random.Range(0f, 360f));
                Vector3 end = new Vector3(Mathf.Cos(rad) * r, Mathf.Sin(rad) * r, 0f);
                end += start;

                this.DirectionBlocks[i].Initialize(start, BlockColor.None);
                //受けた攻撃のブロックの色を指定
                this.DirectionBlocks[i].SetAppearence(blockColors[i]);

                this.DirectionBlocks[i].MoveBrakeAttack(start, end, this.AttackEffectTimes[0]);

                this.DirectionBlocks[i].transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            }
            UpdateBlocks(blockColors);

            Invoke("StartAttackMovement", this.AttackEffectTimes[0]);
        }

        //攻撃を受けた時のブロックの移動処理(後半)
        private void StartAttackMovement()
        {
            for( int i = AddedAttack; i < this.DirectionBlocks.Length; i++ )
            {
                Vector3 start = this.DirectionBlocks[i].transform.position;
                Vector3 end = Vector3.zero;

                //if (i < 6){ end = new Vector3(-1.5f, 0.25f * i, 0f); }
                //else{ end = new Vector3(-1.5f, 0.25f * 6, 0f); }

                this.DirectionBlocks[i].MoveSircleAttack(start, end, this.AttackEffectTimes[1]);
            }
            if( GameOvered ) { Invoke("OnGameOver", this.AttackEffectTimes[1]); }
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

        private void GenerateDamageBlock(int generateCount, ref BlockAppearence[] blocks)
        {
            blocks = new BlockAppearence[generateCount];

            for( int i = 0; i < generateCount; i++)
            {
                blocks[i] = Instantiate(this.BlockPrefab);
                blocks[i].transform.SetParent(this.RemainingBlocks.transform);   
                blocks[i].transform.localPosition = Vector3.zero;
                blocks[i].OnMove(new Vector3(0, 1f, 0f), new Vector3(-1.5f, 0.25f * i, 0f), 1f);
                blocks[i].transform.localScale = new Vector3(this.BlockScale, this.BlockScale, this.BlockScale);
            }
        }

        private void DestroyDamageBlock(int destroyCount, ref BlockAppearence[] blocks)
        {
            for( int i = 0; i < destroyCount; i++)
            {
                blocks[i].SetAppearence(BlockColor.None);
            }       
        }


        private void DestroyAllDamageBlock()
        {
            foreach (var block in this.DamageBlocks)
            {
                if (block != null)
                {
                    Destroy(block.gameObject);
                }
            }
        }

        private void DestroyAllDirectionBlock()
        {
            foreach (var block in this.DirectionBlocks)
            {
                if (block != null)
                {
                    Destroy(block.gameObject);
                }
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
