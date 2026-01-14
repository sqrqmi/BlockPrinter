using UnityEngine;
using Util;

namespace BlockPrinter.UserInterface
{
    public class AttackChargeDisplay : MonoBehaviour
    {
        //表示するテキスト
        [SerializeField] private BitmapText showText;
        private BlockAppearence BlockPrefab;
        private int AttackCharge;

        public void Initialize(BlockAppearence BlockPrefab)
        {
            this.BlockPrefab = BlockPrefab;

            UpdateAttackCharge(0);
        }

        //リセット処理
        public void DiscardInstances()
        {
            Initialize(this.BlockPrefab);
        }

        public void UpdateAttackCharge(int Attack)
        {
            this.AttackCharge = Attack;

            UpdateText(this.AttackCharge);
        }

        private void UpdateText(int Attack)
        {
            this.showText.UpdateText("Attack:" + Attack.ToString());
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if(GameMode.playerMode == PlayerMode.Single) { this.gameObject.SetActive(false); }
        }
    }
}
