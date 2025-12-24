using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlockPrinter
{
    //ボタンが押されたときの処理
    public class ButtonDown : MonoBehaviour
    {
        //ゲームモード
        [SerializeField] private GameMode gameMode;

        //Singleボタンが押されたら
        public void OnButtonDownSingle()
        {
            //ゲームモードを1人プレイにする
            gameMode.SetSingle();
        }

        //Doubleボタンが押されたら
        public void OnButtonDownDouble()
        {
            //ゲームモードを2人プレイにする
            gameMode.SetDouble();
        }

        //Startボタンが押されたら
        public void OnButtonDownStart()
        {
            if (GameMode.playerMode == PlayerMode.Single)
            {
                //1人ゲームを開始する
                SceneManager.LoadScene("GameMain");
            }else if( GameMode.playerMode == PlayerMode.Double)
            {
                //2人対戦ゲームを開始する
                SceneManager.LoadScene("BattleModeScene");
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
