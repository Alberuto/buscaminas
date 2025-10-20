using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StartMenu : MonoBehaviour{

    [SerializeField] public TMP_InputField width;
    [SerializeField] public TMP_InputField height;
    [SerializeField] public TMP_InputField bombs;

    public static StartMenu instance;

    // Start is called once before the first exec
    // ution of Update after the MonoBehaviour is created
    public void Start(){

        instance = this;
    }
}