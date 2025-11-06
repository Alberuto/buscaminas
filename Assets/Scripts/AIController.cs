using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


public class AIController : MonoBehaviour {

    public float turnTime = 1.5f;

    void Start() {

        StartCoroutine(Play());
    }
    System.Collections.IEnumerator Play() {

        if (turnTime <= 0f)
            yield break; // Salir sin hacer nada si turnTime es 0 o menos = boot desactivado

        yield return new WaitForSeconds(turnTime);

        while (!GameManager.instance.endGame){

            if (!LogicPlay()){
                RandomPlay();
            }
            yield return new WaitForSeconds(turnTime);
        }
    }
    bool LogicPlay() {

        bool action = false;

        int width = Generator.gen.width;
        int height = Generator.gen.height;
        var map = Generator.gen.map;
        // Buscar piezas abiertas (check == true) con minas alrededor (número distinto de 0)
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {

                Piece p = map[x][y].GetComponent<Piece>();
                if (p.isCheck()) {

                    int bombsAround = Generator.gen.GetBombsAround(x, y);
                    if (bombsAround == 0) continue;

                    // Listar casillas vecinas
                    List<Piece> neighbors = GetNeighbors(x, y);

                    // Separar vecinas ocultas y con bandera
                    var hidden = neighbors.Where(cell => !cell.isCheck() && !cell.flaged).ToList();
                    int flagged = neighbors.Count(cell => cell.flaged);

                    // Regla 1: Si el número de celdas ocultas es igual al número de minas restantes, todas son minas
                    if (hidden.Count > 0 && bombsAround - flagged == hidden.Count) {

                        foreach (var cell in hidden) {

                            cell.DrawFlag();
                            GameManager.instance.FlagPlaced(cell.isBomb());
                            action = true;
                        }
                        if (action) return true; // Ejecuta solo una acción por turno
                    }
                    // Regla 2: Si el número de banderas ya puestas es igual al número total, las demás ocultas son seguras
                    if (bombsAround == flagged && hidden.Count > 0) {

                        foreach (var cell in hidden) {
                            cell.DrawBomb();
                            action = true;
                        }
                        if (action) return true; // Ejecuta solo una acción por turno
                    }
                }
            }
        }
        // Buscamos todas las casilla comprobadas con bombas alrededor (check == true)
        // Para cada casilla comprobada   
        // Regla 1: todas ocultas son minas: click_derecho (Flag) 
        // Regla 2: todas ocultas son seguras: clic_izquierdo (Flag)
        return action;
    }
    // Selecciona una celda al azar (no abierta, sin bandera)
    void RandomPlay() {

        int width = Generator.gen.width;
        int height = Generator.gen.height;
        var map = Generator.gen.map;

        var candidates = new List<Piece>();
        for (int x = 0; x < width; x++) {

            for (int y = 0; y < height; y++) {

                Piece p = map[x][y].GetComponent<Piece>();
                if (!p.isCheck() && !p.flaged) {

                    candidates.Add(p);
                }
            }
        }
        if (candidates.Count > 0) {

            var pick = candidates[Random.Range(0, candidates.Count)];
            pick.DrawBomb();
        }
    }
    List<Piece> GetNeighbors(int x, int y) {

        var neighbors = new List<Piece>();
        int width = Generator.gen.width;
        int height = Generator.gen.height;
        var map = Generator.gen.map;

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
    public void RestartAI(){

        StopAllCoroutines();
        StartCoroutine(Play());
    }
}

/*
10 test  8 / 8 / 8
3 fail
5 success
 */