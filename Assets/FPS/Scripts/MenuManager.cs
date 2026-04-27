using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    private TMP_Dropdown dropdown;
    private UnityEngine.UI.Button start;
    private UnityEngine.UI.Button credits;
    private UnityEngine.UI.Button exit;
    private GameObject titleScreen;
    private GameObject creditsScreen;
    private bool showingCredits = false;

    void Start()
    {
        Time.timeScale = 1f;
        titleScreen = menu.transform.Find("TitleScreen").gameObject;
        creditsScreen = menu.transform.Find("CreditsScreen").gameObject;

        GameObject buttons = titleScreen.transform.Find("Buttons").gameObject;

        dropdown = buttons.transform.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>();
        start = buttons.transform.Find("Start").gameObject.GetComponent<UnityEngine.UI.Button>();
        credits = buttons.transform.Find("Credits").gameObject.GetComponent<UnityEngine.UI.Button>();
        exit = buttons.transform.Find("Exit").gameObject.GetComponent<UnityEngine.UI.Button>();

        start.onClick.AddListener(HandleStart);
        credits.onClick.AddListener(HandleCredits);
        exit.onClick.AddListener(HandleExit);        
        Debug.Log("Setup menu");
    }

    public void HandleStart()
    {
        GameState.Instance.gamemode = dropdown.value;
        SceneManager.LoadScene("Game");
    }

    public void HandleCredits()
    {
        showingCredits = true;
        titleScreen.SetActive(false);
        creditsScreen.SetActive(true);
    }

    public void HandleExit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public void OnMenu()
    {
        if (showingCredits)
        {
            showingCredits = false;
            titleScreen.SetActive(true);
            creditsScreen.SetActive(false);
        }
    }
}
