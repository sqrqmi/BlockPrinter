using UnityEngine;

namespace BlockPrinter
{
    [CreateAssetMenu(menuName = "BlockSkin_Scriptable")]
    public class BlockSkin : ScriptableObject
    {
        public Sprite None;
        public Sprite Red;
        public Sprite Blue;
        public Sprite Green;
        public Sprite Yellow;
    }
}
