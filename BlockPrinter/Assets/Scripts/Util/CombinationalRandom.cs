using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    public struct CombinationalRandom32 
    {
        public int RemainFlags;
        public int RemainCount;

        private CombinationalRandom32(int Remains, int Count)
        {
            this.RemainFlags = Remains;
            this.RemainCount = Count;
        }


        public static CombinationalRandom32 Flags(int Flags_)
        {
            int Sum = 0;
            for(int i = 0; i < 32; i++)
            {
                Sum += (Flags_ >> i) & 1;
            }
            return new CombinationalRandom32(Flags_, Sum);
        }

        public static CombinationalRandom32 Count(int Quantity)
        {
            return new CombinationalRandom32((1 << Quantity) - 1, Quantity);
        }

        public int PopNextValue()
        {
            int Rand = UnityEngine.Random.Range(0, RemainCount);
            for(int i = 0; i < 32; i++)
            {
                Rand -= (RemainFlags >> i) & 1;
                if(Rand < 0)
                {
                    RemainFlags &= ~(1 << i);
                    RemainCount--;

                    return i;
                }
            }
            return 0;
        }

        public IEnumerable<int> Enum()
        {
            while(RemainCount > 0)
            {
                yield return PopNextValue();
            }
            yield break;
        }
    }

    public struct CombinationalRandomField2d
    {
        public Field2d<bool> RemainField;
        public Vector2Int Size;
        public int RemainCount;

        private CombinationalRandomField2d(Field2d<bool> NewRemain, Vector2Int NewSize)
        {
            RemainField = NewRemain;
            int Sum = 0;
            for(int y = 0; y < RemainField.Size.y; y++)
            {
                for(int x = 0; x < RemainField.Size.x; x++)
                {
                    if(RemainField[new Vector2Int(x, y)])
                    {
                        Sum++;
                    }
                }
            }
            Size = NewSize;
            RemainCount = Sum;
        }

        public static CombinationalRandomField2d Create(Vector2Int NewSize)
        {
            return new CombinationalRandomField2d(new Field2d<bool>(NewSize), NewSize);
        }

        public bool IsNull()
        {
            return RemainField.IsNull();
        }

        public void Reset(in Field2d<bool> Destination)
        {
            int Sum = 0;
            int Area = RemainField.GetArea();
            for(int i = 0; i < Area; i++)
            {
                RemainField.Grid[i] = Destination.Grid[i];
                if(RemainField.Grid[i])
                {
                    Sum++;
                }
            }
            RemainCount = Sum;
        }

        public IEnumerable<Vector2Int> Enum()
        {
            while(RemainCount > 0)
            {
                int Rand = UnityEngine.Random.Range(0, RemainCount);
                for(int y = 0; y < RemainField.Size.y; y++)
                {
                    for(int x = 0; x < RemainField.Size.x; x++)
                    {
                        if(RemainField[new Vector2Int(x, y)])
                        {
                            Rand--;
                            if(Rand < 0)
                            {
                                yield return new Vector2Int(x, y);
                                RemainField[new Vector2Int(x, y)] = false;
                                goto BREAK_DOUBLE_LOOP;
                            }
                        }
                    }
                }
            BREAK_DOUBLE_LOOP:;
                RemainCount--;
            }
        }

    }
}
