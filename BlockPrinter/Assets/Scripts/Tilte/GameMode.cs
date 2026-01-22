using UnityEngine;
using UnityEngine.SceneManagement;

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

        [SerializeField] private UserInterface.MenuList MainMenu;

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
            MainMenu.Initialize(null);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void StartSingleMode()
        {
            FieldController.LastSettings = new FieldController[]
               {
                    new FieldController() { Mode = FieldController.UseMode.Player, PlayerKeyConfig = new KeyConfig() { LeftKey = KeyCode.A, RightKey = KeyCode.D } },
               };
            SceneManager.LoadScene("GameMain");
        }

        public void StartVersusCPUMode()
        {
            FieldController.LastSettings = new FieldController[]
                {
                    new FieldController() { Mode = FieldController.UseMode.Player, PlayerKeyConfig = new KeyConfig() { LeftKey = KeyCode.A, RightKey = KeyCode.D } },
                    new FieldController() { Mode = FieldController.UseMode.CPU, CPUConfig = new CPUProperty() { Prop = new CPUCharacteristics() { Level = 4 } } }
                };
            SceneManager.LoadScene("BattleModeScene");
        }

        public void StartVersusPlayerMode()
        {
            FieldController.LastSettings = new FieldController[]
            {
                    new FieldController() { Mode = FieldController.UseMode.Player, PlayerKeyConfig = new KeyConfig() { LeftKey = KeyCode.A, RightKey = KeyCode.D } },
                    new FieldController() { Mode = FieldController.UseMode.Player, PlayerKeyConfig = new KeyConfig() { LeftKey = KeyCode.LeftArrow, RightKey = KeyCode.RightArrow } },
            };
            SceneManager.LoadScene("BattleModeScene");
        }

        public void StartFourPlayerMode()
        {

            FieldController.LastSettings = new FieldController[]
            {
                    new FieldController() { Mode = FieldController.UseMode.Player, PlayerKeyConfig = new KeyConfig() { LeftKey = KeyCode.A, RightKey = KeyCode.D } },
                    new FieldController() { Mode = FieldController.UseMode.Player, PlayerKeyConfig = new KeyConfig() { LeftKey = KeyCode.J, RightKey = KeyCode.L } },
                    new FieldController() { Mode = FieldController.UseMode.Player, PlayerKeyConfig = new KeyConfig() { LeftKey = KeyCode.LeftArrow, RightKey = KeyCode.RightArrow } },
                    new FieldController() { Mode = FieldController.UseMode.Player, PlayerKeyConfig = new KeyConfig() { LeftKey = KeyCode.Keypad4, RightKey = KeyCode.Keypad6 } },
            };
            SceneManager.LoadScene("FourPlayerModeScene");
        }

        public void ExitApplication()
        {
            Application.Quit(0);
            Debug.Log("Application Exit");
            Debug.Break();
        }
    }
}
