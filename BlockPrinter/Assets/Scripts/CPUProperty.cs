using System;
using System.Collections;
using System.Collections.Generic;
using Util;

namespace BlockPrinter
{
    [Serializable]
    public struct CPUProperty
    {
        public FieldSystem HandlingField;

        [Serializable]
        public struct Properties
        {
            public int Level;
            public int MaxDepth;
            public int MaxChainCheck;
            public float ErrorFrequency;
            public float ThinkingTime;
            public float BaseControlSpan;
            public float HispeedControlSpan;
        }
        public Properties Prop;
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

        public static CPUProperty LevelOf(int Level, FieldSystem System)
        {
            CPUProperty Result = new CPUProperty();
            Result.HandlingField = System;

            Result.Prop = new Properties()
            {
                Level = Level,
                MaxDepth = Level,
                MaxChainCheck = Level,
                ErrorFrequency = 0.0f,
                ThinkingTime = 1.0f,
                BaseControlSpan = 0.7f,
                HispeedControlSpan = 0.2f,
            };
            Result.WaitTime = 0.0f;
            Result.CurrentState = State.Thinking;
            Result.TemporalSequence = new ControlPettern[Result.Prop.MaxDepth];
            Result.ControlSequence = new ControlPettern[Result.Prop.MaxDepth];
            Result.CurrentProcessSequenceIndex = 0;
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
            switch (CurrentState)
            {
                case State.Thinking:
                    {
                        HandlingField.GetVirtualCurrent(ref VirtualField);
                        BestSequenceEval = -30000;
                        EvalDynamic(0);
                        CurrentProcessSequenceIndex = 0;
                        WaitTime += Prop.ThinkingTime;
                    }
                    return FieldControlInput.Neutral;

                    case State.ProcessingSequence:
                    {
                        WaitTime += Prop.BaseControlSpan;
                        switch (ControlSequence[CurrentProcessSequenceIndex])
                        {
                            case ControlPettern.Wait:
                                CurrentState = State.Thinking;
                                return FieldControlInput.Neutral;

                            case ControlPettern.Left:
                                return new FieldControlInput() { Left = true };
                            case ControlPettern.Right:
                                return new FieldControlInput() { Right = true };
                        }

                    }
                    break;
            }
            return FieldControlInput.Neutral;
        }

        private int EvalStatic()
        {
            int Eval = 0;
            int EstimatedScore = 0;
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
            for(int i = 0; i < (int)ControlPettern.MaxCount; i++)
            {
                TemporalSequence[CurrentDepth] = (ControlPettern)i;
                int Eval;
                if (i == (int)ControlPettern.Wait)
                {
                    Eval = EvalStatic();
                }
                else
                {
                    Eval = EvalDynamic(CurrentDepth + 1);
                }
                if (Eval > BestSequenceEval)
                {
                    Array.Copy(TemporalSequence, ControlSequence, ControlSequence.Length);
                    BestSequenceEval = Eval;
                }
            }
            return 0;
        }
    }


}