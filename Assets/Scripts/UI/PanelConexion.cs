using System.Collections;
using UnityEngine;

public class PanelConexion : MonoBehaviour {

    [SerializeField] private float tiempo;
    void Start() {
        StartCoroutine(OcultarPanel());
    }
    void Update() {
        
    }
    IEnumerator OcultarPanel() {
        yield return new WaitForSeconds(tiempo);
        this.gameObject.SetActive(false);
    }
}