using Fusion;
using System.Linq;
using TMPro;
using UnityEngine;

public class NetworkGameManager : NetworkBehaviour {

    [SerializeField] 
        GameObject startMenu;

    [SerializeField] 
        public GameObject endMenu;

    [Networked, Capacity(100*100)]
        public NetworkArray<int> Board => default;  

    [Networked] 
        public int Width { get; set; }
    [Networked] 
        public int Height { get; set; }

    public static NetworkGameManager instance;
    public int BoardCapacity => Width * Height;

    [Networked]
        public PlayerRef ThisTurn {
            get; set;
        }

    [Networked]
        public bool endGame {
            get; set;
        }

    private void Awake() {

        if (instance == null)
            instance = this;
        else 
            Destroy(gameObject);
    }
    public override void Spawned() {
        endGame = false;
        if (Runner.IsServer) {
            ThisTurn = Runner.ActivePlayers.First();
            startMenu.SetActive(true);
            endMenu.SetActive(false);
        }
        base.Spawned();
    }
    public void GameStart() {

        if (!Runner.IsServer) return;

        int w = int.Parse(StartMenu.instance.width.text);
        int h = int.Parse(StartMenu.instance.height.text);
        int b = int.Parse(StartMenu.instance.bombs.text);

        Generator.gen.setWidth(w);
        Generator.gen.setHeight(h);
        Generator.gen.setBombs(b);

        if (Generator.gen.Validate() == 0) {
            Generator.gen.Generate();
            Width = w; Height = h;  // ← AGREGAR
            startMenu.SetActive(false);
        }
    }
    public void ReiniciarJuego() {

        if (Runner.IsServer) Runner.Shutdown();
    }
    public bool CheckVictory() {

        int safePieces = 0;
        for (int i = 0; i < Width; i++) {
            for (int j = 0; j < Height; j++) {
                Piece p = Generator.gen.map[i][j].GetComponent<Piece>();
                if (!p.isBomb() && p.isCheck()) {
                    safePieces++;
                }
            }
        }
        int totalSafe = Width * Height - Generator.gen.bombsNumber;

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
        if (x < 0 || x >= Width || y < 0 || y >= Height) return; //validar rango

        int index = y * Width + x;

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
            Generator.gen.RevealAllBombs(); // opcional, las demuestra todas
        }
        else {
            if (CheckVictory()) {
                endGame = true;
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
    public override void FixedUpdateNetwork() {

        if (!Runner.IsServer || !endGame) return;
        endMenu.SetActive(true);
        // Victoria/derrota por defecto derrota
        endMenu.transform.Find("Derrota").gameObject.SetActive(true);
        endMenu.transform.Find("Victoria").gameObject.SetActive(false);
    }
}