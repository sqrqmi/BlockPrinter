using UnityEngine;
using Util;

namespace BlockPrinter
{
    public class MatrixPrinter : MonoBehaviour
    {
        [SerializeField] private BlockAppearence BlockPrefab;

        [SerializeField] private TextAsset PrintingData;

        [SerializeField] private PlayerCharacter PlayerInstance;
        [SerializeField] private FieldLayout Layout;

        [SerializeField] private Vector2Int FieldSize;
        [SerializeField] private float PlaceSpan;   
        private Field2d<BlockColor> Field;
        private float WaitTime;

        private void Start()
        {
            WaitTime = 0.0f;
            Field = new Field2d<BlockColor>(FieldSize);
            int StrIndex = 0;
            for(int y = 0; y < FieldSize.y; y++)
            {
                for(int x = 0; x < FieldSize.x; x++)
                {
                    while(PrintingData.text[StrIndex] == '\n' && StrIndex < PrintingData.text.Length)
                    {
                        StrIndex++;
                    }
                    if(StrIndex >= PrintingData.text.Length)
                    {
                        return;
                    }
                    Field[new Vector2Int(x, y)] = (BlockColor)(PrintingData.text[StrIndex] - '0');
                    StrIndex++;
                }
            }
        }

        private void Update()
        {
            WaitTime -= Time.deltaTime;
            if(WaitTime < 0.0f)
            {
                WaitTime += PlaceSpan;

            }
        }

    }
}
