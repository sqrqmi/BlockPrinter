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

        // x = (sin(t) + 1) / 2
        Sine,
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


        public void Fill(T Source)
        {
            for (int i = 0; i < Grid.Length; i++)
            {
                Grid[i] = Source;
            }
        }

        public static void Copy(ref Field2d<T> Destination, in Field2d<T> Source)
        {
            if(Destination.IsNull())
            {
                Destination = new Field2d<T>(Source.Size);
            }
            for(int y = 0; y < Source.Size.y; y++)
            {
                for(int x = 0;  x < Source.Size.x; x++)
                {
                    Vector2Int Pos = new Vector2Int(x, y);
                    Destination[Pos] = Source[Pos];
                }
            }
        }
    }

    public static class Util
    {
        public static Vector2Int ToVector2Int(Direction Dir)
        {
            Vector2Int Result = Vector2Int.zero;
            if ((Dir & Direction.Right) != Direction.None) { Result += Vector2Int.right; }
            if ((Dir & Direction.Up) != Direction.None) { Result += Vector2Int.up; }
            if ((Dir & Direction.Left) != Direction.None) { Result += Vector2Int.left; }
            if ((Dir & Direction.Down) != Direction.None) { Result += Vector2Int.down; }
            return Result;
        }

        public static Direction RotateDirection(Direction l, Direction r)
        {
            int Shift = 0;
            switch (r)
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
            if (v.x > 0) { Dir |= Direction.Right; }
            if (v.x < 0) { Dir |= Direction.Left; }
            if (v.y > 0) { Dir |= Direction.Up; }
            if (v.y < 0) { Dir |= Direction.Down; }
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
            switch (Mode)
            {
                case InterpolationMode.Linear:
                    return t;

                case InterpolationMode.QuadraticAccel:
                    return t * t;

                case InterpolationMode.QuadraticBrake:
                    return (-(t - 1.0f) * (t - 1.0f) + 1.0f);

                case InterpolationMode.Smooth:
                    return (Mathf.Exp(-1.0f / (t * t) + 1.0f));

                case InterpolationMode.Sine:
                    return (Mathf.Sin(t * (0.5f * Mathf.PI)) + 1.0f) * 0.5f;
            }
            return 0.0f;
        }
    }

    public static class BinaryOperations
    {
        public static int CountBit(int Flags)
        {
            int Sum = 0;
            for (int i = 0; i < 32; i++)
            {
                if ((Flags & (1 << i)) != 0)
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
            for (int i = 0; i < Size; i++)
            {
                NewMoveTo[i] = i;
            }
            return new Permutation(NewMoveTo);
        }

        public void Reset()
        {
            for (int i = 0; i < MoveTo.Length; i++)
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
            for (int i = 0; i < Iteration; i++)
            {
                if ((Flags & (1 << MoveTo[i])) != 0)
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

        public Line(Vector2Int Origin, Vector2Int Delta)
        {
            this.Origin = Origin;
            this.Delta = Delta;
        }

        public Vector2Int ToPosition(int Index)
        {
            return Delta * Index + Origin;
        }
    }

    [Serializable]
    public struct Transform2d
    {
        public static readonly Transform2d Identity = new Transform2d(Vector2Int.right, Vector2Int.up, Vector2Int.zero);
        public Vector2Int x, y, o;

        public Transform2d(Vector2Int x, Vector2Int y, Vector2Int o)
        {
            this.x = x;
            this.y = y;
            this.o = o;
        }

        public Vector2Int Transform(Vector2Int v)
        {
            return x * v.x + y * v.y + o;
        }

        public Bounds2d Transform(Bounds2d b)
        {
            Vector2Int tb = Transform(b.Bottom);
            Vector2Int tt = Transform(b.Top);
            return new Bounds2d(new Vector2Int(Mathf.Min(tb.x, tt.x), Mathf.Min(tb.y, tt.y)),
                                new Vector2Int(Mathf.Max(tb.x, tt.x), Mathf.Max(tb.y, tt.y)));
        }

        public Transform2d Rotate()
        {
            return new Transform2d(y, -x, o);
        }
        public Transform2d Flip()
        {
            return new Transform2d(-x, y, o);
        }

    }

    [Serializable]
    public struct Bounds2d
    {
        public Vector2Int Bottom;
        public Vector2Int Top;

        public Bounds2d(Vector2Int bottom, Vector2Int top)
        {
            this.Bottom = bottom;
            this.Top = top;
        }

        public Vector2Int CalcSize()
        {
            return Top - Bottom;
        }

        public static bool IsSimilar(in Bounds2d l, in Bounds2d r)
        {
            Vector2Int LSize = l.CalcSize();
            Vector2Int RSize = r.CalcSize();
            return LSize == RSize || (LSize.x == RSize.y && LSize.y == RSize.x);
        }

        public static Bounds2d BoundsOf(Vector2Int[] Points, int Count)
        {
            Vector2Int b = new Vector2Int(int.MaxValue, int.MaxValue);
            Vector2Int t = new Vector2Int(int.MinValue, int.MinValue);
            for (int i = 0; i < Count; i++)
            {
                b = new Vector2Int(Mathf.Min(Points[i].x, b.x), Mathf.Min(Points[i].y, b.y));
                t = new Vector2Int(Mathf.Max(Points[i].x, t.x), Mathf.Max(Points[i].y, t.y));
            }
            return new Bounds2d(b, t);
        }
    }

    public static class CommonInput
    {
        public static bool GetKeyDown(Direction Dir)
        {
            switch(Dir)
            {
                case Direction.None: return false;
                    case Direction.Left: return Input.GetAxis("Horizontal") < -0.5f || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);
                    case Direction.Right: return Input.GetAxis("Horizontal") > +0.5f || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
                case Direction.Up: return Input.GetAxis("Vertical") > +0.5f || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow);
                case Direction.Down: return Input.GetAxis("Vertical") < -0.5f || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);

            }
            return false;
        }
    }
}