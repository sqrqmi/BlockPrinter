using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace BlockPrinter.UserInterface
{
    public class BreakedPolyominosDisplay : MonoBehaviour
    {
        private BlockAppearence BlockPrefab;
        [SerializeField] private PolyominoDisplay PolyominoPrefab;
        [SerializeField] private Vector3 Offset;

        private PolyominoDisplay[] PolyominoDisplays;

        private Vector2Int[][] Polyominos;


        public void Initialize(Vector2Int[][] Polyominos, BlockAppearence BlockPrefab)
        {
            PolyominoDisplays = new PolyominoDisplay[Polyominos.Length];
            this.BlockPrefab = BlockPrefab;
            this.Polyominos = Polyominos;
            for(int i = 0; i < Polyominos.Length; i++)
            {
                PolyominoDisplays[i] = Instantiate(PolyominoPrefab);
                PolyominoDisplays[i].transform.SetParent(this.transform);
                PolyominoDisplays[i].Initialize(Polyominos[i], this.BlockPrefab, Offset * i, BlockColor.Red);
                PolyominoDisplays[i].Fade();
            }
        }

        //リセット処理
        public void DiscardInstances()
        {
            DestroyAll();
            Initialize(this.Polyominos, this.BlockPrefab);
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

        private void DestroyAll()
        {
            foreach( var polyomino in this.PolyominoDisplays )
            {
                Destroy(polyomino.gameObject);
            }
        }
    }
}
