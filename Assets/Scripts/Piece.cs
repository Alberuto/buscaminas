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

    private void OnMouseDown()
    {
        if (!GameManager.instance.endGame && !flaged && GameManager.instance.isHumanTurn)
            DrawBomb();
    }

    public void DrawBomb()
    {
        if (isCheck() || GameManager.instance.endGame) return;

        check = true;

        if (bomb)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            GameManager.instance.endGame = true;
            GameManager.instance.endMenuPanel.SetActive(true);

            Transform victoria = GameManager.instance.endMenuPanel.transform.Find("Victoria");
            Transform derrota = GameManager.instance.endMenuPanel.transform.Find("Derrota");

            victoria.gameObject.SetActive(false);
            derrota.gameObject.SetActive(true);

            Generator.gen.RevealAllBombs();
        }
        else
        {
            int bombsAround = Generator.gen.GetBombsAround(x, y);

            if (bombsAround > 0)
            {
                var text = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
                text.text = bombsAround.ToString();
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.gray;
                Generator.gen.CheckPieceAround(x, y);
            }

            GameManager.instance.CheckVictoryByClear();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
            DetectRightClick();
    }

    public void DetectRightClick()
    {
        if (!GameManager.instance.isHumanTurn || GameManager.instance.endGame) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == this.gameObject)
        {
            if (!flaged && GameManager.instance.flagsRemaining > 0 && !check)
            {
                DrawFlag();
                GameManager.instance.flagsRemaining--;
            }
            else if (flaged)
            {
                EraseFlag();
                GameManager.instance.flagsRemaining++;
            }
        }
    }

    public void DrawFlag()
    {
        transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        flaged = true;
    }

    public void EraseFlag()
    {
        transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        flaged = false;
    }
}