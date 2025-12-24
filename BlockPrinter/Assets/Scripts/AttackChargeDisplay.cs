using UnityEngine;

namespace BlockPrinter.UserInterface
{
    public class AttackChargeDisplay : MonoBehaviour
    {
        private BlockAppearence BlockPrefab;

        public void Initialize(BlockAppearence BlockPrefab)
        {
            this.BlockPrefab = BlockPrefab;
        }

        public void UpdateAttackCharge(int Attack)
        {

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
