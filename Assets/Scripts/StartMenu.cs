using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StartMenu : MonoBehaviour{

    [SerializeField] public TMP_InputField width;
    [SerializeField] public TMP_InputField height;
    [SerializeField] public TMP_InputField bombs;

    public int flags;

    public static StartMenu instance;

    // Start is called once before the first exec
    // ution of Update after the MonoBehaviour is created
    public void Start(){

        instance = this;

        int bombCount;

        if (int.TryParse(bombs.text, out bombCount)){

            flags = bombCount;
        }
        else{
            Debug.LogWarning("No se pudo convertir el valor de 'bombs' a entero.");
            flags = 5; // Valor por defecto si la conversión falla
        }
    }
}