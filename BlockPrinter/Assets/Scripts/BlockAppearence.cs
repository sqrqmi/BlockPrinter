using BlockPrinter.Effect;
using UnityEditor.Analytics;
using UnityEngine;

namespace BlockPrinter
{
    public enum BlockColor
    {
        None,
        Red,
        Blue,
        Green,
        Yellow
    }

    public class BlockAppearence : MonoBehaviour
    {
        //Blockの画像データ(BlockSkin)を入れる
        [SerializeField] private BlockSkin skin;

        //BlockのSpriteRendererを入れる(SerializeField : インスペクター上で表示される設定)
        [SerializeField] private SpriteRenderer sprite;

        //Blockの破壊演出
        [SerializeField] private BreakEffects BreakEffects;

        private float movingTime;
        private float moveDulation;
        private Vector3 moveToPosition;
        private Vector3 startPosition;

        public void Initialize(Vector3 NewPosition, BlockColor NewColor)
        {
            this.transform.localPosition = NewPosition;

            this.startPosition = NewPosition;
            this.moveToPosition = NewPosition;

            SetAppearence(NewColor);
        }

        //見た目の指定
        public void SetAppearence(BlockColor NewColor)
        {
            //新しい画像指定
            Sprite newSprite = null;

            switch (NewColor)
            {
                case BlockColor.None: newSprite = this.skin.None; break;
                case BlockColor.Red: newSprite = this.skin.Red; break;
                case BlockColor.Blue: newSprite = this.skin.Blue; break;
                case BlockColor.Green: newSprite = this.skin.Green; break;
                case BlockColor.Yellow: newSprite = this.skin.Yellow; break;
            }

            //自身の画像を変更する
            this.sprite.sprite = newSprite;
        }

        //ブロックの破壊演出(ポリオミノの場合)
        public void OnBreakBlock()
        {
            //演出ながす
            var effects = Instantiate(this.BreakEffects);

            effects.Initialize(8, EffectColor.Yellow, this.transform.position);
            effects.Run();
        }

        //ブロックの破壊演出(5つ以上の塊になった場合の)
        public void OnRecoverLargeChunk()
        {
            //演出ながす
            var effects = Instantiate(this.BreakEffects);

            effects.Initialize(16, EffectColor.Red, this.transform.position);
            effects.Run();
        }

        //ブロックの移動(引数StartPositionから引数NewPositionの位置に引数Dulationの時間後に移動)
        public void OnMove(Vector3 StartPosition, Vector3 NewPosition, float Dulation)
        {
            //移動開始位置を現在の位置にセットする
            this.startPosition = StartPosition;

            //移動時間に引数の移動時間をセットする
            this.moveDulation = Dulation;

            //移動位置に引数の位置にセットする
            this.moveToPosition = NewPosition;

            //移動中の時間カウンタをリセットする
            this.movingTime = 0f;
        }

        //点灯
        public void Highlight()
        {
            Color newColor = new Color(this.sprite.color.r, this.sprite.color.g, this.sprite.color.b, 1f);

            this.sprite.color = newColor;
        }

        //消灯
        public void Fade()
        {
            Color newColor = new Color(this.sprite.color.r, this.sprite.color.g, this.sprite.color.b, 0.3f);

            this.sprite.color = newColor;
        }

        //線形補間を利用して移動(引数StartPositionから引数EndPositionまでの直線距離の引数Persentの割合
        private void MoveToPosition(Vector3 StartPosition, Vector3 EndPosition, float Percent)
        {
            //線形補間(vector3)で移動先をセット
            Vector3 newPosition = Vector3.Lerp(StartPosition, EndPosition, Percent);

            //セットした移動先を自身の位置にする
            this.transform.localPosition = newPosition;
        }

        void Start()
        {
            this.moveDulation = -1f;
            this.movingTime = 0f;
        }

        // Update is called once per frame
        void Update()
        {
            //移動時間を増加
            this.movingTime += Time.deltaTime;

            //移動時間のパーセントを計算（0除算と1を超えないように計算する）
            float persent;
            if( this.moveDulation <= 0f ) { persent = 1f; }
            else
            {
                persent = this.movingTime / this.moveDulation;

                if( persent > 1f ) { persent = 1f; }
            }

            //移動する
            MoveToPosition(this.startPosition, this.moveToPosition, persent);
        }
    }
}
