using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Piece : MonoBehaviour {

    [SerializeField] private int x, y;
    [SerializeField] private bool bomb, check;
    private NetworkGameManager netGame;
    private void Awake() {
        netGame = FindObjectOfType<NetworkGameManager>();
    }
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
    public void setCheck(bool v) {
        this.check = v;
    }
    public bool isCheck() {
        return check;
    }
    private void OnMouseDown() {

        if (netGame == null || !netGame.Object.HasInputAuthority) 
            return;
        netGame.TryTurn(x, y);
    }
    public void DrawBomb() {

        if (isCheck()) return;

        setCheck(true);

        if (isBomb()) {

            GetComponent<SpriteRenderer>().material.color = Color.red;
            transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            // NO ponemos GameManager.instance.endGame aquí
            // ni activamos menús; lo hace RPC_Play
        }
        else {

            int bombsNumer = Generator.gen.GetBombsAround(x, y);
            var textComponent = transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

            if (bombsNumer != 0) {
                textComponent.text = bombsNumer.ToString();
                textComponent.color = GetColorForNumber(bombsNumer);
            }
            else {
                GetComponent<Renderer>().material.color = new Color(0.5f, 0.5f, 0.5f); // Color.gray5
                Generator.gen.CheckPieceAround(x, y);
            }
        }
    }
    private Color GetColorForNumber(int n) {
        
        return n switch {
            1 => Color.blue,
            2 => Color.magenta,
            3 => Color.red,
            4 => Color.yellow,
            5 => Color.green,
            6 => Color.cyan,
            7 => Color.gray,
            8 => Color.black,
            _ => Color.white
        };
    }

}