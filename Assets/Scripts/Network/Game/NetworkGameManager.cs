using Fusion;
using System;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkGameManager : NetworkBehaviour {

    [SerializeField] 
        GameObject startMenu;

    [SerializeField] 
        public GameObject endMenu;

    public static NetworkGameManager instance;

    [Networked, Capacity(100*100)]
    public NetworkArray<int> Board => default;

    public int BoardCapacity => Generator.gen.width * Generator.gen.height;

    [Networked]
        public PlayerRef ThisTurn {
            get; set;
        }

    [Networked]
        public bool endGame {
            get; set;
        }

    private void Awake() {

        if (instance == null) {

            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this) {

            Destroy(gameObject);
        }
    }
    public void Start() {

        DontDestroyOnLoad(gameObject);
        startMenu.SetActive(true);
        endMenu.SetActive(false);
        endGame = false;
    }
    public void GameStart() {

        Generator.gen.setWidth(int.Parse(StartMenu.instance.width.GetComponentInChildren<TMP_InputField>().text.ToString()));
        Generator.gen.setHeight(int.Parse(StartMenu.instance.height.GetComponentInChildren<TMP_InputField>().text.ToString()));
        Generator.gen.setBombs(int.Parse(StartMenu.instance.bombs.GetComponentInChildren<TMP_InputField>().text.ToString()));

        if (Generator.gen.Validate() == 0) {
            Generator.gen.Generate();
            startMenu.SetActive(false);
        }
        else {
            Debug.Log("Error en los parámetros del juego.");
            //creamos un canvas con el mensaje de error
        }
    }
    public void ReiniciarJuego() {

        if (Generator.gen.map != null) {
            Generator.gen.DestroyMap();
        }
        Start();
        // Recarga la escena actual al estado inicial
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public bool CheckVictory() {

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

        return safePieces == totalSafe;
    }
    public void TryTurn(int x, int y) {

        if (Runner.IsServer) return;
        if (endGame) return;
        Debug.Log($"Player {Runner.LocalPlayer} is trying to play at index bidimensional ( {x} ,{y} )");
        RPC_Play(x,y);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_Play(int x, int y, RpcInfo info = default) {

        if (endGame) return;
        if (x < 0 || x >= Generator.gen.width || y < 0 || y >= Generator.gen.height) return; //validar rango

        int index = y * Generator.gen.width + x;

        // Si ya está marcada, no permitir
        if (Board.Get(index) != 0) return;

        PlayerRef playerCaller = info.Source;
        if (playerCaller != ThisTurn) return;

        int player = (ThisTurn == Runner.ActivePlayers.ToList()[0]) ? 1 : 2;
        Board.Set(index, player);

        // Aquí se revela la casilla en todos los clientes
        Generator.gen.RevealPiece(x, y, playerCaller == Runner.LocalPlayer);

        Piece piece = Generator.gen.map[x][y].GetComponent<Piece>();

        if (piece.isBomb()) {
            // Derrota: el que pisa bomba pierde
            endGame = true;
            endMenu.SetActive(true);
            Transform victoria = endMenu.transform.Find("Victoria");
            Transform derrota = endMenu.transform.Find("Derrota");
            victoria.gameObject.SetActive(false);
            derrota.gameObject.SetActive(true);
            Generator.gen.RevealAllBombs(); // opcional, las demuestra todas
        }
        else {
            if (CheckVictory()) {
                endGame = true;
                endMenu.SetActive(true);
                Transform victoria = endMenu.transform.Find("Victoria");
                Transform derrota = endMenu.transform.Find("Derrota");
                victoria.gameObject.SetActive(true);
                derrota.gameObject.SetActive(false);
            }
            else {
                ChangeTurn();
            }
        }
    }
    private void ChangeTurn() {

        foreach (var player in Runner.ActivePlayers) {

            if (player != ThisTurn) {
                ThisTurn = player;
                break;
            }
        }
    }
}