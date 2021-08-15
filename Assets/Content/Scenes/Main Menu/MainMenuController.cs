using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] List<Button> mainMenuButtons = new List<Button>();

    private int buttonIndex = -1;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(mainMenuButtons[0].gameObject);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
