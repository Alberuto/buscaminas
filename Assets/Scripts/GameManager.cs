using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Referencias")]
    [SerializeField] public GameObject endMenuPanel;

    [HideInInspector] public bool endGame = false;
    [HideInInspector] public bool isHumanTurn = true;
    [HideInInspector] public int flagsRemaining = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void GameStart()
    {
        Debug.Log("GameStart llamado");
        endGame = false;
        isHumanTurn = true;
        flagsRemaining = Generator.gen.BombsNumber;

        Generator.gen.Generate();

        if (endMenuPanel != null)
            endMenuPanel.SetActive(false);

        var ai = FindObjectOfType<AIController>();
        if (ai != null)
            ai.enabled = false;
    }

    public void SwitchTurn()
    {
        if (endGame) return;

        isHumanTurn = !isHumanTurn;

        if (!isHumanTurn)
        {
            var ai = FindObjectOfType<AIController>();
            if (ai != null && !ai.enabled)
            {
                ai.enabled = true;
            }
        }
    }

    public void EndGame(bool humanWon)
    {
        if (endGame) return;

        Debug.Log("valor de boleano de partida:" + humanWon);
        StartMenu.instance.AddWin(humanWon);

        endGame = true;

        if (endMenuPanel != null)
            endMenuPanel.SetActive(true);

        Transform victoria = endMenuPanel.transform.Find("Victoria");
        Transform derrota = endMenuPanel.transform.Find("Derrota");

        if (victoria != null && derrota != null) {

            victoria.gameObject.SetActive(humanWon);
            derrota.gameObject.SetActive(!humanWon);
        }
    }

    public void ReiniciarJuego()
    {
        Generator.gen.DestroyMap();
        StartMenu.instance.ShowStartMenu();
        StartMenu.instance.HideEndMenu();


    }

    public void CheckVictoryByClear()
    {
        int safePieces = 0;
        for (int x = 0; x < Generator.gen.Width; x++)
            for (int y = 0; y < Generator.gen.Height; y++)
            {
                Piece p = Generator.gen.Map[x][y].GetComponent<Piece>();
                if (!p.isBomb() && p.isCheck())
                    safePieces++;
            }

        int totalSafe = Generator.gen.Width * Generator.gen.Height - Generator.gen.BombsNumber;
        if (safePieces == totalSafe) {

            EndGame(true);

        }
    }
}