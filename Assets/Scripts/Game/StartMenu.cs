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

    public void Start(){

        instance = this;

        int bombCount;

        if (int.TryParse(bombs.text, out bombCount)){

            flags = bombCount;
        }
    }
}

/* 
 * 
Falta por hacer: 

(X) 1.- Mostar el mensaje de victoria una vez detectadas (flageadas) todas las bombas
(X) 2.- Que el boton de Empezar sirva para volver a poder jugar (podria preguntarse si con el mismo tablero o modificar)
(X) 3.- Que no escriba banderas en casillas abiertas o con numeros
(X) 4.- Que no escriba/borre banderas una vez has perdido/ganado la partida

5.- Control de errores: 

a) poner mas bombas que casillas
b) hacer un panel mas grande que la pantalla
c) Resumen: Slider de FCO.
d) No obstante hay un codigo de error por consola y no deja iniciar la partida.
*/