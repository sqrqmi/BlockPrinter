using JetBrains.Annotations;
using UnityEngine;

namespace BlockPrinter.UserInterface
{
    //消したポリオミノ(テトリミノ)の形を表示する（１形状１スクリプト）
    public class PolyominoDisplay : MonoBehaviour
    {
        [SerializeField] private float BlockScale = 1f;
        private BlockAppearence[] Polyomino;
        private Vector3 OriginBlockPosition;

        //ポリオミノの形を生成（引数BlocksPosition ブロックの形を作る相対位置, 引数BlockPrefab ブロック, 引数NewPosition ポリオミノの初期位置(相対位置0,0基準), 引数NewColor 初期色）
        public void Initialize(Vector2Int[] BlocksPosition, BlockAppearence BlockPrefab, Vector3 NewPosition, BlockColor NewColor)
        {
            this.Polyomino = new BlockAppearence[BlocksPosition.Length];

            this.transform.localPosition = NewPosition;

            float scale = 1f / this.BlockScale;

            for( int i = 0; i < BlocksPosition.Length; ++i)
            {
                //ブロックを生成
                this.Polyomino[i] = Instantiate(BlockPrefab);

                Polyomino[i].transform.SetParent(this.transform);
                
                //原点かそれ以外で処理を分ける
                if( i == 0 )
                {
                    //相対位置の原点の座標をセット
                    this.OriginBlockPosition = NewPosition;

                    //ブロックの位置と色をセット
                    this.Polyomino[i].Initialize(this.OriginBlockPosition, NewColor);
                }
                else
                {
                    //原点から相対位置分ずらす
                    float posX, posY;
                    posX = this.OriginBlockPosition.x + BlocksPosition[i].x / scale;
                    posY = this.OriginBlockPosition.y + BlocksPosition[i].y / scale;

                    //ブロックの位置と色をセット
                    this.Polyomino[i].Initialize(new Vector3(posX, posY, 0f), NewColor);
                }

                //ブロックごとのスケールを設定
                this.Polyomino[i].transform.localScale = new Vector3(this.BlockScale, this.BlockScale, this.BlockScale);
            }

            //自身のスケールを設定
            this.transform.localScale = new Vector3(this.BlockScale, this.BlockScale, this.BlockScale);

            Fade();
        }

        //点灯
        public void Highlight()
        {
            for( int i = 0; i < this.Polyomino.Length; ++i )
            {
                this.Polyomino[i].Highlight();
            }
        }

        //消灯
        public void Fade()
        {
            for (int i = 0; i < this.Polyomino.Length; ++i)
            {
                this.Polyomino[i].Fade(0.3f);
            }
        }

        //ポリオミノの色を更新
        public void SetBlockColor(BlockColor NewColor)
        {
            for( int i = 0; i < this.Polyomino.Length; ++i )
            {
                Polyomino[i].SetAppearence(NewColor);
            }
        }

        public void OnMove(Vector3 NewPosition)
        {

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
