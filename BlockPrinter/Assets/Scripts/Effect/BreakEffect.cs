using UnityEngine;

namespace BlockPrinter
{
    public enum EffectColor
    {
        None,
        Red,
        Orange,
        Yellow,
        Green,
        SkyBlue,
        Blue,
        Purple,
        Gray,
        Brack
    }


    public class BreakEffect : MonoBehaviour
    {
        //Effectの色画像データ(EffectSkin)を入れる
        [SerializeField] private EffectSkin skin;

        //EffectのSpriteRendererを入れる(SerializeField : インスペクター上で表示される設定)
        [SerializeField] private SpriteRenderer sprite;


        public void SetAppearence(EffectColor newColor)
        {
            //新しい画像指定
            Sprite newSprite = null;

            switch (newColor)
            {
                case EffectColor.None: newSprite = this.skin.None; break;
                case EffectColor.Red: newSprite = this.skin.Red; break;
                case EffectColor.Orange: newSprite = this.skin.Orange; break;
                case EffectColor.Yellow: newSprite = this.skin.Yellow; break;
                case EffectColor.Green: newSprite = this.skin.Green; break;
                case EffectColor.SkyBlue: newSprite = this.skin.SkyBlue; break;
                case EffectColor.Blue: newSprite = this.skin.Blue; break;
                case EffectColor.Purple: newSprite = this.skin.Purple; break;
                case EffectColor.Gray: newSprite = this.skin.Gray; break;
                case EffectColor.Brack: newSprite = this.skin.Brack; break;
            }

            //自身の画像を変更する
            this.sprite.sprite = newSprite;
        }

        private void Start()
        {
            SetAppearence(EffectColor.Yellow);
        }
    }
}
