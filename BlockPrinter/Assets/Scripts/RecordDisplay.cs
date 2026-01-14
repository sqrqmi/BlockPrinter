using System;
using System.Collections;
using UnityEngine;
using Util;

namespace BlockPrinter.UserInterface
{



    public class RecordDisplay : MonoBehaviour
    {
        [SerializeField] private BitmapText ScoreText;
        [SerializeField] private BitmapText PlayTimeText;
        [SerializeField] private BitmapText PlacedBlockCountText;

        private RecordInfo CurrentRecord;

        public void UpdateRecord(RecordInfo Record)
        {
            CurrentRecord = Record;

            if(ScoreText != null)
            {
                ScoreText.UpdateText(CurrentRecord.Score.ToString());
            }
            if(PlayTimeText != null)
            {
                PlayTimeText.UpdateText(CurrentRecord.PlayTime.ToString());
            }
            if(PlacedBlockCountText != null)
            {
                PlacedBlockCountText.UpdateText(CurrentRecord.PlacedBlockCount.ToString());
            }
        }
    }
}
