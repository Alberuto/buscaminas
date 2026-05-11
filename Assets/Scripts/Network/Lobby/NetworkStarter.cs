using UnityEngine;
using Fusion;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SceneManagement;
using Fusion.Sockets;

public class NetworkStarter : MonoBehaviour, INetworkRunnerCallbacks {

    public NetworkRunner runnerPrefab;
    public static NetworkRunner runnerInstance; 
    public NetworkObject BuscaminasPrefab;

    [SerializeField] private string lobbyName = "default";

    [SerializeField] private Transform sessionListContentParent;
    [SerializeField] private GameObject sessionListEntryPrefab;
    [SerializeField] private Dictionary<string, GameObject> sessionListUiDictionary = new Dictionary<string, GameObject>();

    [SerializeField] private SceneAsset gameScene;
    [SerializeField] private SceneAsset lobbyScene;

    private void Start() {

        DontDestroyOnLoad(this);
        runnerInstance = Instantiate(runnerPrefab);
        runnerInstance.AddCallbacks(this);
        //conexion con el server
        runnerInstance.JoinSessionLobby(SessionLobby.Shared, lobbyName);
    }
    public static void ReturnToLobby() {
        //despawn del player simil a logout, te manda al lobby
        //runnerInstance.Despawn(runnerInstance.GetPlayerObject(runnerInstance.LocalPlayer));
        runnerInstance.Shutdown(true, ShutdownReason.Ok);
    }
    public void OnShutDown(NetworkRunner runner, ShutdownReason reason) {

        SceneManager.LoadScene(lobbyScene.name);
    }
    public void CreateRandomSession() {     //Método para salas random

        int randomInt = Random.Range(1000, 9999);
        string randomSessionName = "Session creada en la room" + randomInt.ToString();

        runnerInstance.StartGame(new StartGameArgs() {

            Scene = SceneRef.FromIndex(GetSceneIndex(gameScene.name)),
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

            if (sceneName == name) {
                return i;
            }
        }
        return -1;
    }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {
        //eliminar lista de sesssiones
        DeleteOldSessionFromUI(sessionList);
        //volvermos a generalo
        CompareLists(sessionList);
    }
    private void CompareLists(List<SessionInfo> sessionList) {

        foreach (SessionInfo session in sessionList) {

            if (!sessionListUiDictionary.ContainsKey(session.Name)) { 
                //tenemos la sesion en el array asociativo (diccionario) claves par/valor
                UpdateEntryUI(session);
            }
            else {
                CreateEntryUI(session);
            }
        }
    }
    private void CreateEntryUI(SessionInfo session) {

        GameObject newEntry = Instantiate(sessionListEntryPrefab);
        newEntry.transform.parent = sessionListContentParent;
        SessionListEntry entryScript = newEntry.GetComponent<SessionListEntry>();
        sessionListUiDictionary.Add(session.Name, newEntry);

        entryScript.roomName.text = session.Name;
        entryScript.playerCount.text = session.PlayerCount.ToString() + "/" + session.MaxPlayers;
        entryScript.joinButton.interactable = session.IsOpen;
        newEntry.SetActive(session.IsVisible);
    }
    private void UpdateEntryUI(SessionInfo session) {
        sessionListUiDictionary.TryGetValue(session.Name, out GameObject newEntry);
        SessionListEntry entryScript = newEntry.GetComponent<SessionListEntry>();

        entryScript.roomName.text = session.Name;
        entryScript.playerCount.text = session.PlayerCount.ToString() + "/" + session.MaxPlayers;
        entryScript.joinButton.interactable = session.IsOpen;
        newEntry.SetActive(session.IsVisible);
    }
    private void DeleteOldSessionFromUI(List<SessionInfo> sessionList) {

        bool isContained = false;
        foreach (KeyValuePair<string, GameObject> entry in sessionListUiDictionary) {
            foreach (SessionInfo session in sessionList) {
                if (entry.Key == session.Name) {
                    isContained = true;
                    break;
                }
            }
            if (!isContained) {
                Destroy(entry.Value);
                sessionListUiDictionary.Remove(entry.Key);
            }
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

        /* Todos jugamos sobre el mismo tablero, comentado por si acaso para otros proyectos, 
         * pero para el tictactoe no es necesario el spawn de un objeto por jugador, 
         * ya que el tablero es compartido y cada jugador solo tiene que enviar su input 
         * para colocar su ficha en el tablero, no necesitan un objeto propio en la escena
         * if(player==runner.LocalPlayer)
             NetworkObject playerObject = runner.Spawn(playerPrefab, Vector3.zero);
             runner.SetPlayerObject(player, playerObject);
        */
        Debug.Log($"✅ Jugador {player} se unió");
    }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {
        Debug.Log(new System.NotImplementedException());
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {
        Debug.Log($"Shutdown: {shutdownReason}");
        SceneManager.LoadScene(lobbyScene.name);
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
       // runner.Spawn(BuscaminasPrefab); explota explota me explota
       // Debug.Log(new System.NotImplementedException());
        Debug.Log("✅ Escena Game cargada correctamente");

    }
    public void OnSceneLoadStart(NetworkRunner runner) {
        Debug.Log("✅ Cargando escena Game");
    }
}

/*
 * MODO SIN LOBBY, SOLO PARA TESTEAR LA CONEXION Y EL SPAWN DE OBJETOS EN LA ESCENA DE JUEGO
 * 
 * 
 * private async void start(){
 *      var runnerInstance = Instantiate(runnerPrefab);
        runnerInstance.ProvideInput = false;

        await runnerInstance.StartGame(new StartGameArgs() {

            GameMode = GameMode.Shared,
            SessionName = "Test",
            SceneManager = runnerInstance.GetComponent<NetworkSceneManagerDefault>()

        });

        if (runnerInstance.IsSharedModeMasterClient) { 

            Debug.Log("Spawning game TTT from host player 1");
            runnerInstance.Spawn(tictactoePrefab);
        }
   }
 */