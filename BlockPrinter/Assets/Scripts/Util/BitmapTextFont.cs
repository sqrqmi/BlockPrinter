using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{
    [CreateAssetMenu(menuName = "Util/BitmapTextFont")]
    public class BitmapTextFont : ScriptableObject
    {
        public Sprite[] Chars;

        public Sprite GetCharacter(char c) => Chars[(int)c];
    }

}
