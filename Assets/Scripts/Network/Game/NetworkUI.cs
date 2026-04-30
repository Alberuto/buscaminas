using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.Unicode;

public class NetworkUI : MonoBehaviour {

    public NetworkGameManager game;
    public NetworkRunner runner; 
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI endGameText;

    void Start() {

        InvokeRepeating(nameof(FindGame), 0.5f, 0.5f);
    }
    void Update() {

        if (game == null) return;

        if (runner == null) runner = FindObjectOfType<NetworkRunner>();

        if (game.endGame) {
            turnText.text = "Fin de partida";
        }
        else {
            string turnStr = (game.ThisTurn == runner.LocalPlayer)
                ? "Tú"
                : "Oponente";
            turnText.text = $"Turno: {turnStr}";
        }
    }
    void FindGame() {

        if (game == null) game = FindObjectOfType<NetworkGameManager>();
        if (runner == null) runner = FindObjectOfType<NetworkRunner>();
    }
}