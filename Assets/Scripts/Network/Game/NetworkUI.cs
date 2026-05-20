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
            turnText.text = $"Turno:  {game.ThisTurn}";
        }
    }
    void OnDestroy() {
        gameObject.SetActive(false);
    }
}