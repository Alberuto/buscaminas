using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour{

    [SerializeField] GameObject startMenu;
    [SerializeField] public GameObject endMenu;

    public bool endGame;
    public static GameManager instance;
    public int flagsRemaining, bombsFlaggedCorrectly=0;

    public int humanWins = 0;
    public int aiWins = 0;
    public bool isHumanTurn = true; // true = humano, false = IA

    public void EndGame(bool humanWon) {

        endGame = true;
        if (humanWon) {

            humanWins++;
        }
        else {

            aiWins++;
        }
        // Muestra el marcador y reinicia el juego
        endMenu.SetActive(true);
        Transform victoria = endMenu.transform.Find("Victoria");
        Transform derrota = endMenu.transform.Find("Derrota");
        victoria.gameObject.SetActive(humanWon);
        derrota.gameObject.SetActive(!humanWon);
    }
    public void SwitchTurn() {

        isHumanTurn = !isHumanTurn;
        if (!isHumanTurn) {

            // Activa la IA
            AIController ai = FindObjectOfType<AIController>();
            if (ai != null) {

                ai.enabled = true;
            }
        }
    }
    public void HumanAction() {

        if (isHumanTurn && !endGame) {

            // Aquí va la lógica de acción del humano
            // Cuando el humano hace una jugada válida, llama a SwitchTurn()
            SwitchTurn();
        }
    }

    public void AIAction() {
        if (!isHumanTurn && !endGame) {
            // Aquí va la lógica de acción de la IA
            // Cuando la IA hace una jugada válida, llama a SwitchTurn()
            SwitchTurn();
        }
    }
    private void Awake(){

        if (instance == null){

            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this) { 
            
            Destroy(gameObject);
        }
    }
    public void Start() {

        DontDestroyOnLoad (gameObject); 
        startMenu.SetActive(true);
        endMenu.SetActive(false);
        endGame = false;
        bombsFlaggedCorrectly = 0;
    }
    public void GameStart() {

        if (GameManager.instance == null) {

            Debug.LogError("GameManager.instance no está inicializado.");
            return;
        }
       
        Generator.gen.setWidth(int.Parse(StartMenu.instance.width.GetComponentInChildren<TMP_InputField>().text.ToString()));
        Generator.gen.setHeight(int.Parse(StartMenu.instance.height.GetComponentInChildren<TMP_InputField>().text.ToString()));
        Generator.gen.setBombs(int.Parse(StartMenu.instance.bombs.GetComponentInChildren<TMP_InputField>().text.ToString()));

        flagsRemaining = (int.Parse(StartMenu.instance.bombs.GetComponentInChildren<TMP_InputField>().text.ToString()));

        if (Generator.gen.Validate() == 0) {

            Generator.gen.Generate();
            startMenu.SetActive(false);

            // Configurar la IA
            AIController ai = FindObjectOfType<AIController>();

            if (ai != null) {

                if (ai.enabled) ai.RestartAI();
            }
            else {

                Debug.Log("Error en los parámetros del juego.");//canvas error
            }
        }
    }
    public void FlagPlaced(bool isBomb){

        if (isBomb) bombsFlaggedCorrectly++;
        flagsRemaining--;
        CheckVictory();
    }
    public void FlagRemoved(bool isBomb){

        if (isBomb) bombsFlaggedCorrectly--;
        flagsRemaining++;
    }
    public void CheckVictory(){

        if (bombsFlaggedCorrectly == Generator.gen.bombsNumber && flagsRemaining == 0){

            endGame = true;
            endMenu.SetActive(true);
            Transform victoria = endMenu.transform.Find("Victoria");
            Transform derrota = endMenu.transform.Find("Derrota");
            victoria.gameObject.SetActive(true);
            derrota.gameObject.SetActive(false);
        }
    }
    public int flags() {

        return flagsRemaining;
    }
    public void ReiniciarJuego(){

        if (Generator.gen.map != null) {

            Generator.gen.DestroyMap();
        }
        Start();
        // Recarga la escena actual al estado inicial
        // Reinicia la IA (agrega esto)
        var ai = FindObjectOfType<AIController>();
        if (ai != null){

            if (ai.turnTime > 0f)
                ai.RestartAI();
            else
                ai.enabled = false;
        }
    }
    public void CheckVictoryByClear(){

        int safePieces = 0;

        for (int i = 0; i < Generator.gen.width; i++) {

            for (int j = 0; j < Generator.gen.height; j++) {

                Piece p = Generator.gen.map[i][j].GetComponent<Piece>();

                if (!p.isBomb() && p.isCheck()) {

                    safePieces++;
                }
            }
        }
        int totalSafe = Generator.gen.width * Generator.gen.height - Generator.gen.bombsNumber;
        if (safePieces == totalSafe){

            endGame = true;
            endMenu.SetActive(true);
            Transform victoria = endMenu.transform.Find("Victoria");
            Transform derrota = endMenu.transform.Find("Derrota");
            victoria.gameObject.SetActive(true);
            derrota.gameObject.SetActive(false);
        }
    }
}