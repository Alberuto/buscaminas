using UnityEngine;
using Fusion;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;
using Fusion.Sockets;
using Unity.Mathematics;

public class NetworkStarter : MonoBehaviour, INetworkRunnerCallbacks {

    public NetworkRunner runnerPrefab;
    public static NetworkRunner runnerInstance; 
   // public NetworkObject playerPrefab;

    [SerializeField] private string lobbyName = "default";
    [SerializeField] private Transform sessionListContentParent;
    [SerializeField] private GameObject sessionListEntryPrefab;
    [SerializeField] private string gameScene;
    [SerializeField] private string lobbyScene;
    [SerializeField] private Dictionary<string, GameObject> sessionListUiDictionary = new();

    private void Start() {
        DontDestroyOnLoad(gameObject);
        runnerInstance = Instantiate(runnerPrefab);
        runnerInstance.AddCallbacks(this);
        runnerInstance.JoinSessionLobby(SessionLobby.Shared, lobbyName);
    }
    public static void ReturnToLobby() {
        runnerInstance.Shutdown(true, ShutdownReason.Ok);
    }
   
    public void CreateRandomSession() {     //Método para salas random

        int randomInt = UnityEngine.Random.Range(1000, 9999);
        string randomSessionName = "Session creada en la room" + randomInt.ToString();

        runnerInstance.StartGame(new StartGameArgs() {

            Scene = SceneRef.FromIndex(GetSceneIndex(gameScene)),
            SessionName = randomSessionName,
            GameMode = GameMode.Shared,
            PlayerCount = 2,
            IsVisible = true, //false para que no aparezca en la lista de salas y que cree sala "privada"
        });
    }
    private int GetSceneIndex(string sceneName) {

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)  {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == name) return i;
        }
        return -1;
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {

        foreach (var old in new List<string>(sessionListUiDictionary.Keys)) {
            bool exists = false;
            foreach (var session in sessionList) {
                if (session.Name == old) {
                    exists = true;
                    break;
                }
            }
            if (!exists) {
                Destroy(sessionListUiDictionary[old]);
                sessionListUiDictionary.Remove(old);
            }
        }
        foreach (SessionInfo session in sessionList) {

            if (!sessionListUiDictionary.TryGetValue(session.Name, out GameObject entry)) {
                GameObject newEntry = Instantiate(sessionListEntryPrefab, sessionListContentParent);
                sessionListUiDictionary[session.Name] = newEntry;
                entry = newEntry;
            }
            SessionListEntry entryScript = entry.GetComponent<SessionListEntry>();
            entryScript.roomName.text = session.Name;
            entryScript.playerCount.text = session.PlayerCount + "/" + session.MaxPlayers;
            entryScript.joinButton.interactable = session.IsOpen;
            entry.SetActive(session.IsVisible);
        }
    }
    //interfaz
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {
        Debug.Log(new System.NotImplementedException());
    }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {
        Debug.Log(new System.NotImplementedException());
    }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {
        Debug.Log(new System.NotImplementedException());
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
        Debug.Log(new System.NotImplementedException());
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {
        Debug.Log($"Shutdown: {shutdownReason}");
        SceneManager.LoadScene(lobbyScene);
    }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {
        Debug.Log(new System.NotImplementedException());
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {
        Debug.Log(new System.NotImplementedException());
    }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {
        Debug.Log(new System.NotImplementedException());
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {
        Debug.Log(new System.NotImplementedException());
    }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) {
        Debug.Log(new System.NotImplementedException());
    }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {
        Debug.Log(new System.NotImplementedException());
    }
    public void OnInput(NetworkRunner runner, NetworkInput input) {
     //   Debug.Log(new System.NotImplementedException());
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {
        Debug.Log(new System.NotImplementedException());
    }
    public void OnConnectedToServer(NetworkRunner runner) {
        Debug.Log("✅ Conectado al servidor");
    }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {
        Debug.Log(new System.NotImplementedException());
    }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {
        Debug.Log(new System.NotImplementedException());
    }
    public void OnSceneLoadDone(NetworkRunner runner) {
        Debug.Log("✅ Escena Game cargada correctamente");
    }
    public void OnSceneLoadStart(NetworkRunner runner) {
        Debug.Log("✅ Cargando escena Game");
    }
}
