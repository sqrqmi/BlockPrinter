using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BlockPrinter
{
    public class BattleModeMaster : MonoBehaviour
    {
        [SerializeField] private FieldSystem[] FieldSystems;

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            for(int i = 0; i < FieldSystems.Length; i++)
            {
                FieldSystems[i].Initialize(i, OnSendAttackCharge, OnGameOver);
            }
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
                UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
            }
        }
    }
}
