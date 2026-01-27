using UnityEngine;

namespace BlockPrinter.Misc
{
    public class ForceReturnToTitle : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.F1))
            {
                UserInterface.SceneTransitionManager.LoadScene("TitleScene");
            }
        }
    }
}
