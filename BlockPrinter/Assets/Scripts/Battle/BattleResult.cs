using System.Collections;
using UnityEngine;

namespace BlockPrinter.Battle
{
    public class BattleResult : MonoBehaviour
    {
        private BattleModeMaster Master;

        public void Initialize(BattleModeMaster Master)
        {
            this.Master = Master;
        }

        public void ShowResults(int WinnerId)
        {
            StartCoroutine(InternalRoutine());
            IEnumerator InternalRoutine()
            {


                yield break;
            }
        }
    }
}
