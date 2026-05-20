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
        if (game == null) game = FindObjectOfType<NetworkGameManager>();
        if (runner == null) runner = FindObjectOfType<NetworkRunner>();
        gameObject.SetActive(false);  // Desactivado hasta listo
    }
    void Update() {

        if (game == null || runner == null || game.Object == null) return;
        /* Solo activa cuando ya está spawneado
        if (!gameObject.activeSelf) {
            gameObject.SetActive(true);
            return;
        }*/
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
    void OnDestroy() {
        gameObject.SetActive(false);
    }
}