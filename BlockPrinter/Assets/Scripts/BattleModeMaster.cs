using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BlockPrinter
{
    public class BattleModeMaster : MonoBehaviour
    {
        [SerializeField] private FieldSystem[] FieldSystems;

        [SerializeField] private float ReadyWaitTime;



        private int[] Wins;

        private void Start()
        {
            Initialize();
            StartRound();
        }

        public void Initialize()
        {
            Wins = new int[FieldSystems.Length];
            for(int i = 0; i < FieldSystems.Length; i++)
            {
                Wins[i] = 0;
            }
        }

        public void StartRound()
        {
            StartCoroutine(InternalRoutine());
            IEnumerator InternalRoutine()
            {

                for (int i = 0; i < FieldSystems.Length; i++)
                {
                    FieldSystems[i].Initialize(i, OnSendAttackCharge, OnGameOver);
                }
                yield return new WaitForSeconds(ReadyWaitTime);

                for (int i = 0; i < FieldSystems.Length; i++)
                {
                    FieldSystems[i].SetState(FieldSystem.State.Active);
                }
                yield break;
            };
        }

        public void OnSendAttackCharge(int FieldId, int AttackCharge)
        {
            for(int i = 0; i < FieldSystems.Length;i++)
            {
                if(i == FieldId)
                {
                    continue;
                }
                FieldSystems[i].TakeDamage(AttackCharge);
            }
        }

        public void OnGameOver(int FieldId)
        {
            int RemainsCount = 0;
            int LastFieldId = -1;
            for(int i = 0; i < FieldSystems.Length; i++)
            {
                if (!FieldSystems[i].IsGameOver())
                {
                    RemainsCount++;
                    LastFieldId = i;
                }
            }
            if(RemainsCount <= 1)
            {
                Debug.Log($"Winner: {LastFieldId}");
                RoundSet(LastFieldId);
                StartCoroutine(InternalRoutine());
                IEnumerator InternalRoutine()
                {
                    float WaitTime = 2.0f; 
                    while(WaitTime >= 0.0f)
                    {
                        if(Input.GetKeyDown(KeyCode.Return))
                        {
                            StartRound();
                            yield break;
                        }
                        WaitTime -= Time.deltaTime;
                        yield return null;
                    }
                    LeaveMode();
                    yield break;
                }
            }
        }

        public void RoundSet(int WinnerId)
        {
            Wins[WinnerId]++;
            for(int i = 0; i < FieldSystems.Length; i++)
            {
                FieldSystems[i].SetState(FieldSystem.State.Disable);
            }
        }

        public void LeaveMode()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");

        }
    }
}
