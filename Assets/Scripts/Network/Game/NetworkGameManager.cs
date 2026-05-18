using Fusion;
using System.Linq;
using TMPro;
using UnityEngine;

public class NetworkGameManager : NetworkBehaviour {

    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject endMenuVictory;
    [SerializeField] GameObject endMenuLose;

   /* [Networked, Capacity(100*100)]
    public NetworkArray<int> Board => default;*/
    [Networked] public PlayerRef ThisTurn { get; set; }
    [Networked] public bool endGame { get; set; }
    public static NetworkGameManager instance;
    
    private void Awake() {

        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    public override void Spawned() {

        Debug.Log("🔥 NetworkGameManager SPAWNED");

        if (Runner.IsSharedModeMasterClient) {
            ThisTurn = Runner.ActivePlayers.First();
            endGame = false;
            startMenu.SetActive(true);
            endMenuVictory.SetActive(false);
            endMenuLose.SetActive(false);
        }
    }
    public void GameStart(int w, int h, int b) {

        Debug.Log("🔥 GameStart() LLAMADO");
        if (!Runner.IsSharedModeMasterClient) {
                Debug.LogError("❌ no soy master client");
            return;
        }
        if (Generator.gen == null) {
            Debug.LogError("❌ Generator.gen es null");
            return;
        }

        Debug.Log("🔥 MASTER CLIENT ejecutando Generador");
        Generator.gen.setWidth(w);
        Generator.gen.setHeight(h);
        Generator.gen.setBombs(b);
        Debug.Log($"📏 w={w} h={h} b={b}");

        if (Generator.gen.Validate() != 0) {
            Debug.LogWarning("❌ Datos de tablero inválidos");
            return;
        }
        Generator.gen.Generate();
        endGame = false;
        endMenuVictory.SetActive(false);
        endMenuLose.SetActive(false);
        startMenu.SetActive(false);
    }
    public void ReiniciarJuego() {
        if (Runner.IsServer) Runner.Shutdown();
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

        if (endGame || ThisTurn != Runner.LocalPlayer) return;
        Debug.Log($"Player {Runner.LocalPlayer} is trying to play at index bidimensional ( {x} ,{y} )");
        RPC_Play(x,y);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_Play(int x, int y, RpcInfo info = default) {

        if (endGame) return;
        if (x < 0 || x >= Generator.gen.width || y < 0 || y >= Generator.gen.height) return; //validar rango

        int index = y * Generator.gen.width + x;

        // Si ya está marcada, no permitir
        // if (Board.Get(index) != 0) return;
        if (info.Source != ThisTurn) return;
        // int player = (ThisTurn == Runner.ActivePlayers.ToList()[0]) ? 1 : 2;
        //  Board.Set(index, player);
        // Aquí se revela la casilla en todos los clientes

        Generator.gen.RevealPiece(x, y, info.Source == Runner.LocalPlayer);

        Piece piece = Generator.gen.map[x][y].GetComponent<Piece>();

        if (piece.isBomb()) {
            // Derrota: el que pisa bomba pierde
            endGame = true;
            Generator.gen.RevealAllBombs(); // opcional, las demuestra todas
            endMenuLose.SetActive(true);
        }
        else {
            if (CheckVictory()) {
                endGame = true;
                endMenuVictory.SetActive(true);
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