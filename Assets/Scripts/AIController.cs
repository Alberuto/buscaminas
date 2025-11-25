using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIController : MonoBehaviour {  

    [Header("IA por turnos")]
    private float moveDelay = 2.5f; // Retraso visual fijo

    private void OnEnable() {

        StopAllCoroutines();
        StartCoroutine(PlayTurns());
    }
    private IEnumerator PlayTurns() {

        while (!GameManager.instance.endGame && !GameManager.instance.isHumanTurn) {

            while (GameManager.instance.isHumanTurn) // Espera activo pero no bloquea frame
                yield return null;

            yield return new WaitForSeconds(moveDelay);

            bool opened = LogicPlay();
            if (!opened)
                opened = RandomPlay();

            enabled = false;
        }
    }
    // Abre casillas lógicas seguras primero
    private bool LogicPlay() {

        int width = Generator.gen.Width;
        int height = Generator.gen.Height;
        var map = Generator.gen.Map;

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {

                Piece p = map[x][y].GetComponent<Piece>();
                if (!p.isCheck()) continue;

                int bombsAround = Generator.gen.GetBombsAround(x, y);
                if (bombsAround == 0) continue;

                List<Piece> neighbors = GetNeighbors(x, y);
                var hidden = neighbors.Where(c => !c.isCheck() && !c.IsMarkedByAI()).ToList();
                var marked = neighbors.Where(c => c.IsMarkedByAI()).ToList();

                if (bombsAround == hidden.Count + marked.Count && hidden.Count > 0) {

                    foreach (var mine in hidden) mine.MarkByAI();
                    Debug.Log("IA marcó minas lógicas");
                }
                else if (bombsAround == marked.Count && hidden.Count > 0) {
                    // Todas las ocultas son seguras
                    foreach (var safe in hidden)
                        OpenPieceSafe(safe);

                    GameManager.instance.CheckVictoryByClear();
                    GameManager.instance.SwitchTurn(); //La IA cambia el turno cuando juega lógico
                    Debug.Log("Jugadas lógicas seguras realizadas");
                    return true;
                }
            }
        }
        Debug.Log("jugada logica descartada");
        return false;
    }
    // Abre una casilla al azar si no hay jugadas lógicas
    private bool RandomPlay() {

        int width = Generator.gen.Width;
        int height = Generator.gen.Height;
        var map = Generator.gen.Map;

        var candidates = new List<Piece>();
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (!map[x][y].GetComponent<Piece>().isCheck())
                    candidates.Add(map[x][y].GetComponent<Piece>());

        var pick = candidates[Random.Range(0, candidates.Count)];
        pick.DrawBomb();
        GameManager.instance.CheckVictoryByClear();
        GameManager.instance.SwitchTurn(); //la IA cambia el turno cuando juega random
        Debug.Log("jugada random realizada");
        return true;
    }
    // Obtiene vecinas de una casilla
    private List<Piece> GetNeighbors(int x, int y) {

        var neighbors = new List<Piece>();
        int width = Generator.gen.Width;
        int height = Generator.gen.Height;
        var map = Generator.gen.Map;

        for (int dx = -1; dx <= 1; dx++) {
            for (int dy = -1; dy <= 1; dy++) {
                if (dx == 0 && dy == 0) continue;
                int nx = x + dx;
                int ny = y + dy;
                if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    neighbors.Add(map[nx][ny].GetComponent<Piece>());
            }
        }
        return neighbors;
    }
    private void OpenPieceSafe(Piece p) {
        if (p.flaged) {
            p.EraseFlag(); // elimina bandera visualmente y actualiza contadores
        }
        p.DrawBomb();
        if (Generator.gen.GetBombsAround(p.GetX(), p.GetY()) == 0) {
            Generator.gen.CheckPieceAround(p.GetX(), p.GetY()); // abrir en cascada para ceros
        }
    }
}