﻿using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelManager : MonoBehaviour {

	public static LevelManager levelManager;

	public float autoLoadNextLevelAfter;
	
	void Start () {
		levelManager = GetComponent<LevelManager>();
        if (SceneManager.GetActiveScene().name == "_Splash") {
            Invoke("LoadNextLevel", 3f);
        }
	}

	public void LoadLevel (string name) {
		SceneManager.LoadScene (name);
	}

	public void QuitRequest () {
		Application.Quit ();
	}
	
	public void LevelBack () {
		SceneManager.LoadScene (PlayerPrefsManager.GetDeleteEntryPoint());
	}

    public void LoadNextLevel() {
        int sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(sceneBuildIndex + 1);
    }
}

