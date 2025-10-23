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

        Generator.gen.setWidth(int.Parse(StartMenu.instance.width.GetComponentInChildren<TMP_InputField>().text.ToString()));
        Generator.gen.setHeight(int.Parse(StartMenu.instance.height.GetComponentInChildren<TMP_InputField>().text.ToString()));
        Generator.gen.setBombs(int.Parse(StartMenu.instance.bombs.GetComponentInChildren<TMP_InputField>().text.ToString()));

        flagsRemaining = (int.Parse(StartMenu.instance.bombs.GetComponentInChildren<TMP_InputField>().text.ToString()));

        if (Generator.gen.Validate() == 0) {

            Generator.gen.Generate();
            startMenu.SetActive(false);
        }
        else {

            Debug.Log("Error en los parámetros del juego.");
            //creamos un canvas con el mensaje de error

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
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}