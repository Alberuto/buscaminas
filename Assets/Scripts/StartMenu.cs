using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour {

    [Header("Configuración tablero")]
    [SerializeField] private TMP_InputField widthInput;
    [SerializeField] private TMP_InputField heightInput;
    [SerializeField] private TMP_InputField bombsInput;

    [Header("Marcador")]
    [SerializeField] private TextMeshProUGUI humanWinsText;
    [SerializeField] private TextMeshProUGUI aiWinsText;

    [Header("Referencias")]
    [SerializeField] private GameObject startMenuPanel;
    [SerializeField] private GameObject endMenuPanel;

    public static StartMenu instance;

    private int humanWins = 0;
    private int aiWins = 0;

    private void Awake() {

        instance = this;
        ShowStartMenu();
        UpdateScoreUI();
    }
    public void ShowStartMenu() {

        startMenuPanel.SetActive(true);
        Debug.Log("Mostrando menu");
    }
    public void HideStartMenu() {

        startMenuPanel.SetActive(false);
        Debug.Log("Ocultando menu");
    }
    public void ShowEndMenu() {

        endMenuPanel.SetActive(true);
        Debug.Log("Mostrando menu");
    }
    public void HideEndMenu() {

        endMenuPanel.SetActive(false);
        Debug.Log("Ocultando menu de final partida");
    }
    public void OnStartButtonPressed() {

        Debug.Log("Bot�n Start Game presionado");

        int width = int.Parse(widthInput.text);
        int height = int.Parse(heightInput.text);
        int bombs = int.Parse(bombsInput.text);

        Generator.gen.SetWidth(width);
        Generator.gen.SetHeight(height);
        Generator.gen.SetBombs(bombs);

        if (Generator.gen.Validate() != 0) {

            Debug.LogError("Valores del tablero inválidos");
            return;
        }
        HideStartMenu();
        GameManager.instance.GameStart();
    }
    public void AddWin(bool humanWon) {

        Debug.Log("victoria humana? " + humanWon);
        if (humanWon) humanWins++;
        else aiWins++;
        UpdateScoreUI();
    }
    private void UpdateScoreUI() {

        humanWinsText.text = "Jugador: " + humanWins;
        aiWinsText.text = "Bot: " + aiWins;
    }
}