using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public Button[] button;

	
	void Update () {
		
	}
    public void ScaleUpButton(int i)
    {
        button[i].transform.localScale = new Vector3(1.2f, 1.2f, 1);
    }
    public void ScaleDownButton(int i)
    {
        button[i].transform.localScale = new Vector3(1, 1, 1);
    }
    public void EnterOfflineMatch(string sceneName)
    {                     
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }
}
