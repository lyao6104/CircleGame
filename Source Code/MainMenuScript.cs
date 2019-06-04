/* Name: Larry Y.
 * Date: May 27, 2019
 * Desc: Functions for the main menu. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
	public string[] mapList;

	public void StartGame()
	{
		SceneManager.LoadScene(mapList[0]);
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
