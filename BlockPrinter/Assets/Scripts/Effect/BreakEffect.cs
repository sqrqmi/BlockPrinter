using UnityEngine;
using Util;

namespace BlockPrinter.Effect
{
    public class BreakEffect : MonoBehaviour
    {
        [SerializeField] GameObject Effect;

        private GameObject[] Effects;

        //エフェクトの発射する数
        [SerializeField] private int DirectionCount = 8;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            this.Effects = new GameObject[DirectionCount];

            for( int i = 0; i < this.DirectionCount; i++ )
            {
                this.Effects[i] = Instantiate(this.Effect);
                this.Effects[i].transform.SetParent( this.transform );

                float angle = 360f / (float)this.DirectionCount * i;
                Vector3 toPos = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0f);

                Vector3 rotation = new Vector3(0f, 0f, angle - 90f);

                this.Effects[i].transform.rotation = Quaternion.Euler( rotation );
                this.Effects[i].GetComponent<LinearMovement>().To = toPos;

                Debug.Log($"[ Effect {i} ] angle : {angle} / toPos : {toPos} / rotation : {rotation} ");
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
