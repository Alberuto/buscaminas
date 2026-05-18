using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StartMenu : MonoBehaviour{

    [SerializeField] public TMP_InputField width;
    [SerializeField] public TMP_InputField height;
    [SerializeField] public TMP_InputField bombs;

    public static StartMenu instance;

    public void Awake(){
        instance = this;
    }
    public void OnClickStart() {

        if (NetworkGameManager.instance == null) 
            return;
        if (!int.TryParse(width.text, out int w)) 
            return;
        if (!int.TryParse(height.text, out int h)) 
            return;
        if (!int.TryParse(bombs.text, out int b)) 
            return;

        NetworkGameManager.instance.GameStart(w, h, b);
    }
}