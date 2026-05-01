using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject menu;

    private TMP_Dropdown dropdown;
    private UnityEngine.UI.Button start;
    private UnityEngine.UI.Button credits;
    private UnityEngine.UI.Button exit;
    private GameObject titleScreen;
    private GameObject creditsScreen;
    private GameObject optionsScreen;
    private bool showingCredits = false;

    void Start()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
        titleScreen = menu.transform.Find("TitleScreen").gameObject;
        creditsScreen = menu.transform.Find("CreditsScreen").gameObject;
        optionsScreen = menu.transform.Find("Options").gameObject;
        optionsScreen.transform.Find("Back").gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(OnMenu);
        UnityEngine.UI.Slider sensitivitySlider = optionsScreen.transform.Find("Sensitivity").gameObject.GetComponent<UnityEngine.UI.Slider>();
        sensitivitySlider.onValueChanged.AddListener(GameState.Instance.HandleSensitivity);
        sensitivitySlider.value = Mathf.Sqrt(GameState.Instance.mouseSensitivity);

        UnityEngine.UI.Slider volumeSlider = optionsScreen.transform.Find("Volume").gameObject.GetComponent<UnityEngine.UI.Slider>();
        volumeSlider.onValueChanged.AddListener(GameState.Instance.HandleVolume);
        volumeSlider.value = Mathf.Sqrt(GameState.Instance.gameVolume);

        GameObject buttons = titleScreen.transform.Find("Buttons").gameObject;

        dropdown = buttons.transform.Find("Dropdown").gameObject.GetComponent<TMP_Dropdown>();
        start = buttons.transform.Find("Start").gameObject.GetComponent<UnityEngine.UI.Button>();
        credits = buttons.transform.Find("Credits").gameObject.GetComponent<UnityEngine.UI.Button>();
        exit = buttons.transform.Find("Exit").gameObject.GetComponent<UnityEngine.UI.Button>();
        buttons.transform.Find("Options").gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => ChangeScreen(2));

        start.onClick.AddListener(HandleStart);
        credits.onClick.AddListener(HandleCredits);
        exit.onClick.AddListener(HandleExit);        
    }

    private void ChangeScreen(int index)
    {
        switch (index)
        {
            case 0:
                titleScreen.SetActive(true);
                creditsScreen.SetActive(false);
                optionsScreen.SetActive(false);
                break;
            case 1:
                titleScreen.SetActive(false);
                creditsScreen.SetActive(true);
                optionsScreen.SetActive(false);
                break;
            case 2:
                titleScreen.SetActive(false);
                creditsScreen.SetActive(false);
                optionsScreen.SetActive(true);
                break;
        }
    }

    public void HandleStart()
    {
        GameState.Instance.gamemode = dropdown.value;
        SceneManager.LoadScene("Game");
    }

    public void HandleCredits()
    {
        showingCredits = true;
        ChangeScreen(1);
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
        showingCredits = false;
        ChangeScreen(0);
    }
}
