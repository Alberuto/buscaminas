using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Piece : MonoBehaviour {

    [SerializeField] private int x, y, bombsDetected=0;
    [SerializeField] private bool bomb, check, flaged;

    public void setX(int x) { 

        this.x = x;
    }
    public void setY(int y) {

        this.y = y;
    }
    public void setBomb(bool bomb) {

        this.bomb = bomb;
    }
    public bool isBomb() {

        return bomb;
    }
    public int getX() {

        return x;
    }
    public int getY() { 

        return y;
    }
    private void OnMouseDown() {

        if(!GameManager.instance.endGame)
            DrawBomb();

    }
    public void DrawBomb() {

        if (!isCheck()) {

            setCheck(true);

            if (isBomb()) {

                GetComponent<SpriteRenderer>().material.color = Color.red;
                transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                //impide que sigas jugando
                GameManager.instance.endGame = true;
                //muestra el mensaje de derrota
                GameManager.instance.endMenu.SetActive(true);
            }
            else {

                int bombsNumer = Generator.gen.GetBombsAround(x, y);

                if (bombsNumer != 0) {

                    var textComponent = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
                    textComponent.text = bombsNumer.ToString();

                    if (bombsNumer == 1)
                        textComponent.color = Color.white;                  
                    if (bombsNumer == 2)
                        textComponent.color = Color.yellow;
                    if (bombsNumer == 3)
                        textComponent.color= Color.orange;
                    if (bombsNumer >= 4)
                        textComponent.color = Color.red;
                 }
                else { 

                    GetComponent<Renderer>().material.color = Color.gray5;
                    Generator.gen.CheckPieceAround(x, y);

                }
            }
        }
    }
    public void setCheck(bool v) { 

        this.check = v;
    }
    public bool isCheck() {

        return check;
    }
    void Update() {

        if (Input.GetMouseButtonDown(1)) {
            DetectRightClick();
        }
    }
    public void DetectRightClick() {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject == this.gameObject) {

            if (!flaged && GameManager.instance.flagsRemaining > 0)
            {
                DrawFlag();
                GameManager.instance.flagsRemaining--;

                if (isBomb())
                    bombsDetected++;
            }
            else if (flaged){

                EraseFlag();
                GameManager.instance.flagsRemaining++;

                if (isBomb())
                    bombsDetected--;
            }
        }
        if (bombsDetected == Generator.gen.bombsNumber && GameManager.instance.flagsRemaining == 0){

            Debug.Log("¡Has ganado!");
            GameManager.instance.endGame = true;
            GameManager.instance.endMenu.SetActive(true);
        }

        Debug.Log("Banderas restantes: " + GameManager.instance.flagsRemaining);
    }
    public void DrawFlag() {

        transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        flaged = true;

    }
    public void EraseFlag() {

        transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        flaged = false;

    }
}