using UnityEngine;
using UnityEngine.UI;
using Util;

namespace BlockPrinter.UserInterface
{
    //スコアを表示する
    public class ScoreDisplay : MonoBehaviour
    {
        //表示するテキスト
        [SerializeField] private BitmapText showText;

        //表示するスコア
        private int showScore = 0;

        //初期化処理
        public void initialize(int InitialScore)
        {
            //表示スコアを引数の値にする
            this.showScore = InitialScore;
        }

        //表示スコアを更新する（引数で送られてくるのは追加された後の値）
        public void UpDateScore(int NewScore)
        {
            //表示スコアを引数の値にする
            this.showScore = NewScore;
        }

        //スコアを表示する（引数の値）
        private void PrintDisplay(int printScore)
        {
            //テキストの中身をスコアにする
            this.showText.UpdateText("Score:" + printScore.ToString());
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            PrintDisplay(this.showScore);
        }
    }
}
