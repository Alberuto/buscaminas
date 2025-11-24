using UnityEngine;
using TMPro;

public class Piece : MonoBehaviour
{
    [SerializeField] private int x, y;
    [HideInInspector] public bool bomb, check, flaged;

    public void SetX(int val) => x = val;
    public void SetY(int val) => y = val;
    public void SetBomb(bool val) => bomb = val;
    public bool isBomb() => bomb;
    public int GetX() => x;
    public int GetY() => y;
    public bool isCheck() => check;
    public void SetCheck(bool val) => check = val;

    private bool markedByAI = false;
    public bool IsMarkedByAI() => markedByAI;
    public void MarkByAI() {

        markedByAI = true;
        // Opcional: activa una visualización distintiva, por ejemplo una bandera especial IA, aunque daria pistas al humano por ende descarto idea
    }
    private void OnMouseDown() {

        if (!GameManager.instance.endGame && !flaged && GameManager.instance.isHumanTurn)
            DrawBomb();
        GameManager.instance.SwitchTurn(); //el jugador cede el turno
    }
    public void DrawBomb() {

        if (isCheck() || GameManager.instance.endGame) return;

        if (flaged) {
            EraseFlag();  // Quita la bandera visual y estado
        }

        check = true;

        if (bomb) {

            GetComponent<SpriteRenderer>().color = Color.red;
            Generator.gen.RevealAllBombs();
            GameManager.instance.EndGame(!GameManager.instance.isHumanTurn);
        }
        else {

            int bombsAround = Generator.gen.GetBombsAround(x, y);

            if (bombsAround > 0) {

                var text = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
                text.text = bombsAround.ToString();
                switch (bombsAround) { // Aplicar color según el número

                    case 1:
                        text.color = Color.blue;
                        break;
                    case 2:
                        text.color = Color.magenta;
                        break;
                    case 3:
                        text.color = Color.red;
                        break;
                    case 4:
                        text.color = Color.yellow;
                        break;
                    case 5:
                        text.color = Color.green;
                        break;
                    case 6:
                        text.color = Color.cyan;
                        break;
                    case 7:
                        text.color = Color.gray;
                        break;
                    case 8:
                        text.color = Color.black;
                        break;
                    default:
                        text.color = Color.white;
                        break;
                }
            }
            else {
                GetComponent<SpriteRenderer>().color = Color.gray;
                Generator.gen.CheckPieceAround(x, y);
            }
            GameManager.instance.CheckVictoryByClear();
            GameManager.instance.flagPlacedThisTurn = false;
        }
    }
    private void Update() {
        if (Input.GetMouseButtonDown(1))
            DetectRightClick();
    }
    public void DetectRightClick() {

        if (!GameManager.instance.isHumanTurn || GameManager.instance.endGame) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == this.gameObject) {

            if (!flaged && GameManager.instance.flagsRemaining > 0 && !check) {

                DrawFlag();
                GameManager.instance.flagsRemaining--;
            }
            else if (flaged) {

                EraseFlag();
                GameManager.instance.flagsRemaining++;
            }
        }
    }
    public void DrawFlag() {
        if (GameManager.instance.flagPlacedThisTurn) { // No permitir poner otra bandera
            Debug.Log("Solo puedes poner una bandera por turno.");
            return; 
        }
        transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        flaged = true;
        GameManager.instance.flagPlacedThisTurn = true;
        GameManager.instance.CheckVictoryByFlags();
    }
    public void EraseFlag() {
        transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        flaged = false;
        GameManager.instance.flagPlacedThisTurn = false;
    }
}