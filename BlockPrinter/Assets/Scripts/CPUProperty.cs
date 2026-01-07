using System;
using Util;

namespace BlockPrinter
{
    [Serializable]
    public struct CPUProperty
    {
        public FieldSystem HandlingField;

        public struct Properties
        {
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

        private Field2d<BlockColor>[] FieldBranches;

        private enum ControlPettern
        {
            Wait,
            Left,
            Right,
        }
        private ControlPettern[] ControlSequence;
        private int CurrentProcessSequenceIndex;

        public static CPUProperty LevelOf(int Level, FieldSystem System)
        {
            CPUProperty Result = new CPUProperty();
            Result.HandlingField = System;

            Result.Prop = new Properties()
            {
                MaxDepth = 1,
                MaxChainCheck = 1,
                ErrorFrequency = 0.0f,
                ThinkingTime = 1.0f,
                BaseControlSpan = 1.0f,
                HispeedControlSpan = 0.2f,
            };
            Result.WaitTime = 0.0f;
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
                        HandlingField.GetBlockField(ref FieldBranches[0]);
                        EvalDynamic(0);
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

        private int EvalStatic(ref Field2d<BlockColor> Field)
        {
            for(int i = 0; i < Prop.MaxChainCheck; i++)
            {

            }
            return 0;
        }
        private int EvalDynamic(int CurrentDepth)
        {
            if(CurrentDepth >= Prop.MaxDepth)
            {
                return EvalStatic(ref FieldBranches[CurrentDepth - 1]);
            }
            
            return 0;
        }
    }


}