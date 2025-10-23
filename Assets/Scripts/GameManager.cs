using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour{

    [SerializeField] GameObject startMenu;
    [SerializeField] public GameObject endMenu;

    public bool endGame;
    public static GameManager instance;
    public int flagsRemaining;

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

    }
    public void GameStart() {

        Generator.gen.setWidth(int.Parse(StartMenu.instance.width.GetComponentInChildren<TMP_InputField>().text.ToString()));
        Generator.gen.setHeight(int.Parse(StartMenu.instance.height.GetComponentInChildren<TMP_InputField>().text.ToString()));
        Generator.gen.setBombs(int.Parse(StartMenu.instance.bombs.GetComponentInChildren<TMP_InputField>().text.ToString()));

        flagsRemaining = (int.Parse(StartMenu.instance.bombs.GetComponentInChildren<TMP_InputField>().text.ToString()));

        if (Generator.gen.Validate() == 0){

            Generator.gen.Generate();
            startMenu.SetActive(false);
        }
        else {

            Debug.Log("Error en los par�metros del juego.");
            //creamos un canvas con el mensaje de error

        }
    }
    public void flagsMinus(){
        flagsRemaining--;
    }
    public void flagsPlus(){
        flagsRemaining++;
    }
    public int flags() {
        return flagsRemaining;
    }
    public void ReiniciarJuego(){

        if (Generator.gen.map != null) {

            Generator.gen.DestroyMap();
        
        }
        // Recarga la escena actual al estado inicial
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Start();
    }
}