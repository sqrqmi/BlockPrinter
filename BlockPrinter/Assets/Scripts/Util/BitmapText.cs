using System;
using System.Collections.Generic;
using UnityEngine;

namespace Util
{

    public class BitmapTextChar : MonoBehaviour
    {
        protected SpriteRenderer Renderer;

        public virtual void OnCreate(Sprite CharSprite)
        {
            Renderer.sprite = CharSprite;
        }

        public virtual void OnDelete()
        {
            Destroy(this.gameObject);
        }
    }

    public class BitmapText : MonoBehaviour
    {

        [TextArea(3, 5)]
        public string Text;

        public BitmapTextFont Font;

        [Serializable]
        public struct FontFormat
        {
            public float Spacing;
            public float LineHeight;
        }
        public FontFormat Format;
        public Color FontColor = Color.white;

        [Range(0.0f, 1.0f)]
        public float HorizontalAlignment;
        [Range(0.0f, 1.0f)]
        public float VerticalAlignment;

        public bool IsDisplayOnStart;

        private List<SpriteRenderer> Chars;
        //private List<BitmapTextChar> Chars;

        public void Start()
        {
            if(IsDisplayOnStart)
            {
                UpdateText();
            }
        }

        public void AppendText(string t)
        {

        }

        public void UpdateText()
        {
            if(Chars != null)
            {
                foreach(SpriteRenderer c in Chars)
                //foreach(BitmapTextChar c in Chars)
                {
                    Destroy(c.gameObject);
                    //c.OnDelete();
                }
                Chars.Clear();
            }
            else
            {
                Chars = new List<SpriteRenderer>();
                //Chars = new List<BitmapTextChar>();
            }
            Vector3 Size = Vector3.zero;
            Vector3 CurrentCarriage = Vector3.zero;
            foreach(char c in Text)
            {
                if(c == '\n')
                {
                    CurrentCarriage = new Vector3(0.0f, CurrentCarriage.y - Format.LineHeight);
                    Size += Format.LineHeight * Vector3.down;
                    continue;
                }
                SpriteRenderer NewRend = new GameObject().AddComponent<SpriteRenderer>();
                NewRend.transform.SetParent(this.transform);
                Sprite CharSpr = Font.GetCharacter(c);
                NewRend.transform.localPosition = (CharSpr.rect.width / CharSpr.pixelsPerUnit / 2.0f) * Vector3.right + CurrentCarriage;
                NewRend.sprite = CharSpr;
                NewRend.color = FontColor;
                CurrentCarriage += (CharSpr.rect.width / CharSpr.pixelsPerUnit + Format.Spacing) * Vector3.right;
                Chars.Add(NewRend);
                Size = new Vector3(Mathf.Max(Size.x, CurrentCarriage.x), Size.y);
            }
            Vector3 Offset = Vector3.Scale(Size, new Vector3(-HorizontalAlignment, -VerticalAlignment));
            foreach(SpriteRenderer r in Chars)
            {
                r.transform.localPosition += Offset;
            }
        }

        public void UpdateText(string t)
        {
            Text = t;
            UpdateText();
        }

        public void SetColor(Color NewColor)
        {
            FontColor = NewColor;
            if(Chars == null)
            {
                return;
            }
            foreach(SpriteRenderer r in Chars)
            {
                if(r == null)
                {
                    continue;
                }
                r.color = FontColor;
            }
        }
    }
}
