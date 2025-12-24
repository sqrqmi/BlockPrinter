using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlockPrinter
{
    //一人プレイの管理
    public class SinglePlayerMaster : MonoBehaviour
    {
        //フィールドシステム
        [SerializeField] private FieldSystem fieldSystem;

        //一人プレイを開始する（FieldSystemを一つ初期化）
        public void Rungame()
        {
            //フィールドシステムの初期化処理を呼び出し
            fieldSystem.Initialize(0, null, OnGameOver);
        }


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            //一人モードが選択されていたら一人モードのゲームを開始する
            if( GameMode.playerMode == PlayerMode.Single) { Rungame(); }
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void OnGameOver(int PlayerID)
        {
            //ゲームオーバーになったら
            if (fieldSystem.IsGameOver())
            {
                //タイトルシーンに移動
                SceneManager.LoadScene("TitleScene");
            }
        }
    }
}
