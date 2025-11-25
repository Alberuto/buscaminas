using UnityEngine;

public class Generator : MonoBehaviour
{

    [SerializeField] private GameObject piecePrefab;

    [HideInInspector] public GameObject[][] Map;
    [HideInInspector] public int Width;
    [HideInInspector] public int Height;
    [HideInInspector] public int BombsNumber;

    public static Generator gen;

    private void Awake() { gen = this; }

    public void SetWidth(int w) => Width = w;
    public void SetHeight(int h) => Height = h;
    public void SetBombs(int b) => BombsNumber = b;

    public int Validate() {
        int error = 0;
        if (Width <= 1) error += 4;
        if (Height <= 1) error += 2;
        if (BombsNumber < 0 || BombsNumber >= Width * Height) error += 1;
        return error;
    }
    public void Generate() {

        Debug.Log("Tablero generado");
        Map = new GameObject[Width][];
        for (int i = 0; i < Width; i++)
            Map[i] = new GameObject[Height];

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                Map[x][y] = Instantiate(piecePrefab, new Vector3(x, y, 0), Quaternion.identity);
                var p = Map[x][y].GetComponent<Piece>();
                p.SetX(x);
                p.SetY(y);
            }
        }
        Camera.main.transform.position = new Vector3(Width / 2f - 0.5f, Height / 2f - 0.5f, -10);

        int placed = 0;
        while (placed < BombsNumber) { 

            int x = Random.Range(0, Width);
            int y = Random.Range(0, Height);
            var p = Map[x][y].GetComponent<Piece>();
            if (!p.isBomb()) {

                p.SetBomb(true);
                placed++;
            }
        }
    }
    public int GetBombsAround(int x, int y) {

        int count = 0;
        for (int dx = -1; dx <= 1; dx++) {
            for (int dy = -1; dy <= 1; dy++) {

                if (dx == 0 && dy == 0) continue;
                int nx = x + dx, ny = y + dy;
                if (nx >= 0 && nx < Width && ny >= 0 && ny < Height)
                    if (Map[nx][ny].GetComponent<Piece>().isBomb()) count++;
            }
        }
        return count;
    }
    public void CheckPieceAround(int x, int y) {

        for (int dx = -1; dx <= 1; dx++) {
            for (int dy = -1; dy <= 1; dy++) {

                if (dx == 0 && dy == 0) continue;
                int nx = x + dx, ny = y + dy;
                if (nx >= 0 && nx < Width && ny >= 0 && ny < Height) {
                    Piece neighbor = Map[nx][ny].GetComponent<Piece>();
                    if (!neighbor.isCheck() && !neighbor.IsMarkedByAI()) {
                        if (neighbor.flaged) {
                            neighbor.EraseFlag(); // elimina bandera visual y actualización estados SASJDFIOJAPDSOFIPOSAJFDIPOSAJDFIPOAJDSFIPOJSFDIPOJASIPOFDJAFDAIPOSDJFIPOASDJFIPOASJFDPOAIJFDIPOAJIPOFDJASF
                        }
                        neighbor.DrawBomb();
                    }
                }
            }
        }
    }
    public void DestroyMap() {

        if (Map == null) return;
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                Destroy(Map[x][y]);
    }
    public void RevealAllBombs() {

        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {

                var p = Map[x][y].GetComponent<Piece>();

                if (p.isBomb()) {

                    var sr = p.GetComponent<SpriteRenderer>();
                    if (sr.material.color != Color.red) sr.material.color = Color.gray;
                    p.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                }
            }
        }
    }
}