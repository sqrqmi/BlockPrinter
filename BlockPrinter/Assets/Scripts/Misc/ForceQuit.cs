using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlockPrinter.Misc
{
    public class ForceQuit : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("TitleScene");
            }
        }
    }
}
