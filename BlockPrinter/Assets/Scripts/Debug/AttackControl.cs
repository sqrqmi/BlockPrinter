using UnityEngine;

namespace BlockPrinter._Debug
{
    public class AttackControl : MonoBehaviour
    {
        [SerializeField] private FieldSystem System;

        [SerializeField] private KeyCode ForceAttackKey;
        [SerializeField] private KeyCode ChargeBigAttackKey;

        void Update()
        {
            if(Input.GetKeyDown(ForceAttackKey))
            {
                System.SendAttackCharge();
            }
            if(Input.GetKeyDown(ChargeBigAttackKey))
            {
                System.EarnScore(+234);
            }
        }
    }
}
