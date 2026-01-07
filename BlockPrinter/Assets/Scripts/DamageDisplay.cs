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

        public void Initialize(BlockAppearence BlockPrefab)
        {
            this.BlockPrefab = BlockPrefab;

            GenerateDamageBlock(this.damageBlockCount);

            for( int i = 0; i < this.DamageBlocks.Length; i++ )
            {
                this.DamageBlocks[i].SetAppearence(BlockColor.None);
            }
        }

        public void UpdateBlocks(BlockColor[] blockColors)
        {
            for (int i = 0; i < this.damageBlockCount; i++)
            {
                this.DamageBlocks[i].SetAppearence(blockColors[i]);
            }
        }

        public void UpdateRemainingTime(float second)
        {
            float percent = second / this.LimitTime;

            if( percent < 0 ) { percent = 0f; }

            LimitGauge.SetLength(percent);
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

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
