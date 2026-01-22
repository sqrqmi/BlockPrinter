using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using Unity.Collections;
using UnityEngine;

namespace BlockPrinter
{
    public class BackGround : MonoBehaviour
    {
        const int BG_X = 32;
        const int BG_Y = 18;

        [SerializeField] BGSkin Skin;
        [SerializeField] BlockAppearence BlockPrefab;

        private bool flag = false;

        private void Start()
        {
            Camera cam = Camera.main;
            Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.nearClipPlane));
            Vector3 bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
            BlockAppearence[] blocks = new BlockAppearence[BG_X * BG_Y];

            for( int y = 0; y < BG_Y; y++ )
            {
                for (int x = 0; x < BG_X; x++)
                {
                    //まだ生成されていなかったら（大きなブロックが選ばれたときに既に生成されている可能性あり）
                    if (blocks[y * BG_X + x] == null)
                    {
                        Vector3 end = topLeft;
                        float scale = Mathf.Abs(topLeft.y - bottomRight.y) / BG_Y;
                        end.x += x * scale + (scale / 2f);
                        end.y += y * -scale - (scale / 2f);

                        int spriteNum = Random.Range(0, 10);

                        //大きなブロックが選ばれたら
                        if( spriteNum < 5 )
                        {
                            //範囲外チェック
                            if (y + 1 < BG_Y && x + 1 < BG_X)
                            {
                                int geneCnt = -1;
                                //現在の位置から2*2マス分を埋める
                                for (int y2 = 0; y2 < 2; y2++)
                                {
                                    //フラグが立っていたらループを抜ける
                                    if( flag ) { break; }
                                    for (int x2 = 0; x2 < 2; x2++)
                                    {
                                        //まだ生成されて居なかったら
                                        if(blocks[(y + y2) * BG_X + x + x2] == null)
                                        {
                                            //画像を透明に設定して生成
                                            blocks[(y + y2) * BG_X + x + x2] = Instantiate(BlockPrefab, this.transform);
                                            blocks[(y + y2) * BG_X + x + x2].SetSprite(GetSprite(-1));
                                            geneCnt++;
                                        }
                                        //既に生成されていた部分があったらここまでの生成をキャンセルする
                                        else
                                        {
                                            while(geneCnt > 0)
                                            {
                                                Destroy(blocks[(y + geneCnt / 2) * BG_X + x + geneCnt % 2].gameObject);
                                                geneCnt--;
                                            }
                                            //小さいブロックに変更
                                            spriteNum += 5;
                                            //フラグを立ててループを抜ける
                                            flag = true;
                                        }
                                    }
                                }
                                //フラグが立っていたら元に戻す
                                if (flag) { flag = false; }
                                //大きなブロック用に位置を調整
                                else
                                {
                                    end.x += scale / 2f;
                                    end.y -= scale / 2f;
                                }
                            }
                            //大きなブロックで生成したら範囲外に出てしまう場合は小さいブロックに変更する
                            else
                            {
                                //小さいブロックに変更
                                spriteNum += 5;
                                blocks[y * BG_X + x] = Instantiate(BlockPrefab, this.transform);
                            }
                        }
                        //小さいブロックが選ばれたら
                        else
                        {
                            blocks[y * BG_X + x] = Instantiate(BlockPrefab, this.transform);
                        }
                        //透明度を設定
                        blocks[y * BG_X + x].Fade(0.1f);
                        //画像を直接指定
                        blocks[y * BG_X + x].SetSprite(GetSprite(spriteNum));
                        //サイズを変更
                        blocks[y * BG_X + x].SetScale(scale * 2f);
                        //画像の位置を変更
                        blocks[y * BG_X + x].OnMove(Vector3.zero, end, 0f);
                    }
                }
            }
        }

        private Sprite GetSprite(int value)
        {
            switch (value)
            {
                case 0: return this.Skin.White;
                case 1: return this.Skin.Red;
                case 2: return this.Skin.Blue;
                case 3: return this.Skin.Green;
                case 4: return this.Skin.Yellow;
                case 5: return this.Skin.sWhite;
                case 6: return this.Skin.sRed;
                case 7: return this.Skin.sBlue;
                case 8: return this.Skin.sGreen;
                case 9: return this.Skin.sYellow;
                default: return this.Skin.None;
            }
        }
    }
}
