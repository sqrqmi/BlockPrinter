using System;
using UnityEngine;
using Util;

namespace BlockPrinter.UserInterface
{

    public class PerformDisplay : MonoBehaviour
    {
        [SerializeField] private BitmapText PureChainText;
        [SerializeField] private BitmapText ActiveChainText;
        [SerializeField] private BitmapText SynchronousBreakText;

        public void Initialize()
        {

        }

        public void OnPerformPureChain(int Chain)
        {
            PureChainText.UpdateText("Pure " + Chain.ToString());
            Util.AnimationBase.StartAllAnimation(PureChainText.gameObject);
        }

        public void OnPerformActiveChain(int Chain)
        {
            ActiveChainText.UpdateText("Active " + Chain.ToString());
            Util.AnimationBase.StartAllAnimation(ActiveChainText.gameObject);

        }

        public void OnPerformSynchronousBreak(int Count)
        {
            SynchronousBreakText.UpdateText("Sync " + Count.ToString());
            Util.AnimationBase.StartAllAnimation(SynchronousBreakText.gameObject);

        }

        public void DiscardInstance()
        {

        }
    }
}
