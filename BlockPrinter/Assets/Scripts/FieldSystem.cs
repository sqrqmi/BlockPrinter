using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

namespace BlockPrinter
{


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
            if (Appearence != null)
            {
                Appearence.Initialize(Pos, Color);
            }
        }

        public bool IsFilled()
        {
            return Color != BlockColor.None;
        }

        public void SetBlock(BlockColor NewColor)
        {
            Color = NewColor;
            if (Appearence != null)
            {
                Appearence.SetAppearence(Color);
            }
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


    public struct RecordInfo
    {
        public DateTime PlayDate;
        public int Score;
        public float PlayTime;
        public int PlacedBlockCount;


    }

    public class FieldSystem : MonoBehaviour
    {
        public enum State
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
        [SerializeField] private FieldController Controller;
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
        private float PlayTime;
        private int PlacedBlockCount;

        private Action<int, int> OnSendAttackChargeCallback;
        private Action<int> OnGameOverCallback;

        [Header("Effects")]
        [SerializeField] private Effect.GameOverEffect GameOverEffect;

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
            Identifier = NewIdentifier;
            this.OnSendAttackChargeCallback = OnSendAttackCharge;
            this.OnGameOverCallback = OnGameOver;
            CurrentUnitTime = 0.5f;
            HorizontalPosition = FieldSize.x / 2;
            Player.Initialize(Layout.Transform(new Vector2Int(HorizontalPosition, FieldSize.y)));
            Controller.Initialize(this);
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
            DamagedBlocks = new BlockColor[100];
            CurrentDamagedBlockCount = 0;
            CurrentDamageRemainingTime = 0.0f;
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
            CurrentAttackCharge = 0;

            CurrentErasedShapeFlags = new bool[PolyominoDatabase.Tetriminos.Length];
            for (int i = 0; i < CurrentErasedShapeFlags.Length; i++)
            {
                CurrentErasedShapeFlags[i] = false;
            }
            Score = 0;
            PlayTime = 0.0f;
            PlacedBlockCount = 0;
            GameOverEffect.Initialize();
            BreakedPolyominosDisplay.Initialize(PolyominoDatabase.Tetriminos, BlockPrefab);
            AttackChargeDisplay.Initialize(BlockPrefab);
            DamageDisplay.Initialize(BlockPrefab);
            CurrentState = State.Suspend;
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
            PlayTime += Time.deltaTime;
            if (CurrentDamagedBlockCount != 0)
            {
                CurrentDamageRemainingTime -= Time.deltaTime;
                DamageDisplay.UpdateRemainingTime(CurrentDamageRemainingTime);
            }
            if (IsBlockLanded())
            {
                BlockFallWaitTime -= Time.deltaTime;
            }
            else
            {
                BlockBreakWaitTime -= Time.deltaTime;
            }
            FieldControlInput CurrentInput = Controller.GetInput();
            if (CurrentInput.Left)
            {
                TryPlaceBlock(-1);
            }
            if (CurrentInput.Right)
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
                    if (CurrentDamagedBlockCount == 0)
                    {
                        CurrentDamageRemainingTime = 0.0f;
                        DamageDisplay.SetRemainingTimeVisible(false);
                    }
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
                    return false;
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
                    PlacedBlockCount++;
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
            int BreakedPolyominoCount = 0;
            int EarnedScoreLocalTotal = 0;
            for (int y = 0; y < FieldSize.y; y++)
            {
                for (int x = 0; x < FieldSize.x; x++)
                {
                    Vector2Int Pos = new Vector2Int(x, y);
                    BlockColor CheckColor = Field[Pos].Color;
                    if (_CheckedBlocks[Pos])
                    {
                        continue;
                    }
                    if (CheckColor == BlockColor.None)
                    {
                        continue;
                    }
                    int AdjacentCount = SearchAdjacents(Pos);
                    for (int pi = 0; pi < PolyominoDatabase.Tetriminos.Length; pi++)
                    {
                        Vector2Int[] Polyomino = PolyominoDatabase.Tetriminos[pi];
                        //if (DisallowPolyominoDuplication || CurrentErasedShapeFlags[pi])
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
                                if (!IsPureChain)
                                {
                                    CurrentPureChain = 0;
                                }
                                CurrentPureChain++;
                                CurrentActiveChain++;
                            }
                            IsBreaked = true;
                            BreakedPolyominoCount++;
                            EarnedScoreLocalTotal += CalcBlockBreakScore(CheckColor, CurrentPureChain, CurrentActiveChain);
                            MarkPolyomino(pi);
                            if (IsExistsFloatingBlock())
                            {
                                BlockFallWaitForSeconds(CurrentUnitTime);
                            }
                            break;

                        }
                    }

                }
            }
            if (BreakedPolyominoCount != 0)
            {
                EarnScore(EarnedScoreLocalTotal * BreakedPolyominoCount);
                
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

        public bool IsExistsFloatingBlock()
        {
            for (int x = 0; x < FieldSize.x; x++)
            {
                bool IsEmpty = false;
                for (int y = 0; y < FieldSize.y; y++)
                {
                    if (Field[new Vector2Int(x, y)].Color == BlockColor.None)
                    {
                        IsEmpty = true;
                    }
                    else if (IsEmpty)
                    {
                        return true;
                    }
                }
            }
            return false;
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
                            Field[DeletePos].Appearence.OnRecoverLargeChunk();
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

        public void SetState(State NewState)
        {
            CurrentState = NewState;
        }

        public void GameOver()
        {
            Debug.Log("Game Over Called");
            CurrentState = State.GameOver;
            GameOverEffect.OnGameOver();
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

        public static int CalcBlockBreakScore(BlockColor BreakedColor, int PureChain, int ActiveChain)
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
                    if(CurrentDamagedBlockCount + i >= DamagedBlocks.Length)
                    {
                        break;
                    }
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
                DamageDisplay.UpComingDamagedBlocks(DamagedBlocks, new int[] { RedCount, BlueCount, GreenCount, YellowCount }); // Kore Hairetuno suuchi tte zennbu zeroni natta atono degarasi jann!!
            }
            DamageDisplay.UpdateRemainingTime(CurrentDamageRemainingTime);
            DamageDisplay.SetRemainingTimeVisible(true);
        }

        public RecordInfo GetLastRecord()
        {
            return new RecordInfo()
            {
                PlayDate = DateTime.Now,
                Score = Score,
                PlayTime = PlayTime,
                PlacedBlockCount = PlacedBlockCount,
            };
        }

        public void DiscardInstances()
        {
            if (!Field.IsNull())
            {
                foreach (BlockElement b in Field.Grid)
                {
                    Destroy(b.Appearence.gameObject);
                }
            }
            CandidateBlockDisplay.DiscardInstances();
            ScoreDisplay.DiscardInstances();
            BreakedPolyominosDisplay.DiscardInstances();
            AttackChargeDisplay.DiscardInstances();
            DamageDisplay.DiscardInstances();
        }

        public bool IsGameOver()
        {
            return CurrentState == State.GameOver;
        }

        public void GetVirtualCurrent(ref VirtualFieldSystem Destination)
        {
            Destination.FieldSize = this.FieldSize;
            if (Destination.Field.IsNull())
            {
                Destination.Field = new Field2d<BlockColor>(FieldSize);
            }
            for (int y = 0; y < Field.Size.y; y++)
            {
                for (int x = 0; x < Field.Size.x; x++)
                {
                    Vector2Int Pos = new Vector2Int(x, y);
                    Destination.Field[Pos] = Field[Pos].Color;
                }
            }
            Destination.DeadlineHeight = this.DeadlineHeight;
            if (Destination.CurrentErasedShapeFlags == null)
            {
                Destination.CurrentErasedShapeFlags = new bool[this.CurrentErasedShapeFlags.Length];
            }
            Destination.HorizontalPosition = this.HorizontalPosition;
            if(Destination.NextBlockCandidate == null)
            {
                Destination.NextBlockCandidate = new BlockColor[this.NextBlockColors.Length];
            }
            for(int i = 0; i < NextBlockColors.Length; i++)
            {
                Destination.NextBlockCandidate[i] = this.NextBlockColors[i];
            }
            for (int i = 0; i < CurrentErasedShapeFlags.Length; i++)
            {
                Destination.CurrentErasedShapeFlags[i] = this.CurrentErasedShapeFlags[i];
            }
            Destination.IsPureChain = this.IsPureChain;
            Destination.CurrentPureChain = this.CurrentPureChain;
            Destination.CurrentActiveChain = this.CurrentActiveChain;
        }
    }

    public struct VirtualFieldSystem
    {
        public Vector2Int FieldSize;
        public Field2d<BlockColor> Field;
        public int DeadlineHeight;
        public bool[] CurrentErasedShapeFlags;
        public int HorizontalPosition;
        public BlockColor[] NextBlockCandidate;

        public bool IsPureChain;
        public int CurrentPureChain;
        public int CurrentActiveChain;
        public int Score;

        public (int EarnedScore, bool IsChanged) SimulateStep()
        {
            Score = 0;
            bool IsChanged = false;
            IsChanged |= ApplyGravity();
            IsChanged |= CheckBlockBreak();
            return (Score, IsChanged);
        }

        public bool TryPlaceBlock(int HorizontalDelta)
        {
            int Column = Mathf.Clamp(HorizontalPosition + HorizontalDelta, 0, FieldSize.x - 1);
            for(int y = FieldSize.y - 1; y >= 0; y--)
            {
                Vector2Int Pos = new Vector2Int(Column, y);
                if(Field[Pos] != BlockColor.None)
                {
                    return false;
                }
                bool IsPlacable = false;
                if(!Field.IsIn(Pos + Vector2Int.down))
                {
                    IsPlacable = true;
                }
                else if(Field[Pos + Vector2Int.down] != BlockColor.None)
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
                if(IsPlacable)
                {
                    Field[Pos] = NextBlockCandidate[0];
                    HorizontalPosition = Column;
                    AdvanceBlockCandidate();
                    IsPureChain = false;
                    return true;
                }

            }
            return false;
        }

        public void AdvanceBlockCandidate()
        {
            for(int i = 1; i < NextBlockCandidate.Length; i++)
            {
                NextBlockCandidate[i - 1] = NextBlockCandidate[i];
            }
        }

        private Vector2Int[] _Adjacents;
        private Field2d<bool> _CheckedBlocks;
        public bool CheckBlockBreak()
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
            int BreakedPolyominoCount = 0;
            int EarnedScoreLocalTotal = 0;
            for (int y = 0; y < FieldSize.y; y++)
            {
                for (int x = 0; x < FieldSize.x; x++)
                {
                    Vector2Int Pos = new Vector2Int(x, y);
                    BlockColor CheckColor = Field[Pos];
                    if (_CheckedBlocks[Pos])
                    {
                        continue;
                    }
                    if (CheckColor == BlockColor.None)
                    {
                        continue;
                    }
                    int AdjacentCount = SearchAdjacents(Pos);
                    for (int pi = 0; pi < PolyominoDatabase.Tetriminos.Length; pi++)
                    {
                        Vector2Int[] Polyomino = PolyominoDatabase.Tetriminos[pi];
                        //if (DisallowPolyominoDuplication || CurrentErasedShapeFlags[pi])
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
                                Field[v] = BlockColor.None;
                            }
                            if (!IsBreaked)
                            {
                                if (!IsPureChain)
                                {
                                    CurrentPureChain = 0;
                                }
                                CurrentPureChain++;
                                CurrentActiveChain++;
                            }
                            IsBreaked = true;
                            BreakedPolyominoCount++;
                            EarnedScoreLocalTotal += FieldSystem.CalcBlockBreakScore(CheckColor, CurrentPureChain, CurrentActiveChain);
                            MarkPolyomino(pi);
                            break;

                        }
                    }

                }
            }
            if (BreakedPolyominoCount != 0)
            {
                Score += EarnedScoreLocalTotal * BreakedPolyominoCount;
            }
            if (!IsBreaked)
            {
                CurrentPureChain = 0;
                CurrentActiveChain = 0;
            }
            IsPureChain = true;
            return IsBreaked;
        }

        public bool ApplyGravity()
        {
            bool IsChanged = false;
            for (int x = 0; x < FieldSize.x; x++)
            {
                int Bottom = 0;
                for (int y = 0; y < FieldSize.y; y++)
                {
                    Vector2Int Pos = new Vector2Int(x, y);
                    if (Field[Pos] != BlockColor.None)
                    {
                        if (y != Bottom)
                        {
                            Vector2Int BottomPos = new Vector2Int(x, Bottom);
                            BlockColor temp = Field[Pos];
                            Field[Pos] = Field[BottomPos];
                            Field[BottomPos] = temp;
                            IsChanged = true;
                        }
                        Bottom++;
                    }
                }
            }
            return IsChanged;
        }

        private int SearchAdjacents(Vector2Int Origin)
        {
            BlockColor Color = Field[Origin];
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
                    if (Field[CheckPos] != Color)
                    {
                        continue;
                    }
                    bool IsDuplicate(in VirtualFieldSystem _this, Vector2Int Pos, int AdjacentsIndex)
                    {
                        for (int k = 0; k < AdjacentsIndex; k++)
                        {
                            if (_this._Adjacents[k] == Pos)
                            {
                                return true;
                            }
                        }
                        return false;
                    }
                    if (IsDuplicate(this, CheckPos, AdjacentsIndex))
                    {
                        continue;
                    }
                    _Adjacents[AdjacentsIndex] = CheckPos;
                    AdjacentsIndex++;
                }
            }
            return AdjacentsIndex;
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
                    if (Field[Pos] == BlockColor.None)
                    {
                        continue;
                    }
                    int AdjacentCount = SearchAdjacents(Pos);
                    if (AdjacentCount > 4)
                    {
                        for (int i = 0; i < AdjacentCount; i++)
                        {
                            Vector2Int DeletePos = _Adjacents[i];
                            Field[DeletePos] = BlockColor.None;
                        }
                    }
                }
            }
        }

        public void MarkPolyomino(int Index)
        {
            CurrentErasedShapeFlags[Index] = true;
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
            Score += 5;
            RecoverLargeChunks();
        }

        public int EvalFieldCondition()
        {
            int Eval = 0;
            int RemainShapeCount = 0;
            for(int i = 0; i < CurrentErasedShapeFlags.Length;i++)
            {
                if(!CurrentErasedShapeFlags[i])
                {
                    RemainShapeCount++;
                }
            }
            for (int y = 0; y < Field.Size.y; y++)
            {
                for (int x = 0; x < Field.Size.x; x++)
                {
                    Vector2Int Pos = new Vector2Int(x, y);
                    if(_CheckedBlocks[Pos])
                    {
                        continue;
                    }
                    if(Field[Pos] == BlockColor.None)
                    {
                        continue;
                    }
                    if(y >= DeadlineHeight)
                    {
                        Eval += -50000;
                    }
                    int AdjacentCount = SearchAdjacents(Pos);
                    if(AdjacentCount > 4)
                    {
                        Eval += AdjacentCount * -1000 * RemainShapeCount;
                    }
                    else
                    {
                        Eval += AdjacentCount * AdjacentCount;
                    }
                }
            }
            Eval += RemainShapeCount * -50;
            return Eval;
        }
    }

}