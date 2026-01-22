//using UnityEngine;
//using Util;


//namespace BlockPrinter
//{
//    public class SettingMenu : MonoBehaviour
//    {
//        public enum SettingPhase
//        {
//            PlayerOrCPU,
//            SetCPULevel,
//            SetPlayerLeft,
//            SetPlayerRight,
//        }
//        public SettingPhase CurrentPhase;

//        [SerializeField] private UserInterface.MenuList BaseMenu;

//        [SerializeField] private UserInterface.MenuList PlayerSettingMenu;
//        [SerializeField] private BitmapText MessageText;

//        [SerializeField] private int PlayerCount;

//        private int CurrentPlayerIndex;
//        private FieldController[] PlayerSettings;

//        public void Initialize()
//        {
//            BaseMenu.Initialize(null);
//            SetupPlayer(0);
//        }

//        public void Update()
//        {
//            switch (CurrentPhase)
//            {
//                case SettingPhase.PlayerOrCPU: break;
//                case SettingPhase.SetCPULevel: break;
//                case SettingPhase.SetPlayerLeft:
//                    {
//                        for(int i = 0; i <= (int)KeyCode.Joystick8Button19;  i++)
//                        {
//                            if (Input.GetKeyDown((KeyCode)i))
//                            {
//                                break;
//                            }

//                        }
//                    }
//                    break;
//                case SettingPhase.SetPlayerRight:
//                    {

//                    }
//                    break;
//            }
//        }

//        public void SetupPlayer(int Index)
//        {
//            CurrentPlayerIndex = Index;
//            MessageText.UpdateText($"Choose a Player {CurrentPlayerIndex} Left Button");
//        }

//        public void OnSelectPlayer()
//        {
//            CurrentPhase = SettingPhase.SetPlayerLeft;
//        }

//        public void OnSelectCPU()
//        {

//        }


//}
