using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlockPrinter.UserInterface
{
    public class SceneTransitionManager : MonoBehaviour
    {



        private IEnumerator OnLoadScene(string SceneName)
        {
            DontDestroyOnLoad(this.gameObject);
            {
                SceneTransitionEnter EnterAnimation = GameObject.Find("EnterAnimation").GetComponent<SceneTransitionEnter>();
                if (EnterAnimation != null)
                {
                    EnterAnimation.StartAnimation();
                    yield return new WaitUntil(EnterAnimation.IsDone);
                }
            }
            SceneManager.LoadScene(SceneName);
            yield return null;
            {
                SceneTransitionExit ExitAnimation = GameObject.Find("ExitAnimation").GetComponent<SceneTransitionExit>();
                if (ExitAnimation != null)
                {
                    ExitAnimation.StartAnimation();
                    yield return new WaitUntil(ExitAnimation.IsDone);
                }
            }
            Destroy(this.gameObject);
            yield break;
        }

        public static SceneTransitionManager LoadScene(string SceneName)
        {
            SceneTransitionManager NewManager = new GameObject().AddComponent<SceneTransitionManager>();
            NewManager.StartCoroutine(NewManager.OnLoadScene(SceneName));
            return NewManager;
        }
    }
}
