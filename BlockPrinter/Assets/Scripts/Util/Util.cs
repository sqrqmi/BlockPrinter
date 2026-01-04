using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    [Flags]
    public enum Direction
    {
        None = 0,
        Right = 1,
        Up = 2,
        Left = 4,
        Down = 8,
    }

    public enum InterpolationMode
    {
        // x = at
        Linear,

        // x = t^2
        QuadraticAccel,

        // x = -(t - 1)^2 + 1
        QuadraticBrake,

        // x = e^(-1/t^2 + 1)
        Smooth,
    }


    [Serializable]
    public struct Field2d<T>
    {
        public Vector2Int Size;
        public T[] Grid;

        public Field2d(Vector2Int size)
        {
            Size = size;
            Grid = new T[Size.x * Size.y];
        }

        public ref T this[Vector2Int pos]
        {
            get { return ref Grid[ToIndex(pos)]; }
            // set { Grid[ToIndex(pos)] = value; }
        }

        public int ToIndex(Vector2Int pos)
        {
            return pos.y * Size.x + pos.x;
        }

        public Vector2Int ToVector2Int(int Index)
        {
            return new Vector2Int(Index % Size.x, Index / Size.x);
        }

        public int GetArea()
        {
            return Size.x * Size.y;
        }

        public bool IsIn(Vector2Int pos)
        {
            return 0 <= pos.x && pos.x < Size.x
                && 0 <= pos.y && pos.y < Size.y;
        }

        public bool IsNull()
        {
            return Grid == null;
        }

    }

    public static class Util
    {
        public static Vector2Int ToVector2Int(Direction Dir)
        {
            Vector2Int Result = Vector2Int.zero;
            if((Dir & Direction.Right) != Direction.None) { Result += Vector2Int.right; }
            if((Dir & Direction.Up) != Direction.None) { Result += Vector2Int.up; }
            if((Dir & Direction.Left) != Direction.None) { Result += Vector2Int.left; }
            if((Dir & Direction.Down) != Direction.None) { Result += Vector2Int.down; }
            return Result;
        }

        public static Direction RotateDirection(Direction l, Direction r)
        {
            int Shift = 0;
            switch(r)
            {
                case Direction.Right: Shift = 0; break;
                case Direction.Up: Shift = 1; break;
                case Direction.Left: Shift = 2; break;
                case Direction.Down: Shift = 3; break;
            }
            int temp = (int)l << Shift;
            return (Direction)((temp & 15) | ((temp >> 4) & 15));
        }

        public static Direction RelativeDirection(Vector2Int v)
        {
            Direction Dir = Direction.None;
            if(v.x > 0) { Dir |= Direction.Right; }
            if(v.x < 0) { Dir |= Direction.Left; }
            if(v.y > 0) { Dir |= Direction.Up; }
            if(v.y < 0) { Dir |= Direction.Down; }
            return Dir;
        }

        public static Vector2Int RandomInsideRect(Vector2Int Size)
        {
            return new Vector2Int(UnityEngine.Random.Range(0, Size.x), UnityEngine.Random.Range(0, Size.y));
        }

        public static void Swap<T>(ref T l, ref T r)
        {
            T temp = l;
            l = r;
            r = temp;
        }

        //public static T PushPop<T>(T[] Arr, T NewValue)
        //{

        //}

        public static Vector2Int ClampField(Vector2Int Size, Vector2Int Position)
        {
            Vector2Int Result = Position;
            Result.x = Mathf.Min(Mathf.Max(0, Result.x), Size.x - 1);
            Result.y = Mathf.Min(Mathf.Max(0, Result.y), Size.y - 1);
            return Result;

        }

        public static float ApplyInterpolation(InterpolationMode Mode, float t)
        {
            switch(Mode)
            {
                case InterpolationMode.Linear:
                    return t;

                case InterpolationMode.QuadraticAccel:
                    return t * t;

                case InterpolationMode.QuadraticBrake:
                    return (-(t - 1.0f) * (t - 1.0f) + 1.0f);

                case InterpolationMode.Smooth:
                    return (Mathf.Exp(-1.0f / (t * t) + 1.0f));
            }
            return 0.0f;
        }
    }

    public static class BinaryOperations
    {
        public static int CountBit(int Flags)
        {
            int Sum = 0;
            for(int i = 0; i < 32; i++)
            {
                if((Flags & (1 << i)) != 0)
                {
                    Sum++;
                }
            }
            return Sum;
        }
    }

    [Serializable]
    public struct Permutation
    {
        public int[] MoveTo;

        public Permutation(int[] NewMoveTo)
        {
            MoveTo = NewMoveTo;
        }

        public static Permutation Identity(int Size)
        {
            int[] NewMoveTo = new int[Size];
            for(int i = 0; i < Size; i++)
            {
                NewMoveTo[i] = i;
            }
            return new Permutation(NewMoveTo);
        }

        public void Reset()
        {
            for(int i = 0; i < MoveTo.Length; i++)
            {
                MoveTo[i] = i;
            }
        }

        public void Alternate(int L, int R)
        {
            Util.Swap(ref MoveTo[L], ref MoveTo[R]);
        }

        public ref T Apply<T>(T[] Destination, int Index)
        {
            return ref Destination[MoveTo[Index]];
        }

        public int ApplyFrom(int Index)
        {
            return MoveTo[Index];
        }

        public int ApplyBitwise(int Flags)
        {
            int Result = 0;
            int Iteration = Mathf.Min(MoveTo.Length, 32);
            for(int i = 0; i < Iteration; i++)
            {
                if((Flags & (1 << MoveTo[i])) != 0)
                {
                    Result |= 1 << i;
                }
            }
            return Result;
        }


    }

    [Serializable]
    public struct Line
    {
        public Vector2Int Origin;
        public Vector2Int Delta;

        public Line(Vector2Int  Origin, Vector2Int Delta)
        {
            this.Origin = Origin;
            this.Delta = Delta;
        }

        public Vector2Int ToPosition(int Index)
        {
            return Delta * Index + Origin;
        }
    }

}
