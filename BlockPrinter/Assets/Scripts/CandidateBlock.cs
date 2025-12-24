using UnityEngine;

namespace BlockPrinter
{
    public class CandidateBlock : MonoBehaviour
    {
        //Blockの画像データ(BlockSkin)を入れる
        [SerializeField] private BlockSkin skin;

        //BlockのSpriteRendererを入れる(SerializeField : インスペクター上で表示される設定)
        [SerializeField] private SpriteRenderer sprite;

        //自身の初期位置
        Vector3 InitPosition = new Vector3(-1f, 4.5f, 0f);

        private float movingTime;
        private float moveDulation;
        private Vector3 moveToPosition;
        private Vector3 startPosition;

        //初期化処理
        public void Initialize()
        {
            //自身の位置を設定する
            this.transform.localPosition = InitPosition;
            this.movingTime = 0f;
            //移動開始位置を現在の位置にセットする
            this.startPosition = InitPosition;
        }

        //次のブロックの色を設定
        public void SetNextColor(BlockColor NewColors)
        {
            //新しい画像指定
            Sprite newSprite = null;

            switch (NewColors)
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

        //ブロックの移動(引数StartPositionから引数NewPositionの位置に引数Dulationの時間後に移動)
        public void OnMove(Vector3 NewPosition, float Dulation)
        {
            //移動時間に引数の移動時間をセットする
            this.moveDulation = Dulation;

            //移動位置に引数の位置にセットする
            this.moveToPosition = NewPosition;

            //移動中の時間カウンタをリセットする
            this.movingTime = 0f;
        }

        //線形補間を利用して移動(引数StartPositionから引数EndPositionまでの直線距離の引数Persentの割合
        private void MoveToPosition(Vector3 StartPosition, Vector3 EndPosition, float Persent)
        {
            //線形補間(vector3)で移動先をセット
            Vector3 newPosition = Vector3.Lerp(StartPosition, EndPosition, Persent);

            //セットした移動先を自身の位置にする
            this.transform.localPosition = newPosition;
        }

        void Start()
        {
            Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            //移動時間を増加
            this.movingTime += Time.deltaTime;

            //移動時間のパーセントを計算（0除算と1を超えないように計算する）
            float persent;
            if (this.moveDulation <= 0f) { persent = 1f; }
            else
            {
                persent = this.movingTime / this.moveDulation;

                if (persent > 1f) { persent = 1f; }
            }

            //移動する
            MoveToPosition(this.startPosition, this.moveToPosition, persent);
        }
    }
}
