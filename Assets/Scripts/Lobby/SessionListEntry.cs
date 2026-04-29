using Fusion.Statistics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class SessionListEntry : MonoBehaviour {

    public TextMeshProUGUI roomName, playerCount;
    public Button joinButton;

    public void JoinRoom() {

        NetworkStarter.runnerInstance.StartGame(new StartGameArgs() {
            SessionName = roomName.text,
            GameMode = GameMode.Shared,
        });
    }
}