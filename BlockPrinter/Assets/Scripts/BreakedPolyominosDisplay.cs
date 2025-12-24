using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockPrinter.UserInterface
{
    public class BreakedPolyominosDisplay : MonoBehaviour
    {
        private BlockAppearence BlockPrefab;
        [SerializeField] private PolyominoDisplay PolyominoPrefab;
        [SerializeField] private Vector3 Offset;

        private PolyominoDisplay[] PolyominoDisplays;


        public void Initialize(Vector2Int[][] Polyominos, BlockAppearence BlockPrefab)
        {
            PolyominoDisplays = new PolyominoDisplay[Polyominos.Length];
            this.BlockPrefab = BlockPrefab;
            for(int i = 0; i < Polyominos.Length; i++)
            {
                PolyominoDisplays[i] = Instantiate(PolyominoPrefab);
                PolyominoDisplays[i].transform.SetParent(this.transform);
                PolyominoDisplays[i].Initialize(Polyominos[i], this.BlockPrefab, Offset * i, BlockColor.Red);
            }
        }
        
        public void SetVisible(int Index, bool IsVisible)
        {
            if(IsVisible) 
            {
                PolyominoDisplays[Index].Highlight();
            }
            else
            {
                PolyominoDisplays[Index].Fade();
            }
        }

        public void ResetBreakState()
        {
            for(int i = 0; i < PolyominoDisplays.Length; i++)
            {
                SetVisible(i, false);
            }
        }

        public void EffectMakeAllPolyomino()
        {

        }
    }
}
