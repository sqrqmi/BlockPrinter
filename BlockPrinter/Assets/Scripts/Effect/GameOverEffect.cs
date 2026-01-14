using UnityEngine;

namespace BlockPrinter.Effect
{
    public class GameOverEffect : MonoBehaviour
    {
        [SerializeField] GameObject Explosion;

        private GameObject Exp;

        public void Initialize()
        {

        }

        public void OnGameOver()
        {
            this.Exp = Instantiate(Explosion);
            this.Exp.transform.SetParent(this.transform);
            this.Exp.transform.localPosition = Vector3.zero;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
