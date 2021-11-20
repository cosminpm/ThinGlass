using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void ButtonChangeScene(string scene)
    {
        StartCoroutine(DelaySceneLoad(scene));
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
    IEnumerator DelaySceneLoad(string scene)
    {
        
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(scene);
    }


}
