using System;
using UnityEngine;

namespace BlockPrinter
{
    [Serializable]
    public struct FieldController
    {
        public enum UseMode
        {
            Player,
            CPU,
        }
        public UseMode Mode;
        public KeyConfig PlayerKeyConfig;
        public CPUProperty CPUConfig;

        public void Initialize(FieldSystem Field)
        {
            if (Mode == UseMode.CPU)
            {
                CPUConfig = CPUProperty.LevelOf(CPUConfig.Prop.Level, Field);
            }
        }


        public FieldControlInput GetInput()
        {
            switch (Mode)
            {
                case UseMode.Player: return PlayerKeyConfig.GetInput();
                case UseMode.CPU: CPUConfig.Tick(Time.deltaTime); return CPUConfig.GetInput();
            }
            return FieldControlInput.Neutral;
        }
    }


    [Serializable]
    public struct FieldControlInput
    {
        public static readonly FieldControlInput Neutral = new FieldControlInput();
        public bool Left, Right;
    }


    [Serializable]
    public struct KeyConfig
    {
        public KeyCode LeftKey;
        public KeyCode RightKey;

        public FieldControlInput GetInput()
        {
            return new FieldControlInput()
            {
                Left = Input.GetKeyDown(LeftKey),
                Right = Input.GetKeyDown(RightKey)
            };
        }
    }
}