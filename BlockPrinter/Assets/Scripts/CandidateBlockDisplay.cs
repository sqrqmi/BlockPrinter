using JetBrains.Annotations;
using UnityEngine;

namespace BlockPrinter.UserInterface
{
    public class CandidateBlockDisplay : MonoBehaviour
    {
        //次の色を表示するブロックのプレハブ
        [SerializeField] private CandidateBlock candidateBlockPrefab;
        //以降に出てくるブロックの色の配列(10個まで)
        private BlockColor[] nextColors;
        //表示する以降の色のブロックの個数(~10)
        private int BlockCount;
        //表示するブロックの配列
        private CandidateBlock[] candidateBlocks;

        //自身の初期位置
        Vector3 InitPosition = Vector3.zero;

        //初期化処理
        public void Initialize(int CandidateBlockCount)
        {
            this.BlockCount = CandidateBlockCount;

            this.candidateBlocks = new CandidateBlock[this.BlockCount];

            GenerateBlocks(this.BlockCount);

            //自身の位置を設定する
            this.transform.localPosition = InitPosition;
        }

        //リセット処理
        public void DiscardInstances()
        {
            DestroyAll();
        }

        //次のブロックの色を設定
        public void SetNextBlock(BlockColor[] NewColors)
        {
            this.nextColors = NewColors;

            for( int i = 0; i < this.BlockCount; ++i)
            {
                this.candidateBlocks[i].SetNextColor(this.nextColors[i]);
            }
        }

        private void GenerateBlocks(int CandidateBlockCount)
        {
            for( int i = 0; i < CandidateBlockCount; ++i)
            {
                this.candidateBlocks[i] = Instantiate(candidateBlockPrefab);
                this.candidateBlocks[i].transform.SetParent(this.transform, false);
                this.candidateBlocks[i].transform.localPosition = Vector3.zero;
                this.candidateBlocks[i].OnMove(new Vector3(-1f, 4.5f - 0.5f * i, 0f), 1f);
            }
        }

        private void DestroyAll()
        {
            foreach (var block in this.candidateBlocks)
            {
                Destroy(block.gameObject);
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
