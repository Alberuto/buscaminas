using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIController : MonoBehaviour
{
    [Header("IA por turnos")]
    [SerializeField] private float moveDelay = 0.5f; // Retraso visual fijo

    private bool isPlaying = false;

    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(PlayTurns());
    }

    private IEnumerator PlayTurns()
    {
        isPlaying = true;

        while (!GameManager.instance.endGame)
        {
            while (GameManager.instance.isHumanTurn) // Espera activo pero no bloquea frame
                yield return null;

            yield return new WaitForSeconds(moveDelay);

            bool opened = LogicPlay();
            if (!opened)
                opened = RandomPlay();

            if (opened)
                GameManager.instance.SwitchTurn();

            yield return null;
        }

        isPlaying = false;
        enabled = false; // Solo desactivar cuando termine el juego
    }

    // Abre casillas lógicas seguras primero
    private bool LogicPlay()
    {
        int width = Generator.gen.Width;
        int height = Generator.gen.Height;
        var map = Generator.gen.Map;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Piece p = map[x][y].GetComponent<Piece>();
                if (!p.isCheck()) continue;

                int bombsAround = Generator.gen.GetBombsAround(x, y);
                if (bombsAround == 0) continue;

                List<Piece> neighbors = GetNeighbors(x, y);
                var hidden = neighbors.Where(c => !c.isCheck()).ToList();
                int flagged = neighbors.Count(c => c.flaged);

                if (bombsAround == flagged && hidden.Count > 0)
                {
                    foreach (var c in hidden)
                        c.DrawBomb();

                    return true; // abre una vez por turno
                }
            }
        }

        return false;
    }

    // Abre una casilla al azar si no hay jugadas lógicas
    private bool RandomPlay()
    {
        int width = Generator.gen.Width;
        int height = Generator.gen.Height;
        var map = Generator.gen.Map;

        var candidates = new List<Piece>();
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (!map[x][y].GetComponent<Piece>().isCheck())
                    candidates.Add(map[x][y].GetComponent<Piece>());

        if (candidates.Count == 0) return false;

        var pick = candidates[Random.Range(0, candidates.Count)];
        pick.DrawBomb();
        return true;
    }

    // Obtiene vecinas de una casilla
    private List<Piece> GetNeighbors(int x, int y)
    {
        var neighbors = new List<Piece>();
        int width = Generator.gen.Width;
        int height = Generator.gen.Height;
        var map = Generator.gen.Map;

        for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx;
                int ny = y + dy;
                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    neighbors.Add(map[nx][ny].GetComponent<Piece>());
            }

        return neighbors;
    }
}