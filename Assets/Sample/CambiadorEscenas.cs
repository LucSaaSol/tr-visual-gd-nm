using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiadorEscenas : MonoBehaviour {

    public string rutaEscena3d;
    public string rutaEscena4d;

    // Start is called before the first frame update
    /*void Start() {
        
    }*/

    // Update is called once per frame
    /*void Update() {
        
    }*/

    public void cambiaAEscena3d() {
        SceneManager.LoadScene(rutaEscena3d);
    }
    public void cambiaAEscena4d() {
        SceneManager.LoadScene(rutaEscena4d);
    }
}
