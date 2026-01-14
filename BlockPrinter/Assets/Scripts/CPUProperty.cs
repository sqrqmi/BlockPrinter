using System;
using System.Collections;
using System.Collections.Generic;
using Util;
using UnityEngine;

namespace BlockPrinter
{
    [Serializable]
    public struct CPUCharacteristics
    {
        public int Level;
        public int MaxDepth;
        public int MaxChainCheck;
        public float ErrorFrequency;
        public float ThinkingTime;
        public float BaseControlSpan;
        public float HispeedControlSpan;
    }
    [Serializable]
    public struct CPUProperty
    {
        private const int InvalidEval = -30000;

        public FieldSystem HandlingField;


        public CPUCharacteristics Prop;
        private float WaitTime;
        private enum State
        {
            Thinking,
            ProcessingSequence,
        }
        private State CurrentState;
        private VirtualFieldSystem VirtualField;

        private enum ControlPettern
        {
            Wait,
            Left,
            Right,

            MaxCount
        }
        private ControlPettern[] TemporalSequence;
        private ControlPettern[] ControlSequence;
        private int BestSequenceEval;
        private int CurrentProcessSequenceIndex;
        private int ContinouslyWaitCount;
        public static CPUProperty LevelOf(int Level, FieldSystem System)
        {
            CPUProperty Result = new CPUProperty();
            Result.HandlingField = System;

            Result.Prop = new CPUCharacteristics()
            {
                Level = Level,
                MaxDepth = Level,
                MaxChainCheck = Level,
                ErrorFrequency = 0.0f,
                ThinkingTime = 1.6f,
                BaseControlSpan = 0.3f,
                HispeedControlSpan = 0.1f,
            };
            Result.WaitTime = 0.0f;
            Result.CurrentState = State.Thinking;
            Result.TemporalSequence = new ControlPettern[Result.Prop.MaxDepth];
            Result.ControlSequence = new ControlPettern[Result.Prop.MaxDepth];
            Result.CurrentProcessSequenceIndex = 0;
            Result.ContinouslyWaitCount = 0;
            return Result;
        }

        public void Tick(float DeltaTime)
        {
            WaitTime -= DeltaTime;
        }

        public FieldControlInput GetInput()
        {
            if(WaitTime > 0.0f)
            {
                return FieldControlInput.Neutral;
            }
            FieldControlInput Result = new FieldControlInput();
            switch(CurrentState)
            {
                case State.Thinking:
                {
                    BestSequenceEval = InvalidEval;
                    for(int i = 0; i < TemporalSequence.Length; i++)
                    {
                        TemporalSequence[i] = ControlPettern.Wait;
                    }
                    if(ContinouslyWaitCount > 20)
                    {
                        ControlSequence[0] = UnityEngine.Random.value < 0.5f ? ControlPettern.Left : ControlPettern.Right;
                        Debug.Log("CPU Update: Stuck Forcely Remover Activated.");
                    }
                    else
                    {

                        EvalDynamic(0);
                        {
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                            sb.Append("CPU Update: ");
                            for(int i = 0; i < ControlSequence.Length; i++)
                            {
                                sb.Append((char)((int)ControlSequence[i] + 'A'));
                            }
                            sb.Append(", Eval: ");
                            sb.Append(BestSequenceEval.ToString());
                            Debug.Log(sb.ToString());
                        }
                    }
                    CurrentProcessSequenceIndex = 0;
                    CurrentState = State.ProcessingSequence;
                }
                return FieldControlInput.Neutral;

                case State.ProcessingSequence:
                {
                    WaitTime += Prop.BaseControlSpan;
                    switch(ControlSequence[CurrentProcessSequenceIndex])
                    {
                        case ControlPettern.Wait:
                            ContinouslyWaitCount++;
                            CurrentState = State.Thinking;
                            return FieldControlInput.Neutral;

                        case ControlPettern.Left: ContinouslyWaitCount = 0; Result.Left = true; break;
                        case ControlPettern.Right: ContinouslyWaitCount = 0; Result.Right = true; break;
                    }
                    CurrentProcessSequenceIndex++;
                    if(CurrentProcessSequenceIndex >= ControlSequence.Length)
                    {
                        WaitTime += Prop.ThinkingTime;
                        CurrentState = State.Thinking;
                    }

                }
                break;
            }
            return Result;
        }

        private int EvalStatic()
        {
            int Eval = 0;
            int EstimatedScore = 0;
            HandlingField.GetVirtualCurrent(ref VirtualField);
            for(int i = 0; i < TemporalSequence.Length; i++)
            {
                switch(TemporalSequence[i])
                {
                    case ControlPettern.Wait:
                        Eval += ContinouslyWaitCount * -500;
                        break;
                    case ControlPettern.Left:
                        if(!VirtualField.TryPlaceBlock(-1))
                        {
                            return InvalidEval;
                        }
                        break;
                    case ControlPettern.Right:
                        if(!VirtualField.TryPlaceBlock(+1))
                        {
                            return InvalidEval;
                        }
                        break;
                }
            }


            for(int i = 0; i < Prop.MaxChainCheck; i++)
            {
                (int EarnedScore, bool IsChanged) = VirtualField.SimulateStep();
                EstimatedScore += EarnedScore;
                if(!IsChanged)
                {
                    break;
                }
            }
            Eval += EstimatedScore * 100;
            Eval += VirtualField.EvalFieldCondition();
            return Eval;
        }
        private int EvalDynamic(int CurrentDepth)
        {
            if(CurrentDepth >= Prop.MaxDepth)
            {
                return EvalStatic();
            }
            int LocalMaxEval = InvalidEval;
            for(int i = 0; i < (int)ControlPettern.MaxCount; i++)
            {
                TemporalSequence[CurrentDepth] = (ControlPettern)i;
                int Eval;
                if(i == (int)ControlPettern.Wait)
                {
                    Eval = EvalStatic();
                }
                else
                {
                    Eval = EvalDynamic(CurrentDepth + 1);
                }
                if(Eval > BestSequenceEval)
                {
                    Array.Copy(TemporalSequence, ControlSequence, ControlSequence.Length);
                    BestSequenceEval = Eval;
                }
                LocalMaxEval = Mathf.Max(Eval, LocalMaxEval);
            }
            TemporalSequence[CurrentDepth] = ControlPettern.Wait;
            return LocalMaxEval;
        }
    }


}