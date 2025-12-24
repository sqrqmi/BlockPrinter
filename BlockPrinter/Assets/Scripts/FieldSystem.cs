using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlockPrinter
{
    #region __DontCare
#if !UNITY_EDITOR
    public struct Vector2Int
    {
        public int x, y;

        public Vector2Int(int x, int y)
        {

        }
    }

    public struct Vector3
    {
        public float x, y, z;

        public Vector3(float x, float y, float z)
        {

        }

        public Vector3(float x, float y)
        {

        }
    }

    public struct Color
    {

    }

    public class SerializeFieldAttribute : Attribute
    { }

    [Serializable]
    public enum BlockColor
    {
        None,
        Red,
        Blue,
        Green,
        Yellow,
    }

    public class Debug
    {
        public static void Log(string str) { }
    }

    public class Gizmos
    {

    }


#endif
    #endregion

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

    [Serializable]
    public struct Field2d<T>
    {
        public Vector2Int Size;
        public T[] Array;

        public Field2d(Vector2Int s)
        {
            Size = s;
            Array = new T[s.x * s.y];
        }

        public ref T this[Vector2Int v]
        {
            get { return ref Array[Map(v)]; }
        }

        public int Map(Vector2Int v)
        {
            return v.y * Size.x + v.x;
        }

        public bool IsIn(Vector2Int v)
        {
            return 0 <= v.x && v.x < Size.x && 0 <= v.y && v.y < Size.y;
        }

        public void Fill(T Source)
        {
            for (int i = 0; i < Array.Length; i++)
            {
                Array[i] = Source;
            }
        }
    }

    [Serializable]
    public struct FieldLayout
    {
        public Vector3 AxisX;
        public Vector3 AxisY;
        public Vector3 AxisZ;

        public Vector3 Pivot;

        public Vector3 Transform(Vector2Int f)
        {
            return AxisX * f.x + AxisY * f.y + Pivot;
        }
    }



    [Serializable]
    public struct BlockElement
    {
        public static readonly BlockElement Empty = new BlockElement();
        public BlockAppearence Appearence;

        public BlockColor Color;

        public void Initialize(BlockAppearence Appearence, Vector3 Pos, BlockColor Color)
        {
            this.Appearence = Appearence;
            this.Color = Color;
            Appearence.Initialize(Pos, Color);
        }

        public bool IsFilled()
        {
            return Color != BlockColor.None;
        }

        public void SetBlock(BlockColor NewColor)
        {
            Color = NewColor;
            Appearence.SetAppearence(Color);
        }
    }

    public static class PolyominoDatabase
    {
        public static readonly Vector2Int[] AdjacentRelations = new Vector2Int[]
            {
                      new(0, +1),
                new(-1, 0), new(+1, 0),
                      new(0, -1),
            };

        public static readonly Transform2d[] SimilarTransforms = new Transform2d[]
            {
                Transform2d.Identity,
                Transform2d.Identity.Rotate(),
                Transform2d.Identity.Rotate().Rotate(),
                Transform2d.Identity.Rotate().Rotate().Rotate(),
                Transform2d.Identity.Flip(),
                Transform2d.Identity.Flip().Rotate(),
                Transform2d.Identity.Flip().Rotate().Rotate(),
                Transform2d.Identity.Flip().Rotate().Rotate().Rotate(),
            };


        public static readonly Vector2Int[][] Tetriminos = new Vector2Int[][]
        {
            new Vector2Int[] { new(0,0),new(1,0),new(2,0),new(3,0),}, // 4I
            new Vector2Int[] { new(0,0),new(1,0),new(2,0),new(0,1),}, // 4L
            new Vector2Int[] { new(0,0),new(1,0),new(2,0),new(1,1),}, // 4T
            new Vector2Int[] { new(0,0),new(1,0),new(1,1),new(2,1),}, // 4S
            new Vector2Int[] { new(0,0),new(1,0),new(0,1),new(1,1),}, // 4O
        };

        public static bool IsSimilar(Vector2Int[] l, Vector2Int[] r, int BlockCount)
        {
            if (l == null || r == null)
            {
                return false;
            }
            Bounds2d LBounds = Bounds2d.BoundsOf(l, BlockCount);
            Bounds2d RBounds = Bounds2d.BoundsOf(r, BlockCount);
            if (!Bounds2d.IsSimilar(LBounds, RBounds))
            {
                return false;
            }
            Vector2Int RPivot = RBounds.Bottom;
            foreach (Transform2d t in SimilarTransforms)
            {
                bool IsCorrectAll = true;
                Vector2Int LPivot = t.Transform(LBounds).Bottom;
                for (int li = 0; li < BlockCount; li++)
                {
                    bool IsCorrectThis = false;
                    Vector2Int lb = t.Transform(l[li]) - LPivot;
                    for (int ri = 0; ri < BlockCount; ri++)
                    {
                        Vector2Int rb = r[ri] - RPivot;
                        if (lb == rb)
                        {
                            IsCorrectThis = true;
                            break;
                        }
                    }
                    if (!IsCorrectThis)
                    {
                        IsCorrectAll = false;
                        break;
                    }

                }
                if (IsCorrectAll)
                {
                    return true;
                }
            }
            return false;
        }

    }

    [Serializable]
    public struct KeyConfig
    {
        public KeyCode LeftKey;
        public KeyCode RightKey;

        public bool IsInputLeft()
        {
            return Input.GetKeyDown(LeftKey);
        }
        public bool IsInputRight()
        {
            return Input.GetKeyDown(RightKey);
        }
    }



    public class FieldSystem : MonoBehaviour
    {
        private enum State
        {
            Disable,
            Active,
            Suspend,
            GameOver,
        }
        private State CurrentState = State.Disable;
        private int Identifier;

        [SerializeField] private Vector2Int FieldSize;
        [SerializeField] private int DeadlineHeight;
        [SerializeField] private FieldLayout Layout;
        [SerializeField] private BlockAppearence BlockPrefab;

        private Field2d<BlockElement> Field;
        private float CurrentUnitTime;

        [SerializeField] private PlayerCharacter Player;
        [SerializeField] private KeyConfig KeyConfig;
        private int HorizontalPosition;

        private BlockColor[] NextBlockColors;
        private float BlockBreakWaitTime;
        private float BlockFallWaitTime;
        bool IsPureChain;

        private int CurrentPureChain;
        private int CurrentActiveChain;
        private int CurrentAttackCharge;
        private BlockColor[] DamagedBlocks;
        private int CurrentDamagedBlockCount;
        private float CurrentDamageRemainingTime;

        private bool[] CurrentErasedShapeFlags;
        private int Score;

        private Action<int, int> OnSendAttackChargeCallback;
        private Action<int> OnGameOverCallback;

        //[Header("Effects")]
        //[SerializeField] private Effect.BlockBreak BlockBreakEffect;
        //[SerializeField] private Effect....

        [Header("User Interfaces")]
        [SerializeField] private UserInterface.CandidateBlockDisplay CandidateBlockDisplay;
        [SerializeField] private UserInterface.ScoreDisplay ScoreDisplay;
        [SerializeField] private UserInterface.BreakedPolyominosDisplay BreakedPolyominosDisplay;
        [SerializeField] private UserInterface.AttackChargeDisplay AttackChargeDisplay;
        [SerializeField] private UserInterface.DamageDisplay DamageDisplay;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(Layout.Transform(Vector2Int.zero), Layout.Transform(new Vector2Int(FieldSize.x, 0)));
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Layout.Transform(Vector2Int.zero), Layout.Transform(new Vector2Int(0, FieldSize.y)));
        }


        public void Initialize(int NewIdentifier, Action<int, int> OnSendAttackCharge, Action<int> OnGameOver)
        {
            DiscardInstances();
            Identifier = NewIdentifier;
            this.OnSendAttackChargeCallback = OnSendAttackCharge;
            this.OnGameOverCallback = OnGameOver;
            CurrentUnitTime = 0.5f;
            HorizontalPosition = FieldSize.x / 2;
            Player.Initialize(Layout.Transform(new Vector2Int(HorizontalPosition, FieldSize.y)));
            Field = new Field2d<BlockElement>(FieldSize);
            Field.Fill(BlockElement.Empty);
            for (int y = 0; y < FieldSize.y; y++)
            {
                for (int x = 0; x < FieldSize.x; x++)
                {
                    Vector2Int Pos = new Vector2Int(x, y);
                    BlockAppearence NewBlock = Instantiate(BlockPrefab);
                    Vector3 WorldPos = Layout.Transform(new Vector2Int(x, y));
                    Field[Pos].Initialize(NewBlock, WorldPos, BlockColor.None);
                }
            }
            NextBlockColors = new BlockColor[10];
            CandidateBlockDisplay.Initialize(NextBlockColors.Length);
            for (int i = 0; i < NextBlockColors.Length; i++)
            {
                AdvanceBlockCandidate();
            }
            IsPureChain = true;
            BlockBreakWaitTime = 0.0f;
            BlockFallWaitTime = 0.0f;
            CurrentPureChain = 0;
            CurrentActiveChain = 0;
            DamagedBlocks = new BlockColor[100];
            CurrentDamagedBlockCount = 0;
            CurrentDamageRemainingTime = 0.0f;
            CurrentErasedShapeFlags = new bool[PolyominoDatabase.Tetriminos.Length];
            for (int i = 0; i < CurrentErasedShapeFlags.Length; i++)
            {
                CurrentErasedShapeFlags[i] = false;
            }
            BreakedPolyominosDisplay.Initialize(PolyominoDatabase.Tetriminos, BlockPrefab);
            AttackChargeDisplay.Initialize(BlockPrefab);
            DamageDisplay.Initialize(BlockPrefab);
            CurrentState = State.Active;
        }

        public void Update()
        {
            switch (CurrentState)
            {
                case State.Disable:
                    break;
                case State.Active:
                    {
                        Tick();
                    }
                    break;
                case State.Suspend:
                    break;
                case State.GameOver:
                    break;
            }
        }

        private void Tick()
        {
            CurrentDamageRemainingTime -= Time.deltaTime;
            DamageDisplay.UpdateRemainingTime(CurrentDamageRemainingTime);
            if (IsBlockLanded())
            {
                BlockFallWaitTime -= Time.deltaTime;
            }
            else
            {
                BlockBreakWaitTime -= Time.deltaTime;
            }

            if (KeyConfig.IsInputLeft())
            {
                TryPlaceBlock(-1);
            }
            if (KeyConfig.IsInputRight())
            {
                TryPlaceBlock(+1);
            }
            if (IsFallStarted())
            {
                ApplyGravity();
                if (IsBlockLanded())
                {
                    CheckBlockBreak();
                }
            }
            if (IsReachGameOver())
            {
                GameOver();
            }
        }

        public void AdvanceBlockCandidate()
        {
            for (int i = 1; i < NextBlockColors.Length; i++)
            {
                NextBlockColors[i - 1] = NextBlockColors[i];
            }
            {
                BlockColor NextColor = BlockColor.None;
                if (CurrentDamagedBlockCount != 0)
                {
                    NextColor = DamagedBlocks[0];
                    for (int i = 0; i < CurrentDamagedBlockCount - 1; i++)
                    {
                        DamagedBlocks[i] = DamagedBlocks[i + 1];
                    }
                    CurrentDamagedBlockCount--;
                    DamagedBlocks[CurrentDamagedBlockCount] = BlockColor.None;
                    DamageDisplay.UpdateBlocks(DamagedBlocks);
                }
                else
                {
                    if (UnityEngine.Random.value < 0.5f)
                    {
                        NextColor = BlockColor.Red;
                    }
                    else
                    {
                        NextColor = BlockColor.Blue;
                    }
                }
                NextBlockColors[NextBlockColors.Length - 1] = NextColor;

            }

            CandidateBlockDisplay.SetNextBlock(NextBlockColors);
        }

        public bool TryPlaceBlock(int HorizontalDelta)
        {
            int Column = Mathf.Clamp(HorizontalPosition + HorizontalDelta, 0, FieldSize.x - 1);
            for (int y = FieldSize.y - 1; y >= 0; y--)
            {
                Vector2Int Pos = new Vector2Int(Column, y);
                if (Field[Pos].IsFilled())
                {
                    continue;
                }
                bool IsPlacable = false;
                if (!Field.IsIn(Pos + Vector2Int.down))
                {
                    IsPlacable = true;
                }
                else if (Field[Pos + Vector2Int.down].IsFilled())
                {
                    IsPlacable = true;
                }
                //foreach (Vector2Int AdjDelta in PolyominoDatabase.AdjacentRelations)
                //{
                //    Vector2Int CheckPos = Pos + AdjDelta;
                //    if (!Field.IsIn(CheckPos))
                //    {
                //        continue;
                //    }
                //    if (Field[CheckPos].Color == Field[Pos].Color)
                //    {
                //        IsPlacable = true;
                //        break;
                //    }
                //}
                if (IsPlacable)
                {
                    Field[Pos].SetBlock(NextBlockColors[0]);
                    Field[Pos].Appearence.OnMove(Layout.Transform(new Vector2Int(Column, FieldSize.y)), Layout.Transform(Pos), CurrentUnitTime);
                    UpdateCharacterPosition(Column);
                    AdvanceBlockCandidate();
                    IsPureChain = false;
                    BlockBreakWaitForSeconds(CurrentUnitTime);
                    return true;
                }

            }
            return false;
        }

        public bool IsBlockLanded()
        {
            return BlockBreakWaitTime <= 0.0f;
        }

        public bool IsFallStarted()
        {
            return BlockFallWaitTime <= 0.0f;
        }

        private Vector2Int[] _Adjacents;
        private Field2d<bool> _CheckedBlocks;
        public void CheckBlockBreak()
        {
            if (_Adjacents == null || _Adjacents.Length != FieldSize.x * FieldSize.y)
            {
                _Adjacents = new Vector2Int[FieldSize.x * FieldSize.y];
            }
            if (_CheckedBlocks.Size != FieldSize)
            {
                _CheckedBlocks = new Field2d<bool>(FieldSize);
            }
            _CheckedBlocks.Fill(false);
            bool IsBreaked = false;
            for (int y = 0; y < FieldSize.y; y++)
            {
                for (int x = 0; x < FieldSize.x; x++)
                {
                    Vector2Int Pos = new Vector2Int(x, y);
                    if (_CheckedBlocks[Pos])
                    {
                        continue;
                    }
                    if (Field[Pos].Color == BlockColor.None)
                    {
                        continue;
                    }
                    int AdjacentCount = SearchAdjacents(Pos);
                    for (int pi = 0; pi < PolyominoDatabase.Tetriminos.Length; pi++)
                    {
                        Vector2Int[] Polyomino = PolyominoDatabase.Tetriminos[pi];
                        //if (CurrentErasedShapeFlags[pi])
                        //{
                        //    continue;
                        //}
                        if (AdjacentCount != Polyomino.Length)
                        {
                            continue;
                        }
                        if (PolyominoDatabase.IsSimilar(_Adjacents, Polyomino, Polyomino.Length))
                        {
                            for (int bi = 0; bi < AdjacentCount; bi++)
                            {
                                Vector2Int v = _Adjacents[bi];
                                Field[v].Appearence.OnBreakBlock();
                                Field[v].SetBlock(BlockColor.None);
                            }
                            if (!IsBreaked)
                            {
                                if (IsPureChain)
                                {
                                    CurrentPureChain++;
                                }
                                CurrentActiveChain++;
                            }
                            IsBreaked = true;
                            EarnScore(CalcBlockBreakScore(Field[Pos].Color, CurrentPureChain, CurrentActiveChain));
                            MarkPolyomino(pi);
                            BlockFallWaitForSeconds(CurrentUnitTime);
                            break;

                        }
                    }

                }
            }
            if (!IsBreaked)
            {
                ResetChain();
            }
            IsPureChain = true;
        }

        private int SearchAdjacents(Vector2Int Origin)
        {
            BlockColor Color = Field[Origin].Color;
            int AdjacentsIndex = 1;
            _Adjacents[0] = Origin;
            for (int i = 0; i < AdjacentsIndex; i++)
            {
                foreach (Vector2Int AdjDelta in PolyominoDatabase.AdjacentRelations)
                {
                    Vector2Int CheckPos = _Adjacents[i] + AdjDelta;
                    if (!Field.IsIn(CheckPos))
                    {
                        continue;
                    }
                    if (Field[CheckPos].Color != Color)
                    {
                        continue;
                    }
                    bool IsDuplicate(Vector2Int Pos, int AdjacentsIndex)
                    {
                        for (int k = 0; k < AdjacentsIndex; k++)
                        {
                            if (_Adjacents[k] == Pos)
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                    if (IsDuplicate(CheckPos, AdjacentsIndex))
                    {
                        continue;
                    }
                    _Adjacents[AdjacentsIndex] = CheckPos;
                    AdjacentsIndex++;
                }
            }
            return AdjacentsIndex;
        }

        public void ApplyGravity()
        {
            for (int x = 0; x < FieldSize.x; x++)
            {
                int Bottom = 0;
                for (int y = 0; y < FieldSize.y; y++)
                {
                    Vector2Int Pos = new Vector2Int(x, y);
                    if (Field[Pos].IsFilled())
                    {
                        if (y != Bottom)
                        {
                            Vector2Int BottomPos = new Vector2Int(x, Bottom);
                            BlockColor temp = Field[Pos].Color;
                            Field[Pos].SetBlock(Field[BottomPos].Color);
                            Field[BottomPos].SetBlock(temp);
                            Field[BottomPos].Appearence.OnMove(Layout.Transform(Pos), Layout.Transform(BottomPos), CurrentUnitTime);
                            BlockBreakWaitForSeconds(CurrentUnitTime);
                        }
                        Bottom++;
                    }
                }
            }
        }

        public void RecoverLargeChunks()
        {
            if (_Adjacents == null || _Adjacents.Length != FieldSize.x * FieldSize.y)
            {
                _Adjacents = new Vector2Int[FieldSize.x * FieldSize.y];
            }
            if (_CheckedBlocks.Size != FieldSize)
            {
                _CheckedBlocks = new Field2d<bool>(FieldSize);
            }
            _CheckedBlocks.Fill(false);
            for (int y = 0; y < FieldSize.y; y++)
            {
                for (int x = 0; x < FieldSize.x; x++)
                {
                    Vector2Int Pos = new Vector2Int(x, y);
                    if (_CheckedBlocks[Pos])
                    {
                        continue;
                    }
                    if (Field[Pos].Color == BlockColor.None)
                    {
                        continue;
                    }
                    int AdjacentCount = SearchAdjacents(Pos);
                    if (AdjacentCount > 4)
                    {
                        for (int i = 0; i < AdjacentCount; i++)
                        {
                            Vector2Int DeletePos = _Adjacents[i];
                            Field[DeletePos].SetBlock(BlockColor.None);
                        }
                    }
                }
            }
        }

        public bool IsReachGameOver()
        {
            if (CurrentDamagedBlockCount != 0 && CurrentDamageRemainingTime < 0.0f)
            {
                return true;
            }
            if (!IsBlockLanded())
            {
                return false;
            }
            if (!IsFallStarted())
            {
                return false;
            }
            for (int y = DeadlineHeight; y < FieldSize.y; y++)
            {
                for (int x = 0; x < FieldSize.x; x++)
                {
                    if (Field[new Vector2Int(x, y)].IsFilled())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void GameOver()
        {
            Debug.Log("Game Over Called");
            CurrentState = State.GameOver;
            if (OnGameOverCallback != null)
            {
                OnGameOverCallback(Identifier);
            }
        }

        public void BlockBreakWaitForSeconds(float Seconds)
        {
            BlockBreakWaitTime = Mathf.Max(Seconds, BlockBreakWaitTime);
        }

        public void BlockFallWaitForSeconds(float Seconds)
        {
            BlockFallWaitTime = Mathf.Max(Seconds, BlockFallWaitTime);
        }

        public void MarkPolyomino(int Index)
        {
            CurrentErasedShapeFlags[Index] = true;
            BreakedPolyominosDisplay.SetVisible(Index, true);
            for (int i = 0; i < CurrentErasedShapeFlags.Length; i++)
            {
                if (!CurrentErasedShapeFlags[i])
                {
                    return;
                }
            }
            EarnCreateAllPolyominoBonus();
        }

        public void EarnCreateAllPolyominoBonus()
        {
            for (int i = 0; i < CurrentErasedShapeFlags.Length; i++)
            {
                CurrentErasedShapeFlags[i] = false;
            }
            EarnScore(+5);
            BreakedPolyominosDisplay.EffectMakeAllPolyomino();
            BreakedPolyominosDisplay.ResetBreakState();
            RecoverLargeChunks();
            SendAttackCharge();
        }

        public int CalcBlockBreakScore(BlockColor BreakedColor, int PureChain, int ActiveChain)
        {
            int ColorBonusMultiplier = 1;
            switch (BreakedColor)
            {
                case BlockColor.Green:
                    ColorBonusMultiplier = 10; break;
                case BlockColor.Yellow:
                    ColorBonusMultiplier = 50; break;
            }
            if (PureChain <= 0)
            {
                return +1 * ColorBonusMultiplier;
            }
            int Score = PureChain * PureChain * PureChain;
            if (ActiveChain > 1)
            {
                Score += 1;
            }
            return Score * ColorBonusMultiplier;
        }

        public void ResetChain()
        {
            CurrentPureChain = 0;
            CurrentActiveChain = 0;
        }

        public void EarnScore(int Increment)
        {
            Score += Increment;
            CurrentAttackCharge += Increment;
            ScoreDisplay.UpDateScore(Score);
            AttackChargeDisplay.UpdateAttackCharge(CurrentAttackCharge);
        }

        public void UpdateCharacterPosition(int NewHorizontalPosition)
        {
            Player.OnMove(Layout.Transform(new Vector2Int(HorizontalPosition, FieldSize.y)),
                          Layout.Transform(new Vector2Int(NewHorizontalPosition, FieldSize.y)),
                          CurrentUnitTime);
            HorizontalPosition = NewHorizontalPosition;
        }

        public void SendAttackCharge()
        {
            if (OnSendAttackChargeCallback != null)
            {
                OnSendAttackChargeCallback(Identifier, CurrentAttackCharge);
            }
            CurrentAttackCharge = 0;
            AttackChargeDisplay.UpdateAttackCharge(CurrentAttackCharge);
        }

        public void TakeDamage(int Damage)
        {
            int CurrentDamage = Damage;
            {
                int YellowCount = CurrentDamage / 100;
                CurrentDamage -= YellowCount * 100;
                int GreenCount = CurrentDamage / 10;
                CurrentDamage -= GreenCount * 10;
                int BlueCount = CurrentDamage - UnityEngine.Random.Range(0, CurrentDamage + 1);
                CurrentDamage -= BlueCount * 1;
                int RedCount = CurrentDamage;

                int BlockSum = YellowCount + GreenCount + BlueCount + RedCount;
                Debug.Log($"{BlockSum} Blocks Attacked From {Identifier}: Red: {RedCount}, Blue: {BlueCount}, Green: {GreenCount}, Yellow: {YellowCount}");
                if (CurrentDamagedBlockCount == 0)
                {
                    CurrentDamageRemainingTime = CurrentUnitTime * 20.0f;
                }
                for (int i = 0; i < BlockSum; i++)
                {
                    BlockColor NextColor = BlockColor.None;
                    int Rand = UnityEngine.Random.Range(0, BlockSum - i);
                    if (Rand < YellowCount + GreenCount + BlueCount + RedCount)
                    {
                        NextColor = BlockColor.Red;
                    }
                    if (Rand < YellowCount + GreenCount + BlueCount)
                    {
                        NextColor = BlockColor.Blue;
                    }
                    if (Rand < YellowCount + GreenCount)
                    {
                        NextColor = BlockColor.Green;
                    }
                    if (Rand < YellowCount)
                    {
                        NextColor = BlockColor.Yellow;
                    }

                    DamagedBlocks[CurrentDamagedBlockCount + i] = NextColor;
                    switch (NextColor)
                    {
                        case BlockColor.Red: RedCount--; break;
                        case BlockColor.Blue: BlueCount--; break;
                        case BlockColor.Green: GreenCount--; break;
                        case BlockColor.Yellow: YellowCount--; break;
                    }
                }
                CurrentDamagedBlockCount += BlockSum;
            }
            DamageDisplay.UpdateBlocks(DamagedBlocks);
        }

        public void DiscardInstances()
        {
            if (Field.Array != null)
            {
                foreach (BlockElement b in Field.Array)
                {
                    Destroy(b.Appearence.gameObject);
                }
            }
        }

        public bool IsGameOver()
        {
            return CurrentState == State.GameOver;
        }
    }


}
