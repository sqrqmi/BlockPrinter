using System.Collections;
using UnityEngine;

namespace BlockPrinter
{
    public class TransitionEnter : MonoBehaviour
    {
        private struct AnimationBlock
        {
            public BlockAppearence Block;
            public bool IsDone;
        };

        private const int BG_X = 32;
        private const int BG_Y = 18;

        [SerializeField] private BlockAppearence BlockPrefab;

        [SerializeField] private float TotalAnimationTime = 0.5f;
        private float IntervalTime;
        AnimationBlock[] Blocks;


        public void StartAnimation()
        {
            GenerateAnimationBlock();
            StartCoroutine(PlayAnimation());
        }

        public bool IsDone()
        {
            int index = 0;
            while( index < Blocks.Length)
            {
                if(this.Blocks[index].IsDone == false )
                {
                    return false;
                }
                index++;
            }
            return true;
        }

        private IEnumerator PlayAnimation()
        {
            float cumulativeTime = 0f;
            for( int x = 0; x < BG_X;  x++ )
            {
                for( int y = 0; y < BG_Y; y++ )
                {
                    bool IsDone = false;
                    while( !IsDone )
                    {
                        int bx = Random.Range(0, BG_X);
                        int index = y * BG_X + bx;
                        if(this.Blocks[index].IsDone == false)
                        {
                            StartCoroutine(LightBlock(this.Blocks[index]));
                            this.Blocks[index].IsDone = true;
                            IsDone = true;
                            cumulativeTime += this.IntervalTime;
                            if (cumulativeTime > Time.deltaTime)
                            {
                                yield return new WaitForSeconds(this.IntervalTime);
                                cumulativeTime = 0f;
                            }
                        }
                    }
                }
            }
        }

        private IEnumerator LightBlock(AnimationBlock block)
        {
            block.Block.SetAlpha(0f, 1f, this.IntervalTime);
            yield return null;
        }

        private void GenerateAnimationBlock()
        {
            this.IntervalTime = this.TotalAnimationTime / (BG_X * BG_Y);

            this.Blocks = new AnimationBlock[BG_X * BG_Y];

            Camera cam = Camera.main;
            Vector3 topLeft = cam.ViewportToWorldPoint(new Vector3(0, 1, cam.nearClipPlane));
            Vector3 bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane));
            BlockAppearence[] blocks = new BlockAppearence[BG_X * BG_Y];

            for (int y = 0; y < BG_Y; y++)
            {
                for (int x = 0; x < BG_X; x++)
                {
                    Vector3 end = topLeft;
                    float scale = Mathf.Abs(topLeft.y - bottomRight.y) / BG_Y;
                    end.x += x * scale + (scale / 2f);
                    end.y += y * -scale - (scale / 2f);

                    int index = y * BG_X + x;
                    this.Blocks[index].Block = Instantiate(BlockPrefab, this.transform);
                    this.Blocks[index].Block.Initialize(Vector3.zero, BlockColor.None);
                    this.Blocks[index].IsDone = false;
                    this.Blocks[index].Block.Fade(0f);
                    this.Blocks[index].Block.SetAppearence((BlockColor)(Random.Range(1, 5)));
                    this.Blocks[index].Block.SetScale(scale);
                    this.Blocks[index].Block.OnMove(Vector3.zero, end, 0f);
                }
            }
        }
    }
}
