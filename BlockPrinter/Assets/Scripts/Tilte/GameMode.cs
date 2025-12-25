using UnityEngine;

namespace BlockPrinter
{
    //プレイヤーモード（1人プレイか、2人プレイか）
    public enum PlayerMode
    {
        Single,
        Double,
    }

    public class GameMode : MonoBehaviour
    {
        //staticでplayerModeを宣言
        public static PlayerMode playerMode = PlayerMode.Double;

        //一人プレイに設定する
        public void SetSingle()
        {
            //プレイヤーモードを一人にする
            playerMode = PlayerMode.Single;
        }

        //二人プレイに設定する
        public void SetDouble()
        {
            //プレイヤーモードを二人にする
            playerMode = PlayerMode.Double;
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
