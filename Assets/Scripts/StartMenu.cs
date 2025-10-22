using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class StartMenu : MonoBehaviour{

    [SerializeField] public TMP_InputField width;
    [SerializeField] public TMP_InputField height;
    [SerializeField] public TMP_InputField bombs;

    private int flags;

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
            flags = 10; // Valor por defecto si la conversión falla
        }
    }
}

/* 
 
Falta por hacer: 

1.- Mostar el mensaje de victoria una vez detectadas y flageadas todas las bombas

2.- Mostrar el boton de Empezar de nuevo para cargar la interfaz principal y volver a poder jugar

3.- Que no escriba banderas en casillas abiertas o con numeros
4.- Que no escriba banderas una vez has perdido la partida

5.- Control de errores: 

a) poner mas bombas que casillas
b) hacer un panel mas grande que la pantalla
c) 
 
 
 */