using UnityEngine;

namespace BlockPrinter
{
    public class PlayerCharacter : MonoBehaviour
    {
        private float movingTime;
        private float moveDulation;
        private Vector3 moveToPosition;
        private Vector3 startPosition;

        //プレイヤーの初期化処理
        public void Initialize(Vector3 InitPosition)
        {
            //プレイヤーを初期位置にセットする
            OnMove(this.transform.position, InitPosition, 0f);
        }

        //プレイヤーの移動キャンセル(引数の位置に移動)
        public void OnMoveCancel(Vector3 NewPosition)
        {
            //自身の位置を引数の位置にセットする
            transform.position = NewPosition;
        }

        //ゲームオーバー時のプレイヤーの処理
        public void OnGameOver()
        {
            //自身を削除する
            Destroy(gameObject);
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

        //線形補間を利用して移動(引数StartPositionから引数EndPositionまでの直線距離の引数Persentの割合
        private void MoveToPosition(Vector3 StartPosition, Vector3 EndPosition, float Persent)
        {
            //線形補間(vector3)で移動先をセット
            Vector3 newPosition = Vector3.Lerp(StartPosition, EndPosition, Persent);

            //セットした移動先を自身の位置にする
            this.transform.position = newPosition;
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
