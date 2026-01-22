//using BlockPrinter.UserInterface;
//using UnityEngine;
//using Util;

//namespace BlockPrinter
//{
//    public class ValueSelector : MenuBase
//    {
//        [SerializeField] private BitmapText ValueText;
//        [SerializeField] private int InitialValue;
//        [SerializeField] private int MinValue;
//        [SerializeField] private int MaxValue;

//        private int CurrentValue;

//        private void Update()
//        {
//            if(!IsActive)
//            {
//                return;
//            }
//            if (Util.CommonInput.GetKeyDown(Direction.Up))
//            {
//                SetValue(CurrentValue + +1);
//            }
//            if (Util.CommonInput.GetKeyDown(Direction.Down))
//            {
//                SetValue(CurrentValue + -1);
//            }
//            if(Input.GetKeyDown(KeyCode.Return))
//            {
//                Submit
//            }
//        }

//        public override void Initialize(MenuBase SuperMenu)
//        {
//            if (SuperMenu == null)
//            {
//                RootInstance = this;
//            }
//            else
//            {
//                SuperMenu.SwitchActive(false);
//            }
//            SuperInstance = SuperMenu;
//            CurrentValue = InitialValue;
//            IsActive = true;
//        }

//        public void SetValue(int NewValue)
//        {
//            CurrentValue = Mathf.Clamp(NewValue, MinValue, MaxValue);
//        }

//        public int GetValue()
//        {
//            return CurrentValue;
//        }
//    }
//}
