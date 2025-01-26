using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class ControladorPrincipal : MonoBehaviour {

    public GameObject esfera;
    public GameObject canyon;
    public GameObject marcador;
    public GameObject marcadorFinal;
    public GameObject camaraPrincipal;
    public GameObject flecha;

    public GameObject contenedorMarcadores;
    private int indiceMarcador;
        
    public float potencia; // Max potencia para no salirse: 1176 (antigua: 590)
    public float multiplicadorPotencia; // Cuanto mas alto, mas potencia se puede dar
    public float angulo;

    public int tipoIndicadorJugador; // 0-> Indicador normal  1-> Indicador con flecha  2-> Indicadores cercanos  3-> Indicadores cercanos con flecha
    public bool GDUsaFlecha;
    public float escalaGradienteFlecha;
    public float escalaNewtonFlecha;

    public int rivalActual; // 0-> Sin rival  1-> Gradient descent  2-> Newton Method  3->Ambos

    public float posicionX;
    public float posicionY; // En realidad es la coordenada Z
    private bool yaMedido;

    public float xKGradiente;
    public float yKGradiente;

    public float xKNewton;
    public float yKNewton;

    public float xKCombinado;
    public float yKCombinado;

    public Color colorMarcadoresJugador;
    public Color colorMarcadoresGradient;
    public Color colorMarcadoresNewton;
    public Color colorFlechasJugador;
    public Color colorFlechasGradient;
    public Color colorFlechasNewton;
    public Color colorMarcadoresMetodoCombinado;
    public Color colorFlechasMetodoCombinado;

    private float originClickX;
    private float originClickY;

    private float gravedad=9.8f;

    public int jugadorActual; // 0->Jugador  1->Gradient descent  2->Newton method  3->Metodo combinado

    public bool estadoMetodoCombinado; // False->GD  True->NM

    // Parámetros para las simulaciones
    public float toleranciaSimulacion;
    public float alphaGDSimulacion;
    public int iteracionesSimulacion;


    public GameObject indicadorCanyon;

    public GameObject textoPotencia;
    public GameObject textoAngulo;
    public GameObject botonAumentarPotencia;
    public GameObject botonDisminuirPotencia;
    public GameObject botonAumentarAngulo;
    public GameObject botonDisminuirAngulo;
    public GameObject botonEscogerRival;
    public GameObject botonEscogerModoGradiente;
    public GameObject botonEscogerIndicadorJugador;
    public GameObject textoControles;
    public GameObject textoPotenciaGradiente;
    public GameObject textoAnguloGradiente;
    public GameObject textoPotenciaNewton;
    public GameObject textoAnguloNewton;
    public GameObject botonEscogerAnimacion;
    public GameObject textoPotenciaCombinado;
    public GameObject textoAnguloCombinado;

    private float potenciaLanzamientoActual;
    private float anguloLanzamientoActual;


    private bool juegoFinalizado;

    private bool animacionEsferaActivada;

    public int aQuienTocaLanzar; //0-> Nadie  1-> Gradient descent  2->Newton method  3->Metodo combinado

    // El objetivo que de mas puntos tiene que tener c=10, y siempre tiene que haber uno que tenga c=10
    private Objetivo[] objetivos;



    // Start is called before the first frame update
    void Start() {
        yaMedido=true;
        indiceMarcador=0;

        tipoIndicadorJugador=0;

        juegoFinalizado=false;

        animacionEsferaActivada=true;

        aQuienTocaLanzar=0;

        estadoMetodoCombinado=false;


        botonAumentarPotencia.GetComponent<Button>().onClick.AddListener(() => {
            potencia+=1;
            if(potencia>1200) potencia=1200;
        });
        botonDisminuirPotencia.GetComponent<Button>().onClick.AddListener(() => {
            potencia -= 1;
            if(potencia <= 0) potencia = 0;
        });
        botonAumentarAngulo.GetComponent<Button>().onClick.AddListener(() => {
            angulo+=0.25f;
            if(angulo>=360) angulo=0;
        });
        botonDisminuirAngulo.GetComponent<Button>().onClick.AddListener(() => {
            angulo-=0.25f;
            if(angulo<0) angulo=359;
        });

        botonEscogerRival.GetComponent<Button>().onClick.AddListener(() => {
            rivalActual++;
            if(rivalActual>3) rivalActual=0;
        });
        botonEscogerModoGradiente.GetComponent<Button>().onClick.AddListener(() => {
            GDUsaFlecha=!GDUsaFlecha;
        });
        botonEscogerIndicadorJugador.GetComponent<Button>().onClick.AddListener(() => {
            tipoIndicadorJugador++;
            if(tipoIndicadorJugador>3) tipoIndicadorJugador=0;
        });

        botonEscogerAnimacion.GetComponent<Button>().onClick.AddListener(() => {
            animacionEsferaActivada=!animacionEsferaActivada;
        });



        originClickX =-1;
        originClickY=-1;

        // Valores iniciales de gradient descent
        xKGradiente=Random.Range((float)-10, 10);
        yKGradiente=Random.Range((float)-10, 10);

        // Valores iniciales de newton method
        xKNewton=Random.Range((float)-10, 10);
        yKNewton=Random.Range((float)-10, 10);

        // Valores iniciales del metodo combinado
        xKCombinado=Random.Range((float)-10, 10);
        yKCombinado=Random.Range((float)-10, 10);

        jugadorActual=0;

        objetivos=reinicializarObjetivos(3);
    }

    // Update is called once per frame
    void Update() {

        if(Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
        }

        if(Input.GetMouseButtonDown(0)) {
            originClickX=Input.mousePosition.x;
            originClickY=Input.mousePosition.y;
        }
        if(Input.GetMouseButtonUp(0)) {
            originClickX=-1;
            originClickY=-1;
        }
        // Rotacion camara
        if((originClickX>=0)&&(originClickY>=0)){
            camaraPrincipal.transform.Rotate(0, (Input.mousePosition.x-originClickX)/5, 0, Space.World);
            camaraPrincipal.transform.Rotate((-Input.mousePosition.y+originClickY)/5, 0, 0);

            originClickX = Input.mousePosition.x;
            originClickY = Input.mousePosition.y;

            //Mathf.Clamp(camaraPrincipal.transform.eulerAngles.x, -89, 89);
            //Debug.Log(camaraPrincipal.transform.eulerAngles.x);
        }

        float velocidadMovimiento=1.3f*Time.deltaTime;
        float cambioValores=1;

        if(Input.GetKey(KeyCode.LeftShift)) {
            velocidadMovimiento*=2.5f;
            cambioValores*=2;
        }
        if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftAlt)) {
            velocidadMovimiento*=0.3f;
            cambioValores*=0.25f;
        }

        // Movimiento de la camara
        if(Input.GetKey(KeyCode.W)) {
            camaraPrincipal.transform.position+=new Vector3(camaraPrincipal.transform.forward.x, 0, camaraPrincipal.transform.forward.z).normalized * velocidadMovimiento;
        }
        if(Input.GetKey(KeyCode.S)) {
            camaraPrincipal.transform.position+=new Vector3(camaraPrincipal.transform.forward.x, 0, camaraPrincipal.transform.forward.z).normalized * -velocidadMovimiento;
        }
        if(Input.GetKey(KeyCode.A)) {
            Vector3 direccion=Quaternion.Euler(0, -90, 0)*camaraPrincipal.transform.forward;
            camaraPrincipal.transform.position+=new Vector3(direccion.x, 0, direccion.z).normalized * velocidadMovimiento;
        }
        if(Input.GetKey(KeyCode.D)) {
            Vector3 direccion=Quaternion.Euler(0, -90, 0)*camaraPrincipal.transform.forward;
            camaraPrincipal.transform.position+=new Vector3(direccion.x, 0, direccion.z).normalized * -velocidadMovimiento;
        }
        // Evitar que se salga del mapa
        if(camaraPrincipal.transform.position.x>10) camaraPrincipal.transform.position=new Vector3(10, camaraPrincipal.transform.position.y, camaraPrincipal.transform.position.z);
        if(camaraPrincipal.transform.position.x<-10) camaraPrincipal.transform.position=new Vector3(-10, camaraPrincipal.transform.position.y, camaraPrincipal.transform.position.z);
        if(camaraPrincipal.transform.position.z>10) camaraPrincipal.transform.position=new Vector3(camaraPrincipal.transform.position.x, camaraPrincipal.transform.position.y, 10);
        if(camaraPrincipal.transform.position.z<-10) camaraPrincipal.transform.position=new Vector3(camaraPrincipal.transform.position.x, camaraPrincipal.transform.position.y, -10);

        if(Input.GetKey(KeyCode.Q)) {
            camaraPrincipal.transform.Translate(Vector3.down*velocidadMovimiento, Space.World);
            if(camaraPrincipal.transform.position.y < 0.01f) {
                camaraPrincipal.transform.position=new Vector3(camaraPrincipal.transform.position.x, 0.01f, camaraPrincipal.transform.position.z);
            }
        }
        if(Input.GetKey(KeyCode.E)) {
            camaraPrincipal.transform.Translate(Vector3.up*velocidadMovimiento, Space.World);
            if(camaraPrincipal.transform.position.y > 20) {
                camaraPrincipal.transform.position = new Vector3(camaraPrincipal.transform.position.x, 20, camaraPrincipal.transform.position.z);
            }
        }


        bool seHaLanzadoSinAnimacion=false;
        if(Input.GetKeyDown(KeyCode.Space) && potencia>=0 && yaMedido && !juegoFinalizado) {
            if(animacionEsferaActivada) {
                lanzarEsfera(potencia, angulo, colorMarcadoresJugador);
            } else {
                seHaLanzadoSinAnimacion=true;
                yaMedido = false;
            }
            jugadorActual=0;
            potenciaLanzamientoActual=potencia;
            anguloLanzamientoActual=angulo;
        }

        if(Input.GetKey(KeyCode.DownArrow)) {
            potencia-=cambioValores*4;
            if(potencia<=0) potencia=0;
        }
        if(Input.GetKey(KeyCode.UpArrow)) {
            potencia+=cambioValores*4;
            if(potencia>1200) potencia=1200;
        }
        if(Input.GetKey(KeyCode.LeftArrow)) {
            angulo+=cambioValores;
            if(angulo>=360) angulo=0;
        }
        if(Input.GetKey(KeyCode.RightArrow)) {
            angulo-=cambioValores;
            if(angulo<0) angulo=359;
        }

        if(Input.GetKeyDown(KeyCode.C)) {
            if(textoControles.activeSelf) {
                textoControles.SetActive(false);
            } else {
                textoControles.SetActive(true);
            }
        }

        if(Input.GetKeyDown(KeyCode.F) && !juegoFinalizado) {
            for(int i=0; i<objetivos.Length; i++) {
                colocarMarcadorFinal(objetivos[i].a, objetivos[i].b, funcionPrincipal(objetivos[i].a, objetivos[i].b, objetivos).ToString("n"), Color.black);
            }
            juegoFinalizado=true;
        }

        if(Input.GetKeyDown(KeyCode.R)) {
            juegoFinalizado=false;
            estadoMetodoCombinado=false;
            for(int i=0; i<contenedorMarcadores.transform.childCount; i++) {
                Destroy(contenedorMarcadores.transform.GetChild(i).gameObject);
            }
            indiceMarcador=0;
            objetivos = reinicializarObjetivos(3);

            // Valores iniciales de gradient descent
            xKGradiente = Random.Range((float)-10, 10);
            yKGradiente = Random.Range((float)-10, 10);

            // Valores iniciales de newton method
            xKNewton = Random.Range((float)-10, 10);
            yKNewton = Random.Range((float)-10, 10);

            // Valores iniciales del metodo combinado
            xKCombinado = Random.Range((float)-10, 10);
            yKCombinado = Random.Range((float)-10, 10);
        }

        if(((Input.GetKeyDown(KeyCode.G) && yaMedido) || aQuienTocaLanzar==1) && !juegoFinalizado) {
            jugadorActual=1;
            float[] valoresCalculados = deCoordenadasAPotenciaAngulo(xKGradiente, yKGradiente);
            if(animacionEsferaActivada) {
                lanzarEsfera(valoresCalculados[0], valoresCalculados[1], colorMarcadoresGradient);
                Debug.Log("Holi lanzandoG:"+esfera.transform.position);
            } else {
                seHaLanzadoSinAnimacion = true;
                yaMedido = false;
            }
            aQuienTocaLanzar = 0;
            Debug.Log("X:" + xKGradiente + " Y:" + yKGradiente);
        }

        if(((Input.GetKeyDown(KeyCode.N) && yaMedido) || aQuienTocaLanzar==2) && !juegoFinalizado) {
            jugadorActual=2;
            float[] valoresCalculados = deCoordenadasAPotenciaAngulo(xKNewton, yKNewton);
            if(animacionEsferaActivada) {
                lanzarEsfera(valoresCalculados[0], valoresCalculados[1], colorMarcadoresNewton);
            } else {
                seHaLanzadoSinAnimacion = true;
                yaMedido = false;
            }
            aQuienTocaLanzar=0;
            Debug.Log("X:" + xKNewton + " Y:" + yKNewton + " Puntos:" + funcionPrincipal(xKNewton, xKNewton, objetivos) + " Potencia:" + valoresCalculados[0] + " Angulo:" + valoresCalculados[1]);
        }

        if(((Input.GetKeyDown(KeyCode.H) && yaMedido) || aQuienTocaLanzar == 3) && !juegoFinalizado) {
            jugadorActual=3;
            float[] valoresCalculados = deCoordenadasAPotenciaAngulo(xKCombinado, yKCombinado);
            if(animacionEsferaActivada) {
                lanzarEsfera(valoresCalculados[0], valoresCalculados[1], colorMarcadoresMetodoCombinado);
            } else {
                seHaLanzadoSinAnimacion = true;
                yaMedido = false;
            }
            aQuienTocaLanzar=0;
            Debug.Log("X:" + xKCombinado + " Y:" + yKCombinado + " Puntos:" + funcionPrincipal(xKCombinado, xKCombinado, objetivos) + " Potencia:" + valoresCalculados[0] + " Angulo:" + valoresCalculados[1]);
        }

        // Ejecutar simulacion
        /*if(Input.GetKeyDown(KeyCode.Return)) {
            int cantidadSimulaciones=1000;

            float mediaGD=0;
            float mediaNM=0;
            float mediaMC=0;

            for(int i=0; i<cantidadSimulaciones; i++) {
                float[] resultadosSimulacionActual=simulacionMetodos(iteracionesSimulacion, alphaGDSimulacion, toleranciaSimulacion, false);
                mediaGD+=resultadosSimulacionActual[0];
                mediaNM+=resultadosSimulacionActual[1];
                mediaMC+=resultadosSimulacionActual[2];
            }
            mediaGD/=cantidadSimulaciones;
            mediaNM/=cantidadSimulaciones;
            mediaMC/=cantidadSimulaciones;

            // float[] resultadosSimulacion=simulacionMetodos(iteracionesSimulacion, alphaGDSimulacion, toleranciaSimulacion);

            Debug.Log("Resultados: GD:"+ mediaGD + " NM:"+ mediaNM + " MC:"+ mediaMC);

        }*/

        // Hacer que todos los textos de los marcadores miren hacia la camara
        /*for(int i=0; i<contenedorMarcadores.transform.childCount; i++) {
            Transform textoDelMarcador= contenedorMarcadores.transform.GetChild(i).GetChild(0);
            textoDelMarcador.rotation=Quaternion.LookRotation(camaraPrincipal.transform.forward, camaraPrincipal.transform.up);
            textoDelMarcador.position=new Vector3(textoDelMarcador.position.x, 0.03f, textoDelMarcador.position.z);
        }*/

        //Cambiar el ángulo del cañón
        if(yaMedido) {
            canyon.transform.eulerAngles = new Vector3(1, angulo * -1, 0.006f);
        } else {
            switch(jugadorActual) {
                case 0:
                    canyon.transform.eulerAngles = new Vector3(1, angulo * -1, 0.006f);
                    break;
                case 1:
                    canyon.transform.eulerAngles = new Vector3(1, deCoordenadasAPotenciaAngulo(xKGradiente, yKGradiente)[1] * -1, 0.006f);
                    break;
                case 2:
                    canyon.transform.eulerAngles = new Vector3(1, deCoordenadasAPotenciaAngulo(xKNewton, yKNewton)[1] * -1, 0.006f);
                    break;
                case 3:
                    canyon.transform.eulerAngles = new Vector3(1, deCoordenadasAPotenciaAngulo(xKCombinado, yKCombinado)[1] * -1, 0.006f);
                    break;
                default:
                    break;
            }
        }
        

        if((esfera.transform.position.y<=0 || seHaLanzadoSinAnimacion) && !yaMedido && !juegoFinalizado) {
            if(jugadorActual==0 && aQuienTocaLanzar==0) {
                posicionX = (((potenciaLanzamientoActual / multiplicadorPotencia) * (potenciaLanzamientoActual / multiplicadorPotencia)) / gravedad) * Mathf.Cos(anguloLanzamientoActual * Mathf.Deg2Rad);
                posicionY = (((potenciaLanzamientoActual / multiplicadorPotencia) * (potenciaLanzamientoActual / multiplicadorPotencia)) / gravedad) * Mathf.Sin(anguloLanzamientoActual * Mathf.Deg2Rad);

                string puntos = funcionPrincipal(posicionX, posicionY, objetivos).ToString("n");

                float[] gradienteEvaluado = gradiente(posicionX, posicionY, objetivos);

                //float anguloGradiente = calcularArctTan(-gradienteEvaluado[0], -gradienteEvaluado[1]);

                //Colocar los marcadores del jugador dependiendo de lo que tiene seleccionado
                if(tipoIndicadorJugador==3) {
                    string[] textosExtra = new string[9];
                    for(int i = 0; i < textosExtra.Length; i++) {
                        textosExtra[i] = "";
                    }
                    colocarMarcadorYCercanosFlecha(posicionX, posicionY, textosExtra, escalaGradienteFlecha, colorMarcadoresJugador, colorFlechasJugador, 0.3f);
                } else if(tipoIndicadorJugador==1) {
                    colocarMarcadorConFlecha(posicionX, posicionY, puntos, -gradienteEvaluado[0], -gradienteEvaluado[1], escalaGradienteFlecha, colorMarcadoresJugador, colorFlechasJugador);
                } else if(tipoIndicadorJugador==2) {
                    string[] textosExtra = new string[9];
                    for(int i = 0; i < textosExtra.Length; i++) {
                        textosExtra[i] = "";
                    }

                    colocarMarcadorYCercanos(posicionX, posicionY, textosExtra, colorMarcadoresJugador, 0.3f);
                } else {
                    colocarMarcador(posicionX, posicionY, puntos, colorMarcadoresJugador);
                }

                /*if(conGradientDescent) {
                    float[] xK2 = iterarGradientDescent(xKGradiente, yKGradiente, 0.1f, objetivos);
                    float[] gradienteEvaluadoGD = gradiente(xKGradiente, yKGradiente, objetivos);
                    xKGradiente = xK2[0];
                    yKGradiente = xK2[1];
                    colocarMarcadorConFlecha(xKGradiente, yKGradiente, funcionPrincipal(xKGradiente, yKGradiente, objetivos)+"", -gradienteEvaluadoGD[0], -gradienteEvaluadoGD[1], escalaGradienteFlecha, colorMarcadoresGradient, colorFlechasGradient);
                    Debug.Log("Gradient descent X:" + xKGradiente + " Y:" + yKGradiente);
                }*/
                
                if(rivalActual==1 || rivalActual == 3) {
                    aQuienTocaLanzar=1;
                    StopAllCoroutines();
                    esfera.transform.position=new Vector3(0, 0.0001f, 0);
                } else if(rivalActual==2) {
                    aQuienTocaLanzar=2;
                    StopAllCoroutines();
                    esfera.transform.position = new Vector3(0, 0.0001f, 0);
                }

                //Debug.Log(posicionX + " " + posicionY + " Puntos:" + puntos + " GradienteX:" + (-gradienteEvaluado[0]) + " GradienteY:" + (-gradienteEvaluado[1]) + " Angulo:" + anguloGradiente);
            } else if(jugadorActual==1) {

                float[] gradienteEvaluado = gradiente(xKGradiente, yKGradiente, objetivos);
                if(GDUsaFlecha) {
                    colocarMarcadorConFlecha(xKGradiente, yKGradiente, funcionPrincipal(xKGradiente, yKGradiente, objetivos).ToString("n"), -gradienteEvaluado[0], -gradienteEvaluado[1], escalaGradienteFlecha, colorMarcadoresGradient, colorFlechasGradient);
                } else {
                    string[] textosExtra = new string[9];
                    for(int i = 0; i < textosExtra.Length; i++) {
                        textosExtra[i] = "";
                    }

                    colocarMarcadorYCercanos(xKGradiente, yKGradiente, textosExtra, colorMarcadoresGradient, 0.3f);
                }

                
                //Hacer la siguiente iteracion
                float[] xK2 = iterarGradientDescent(xKGradiente, yKGradiente, 0.1f, objetivos);
                xKGradiente = xK2[0];
                yKGradiente = xK2[1];

                if(rivalActual==3) {
                    aQuienTocaLanzar=2;
                    StopAllCoroutines();
                    esfera.transform.position = new Vector3(0, 0.0001f, 0);
                } else {
                    aQuienTocaLanzar=0;
                }

            } else if(jugadorActual == 2) {

                string[] textosExtra=new string[9];
                for(int i=0; i< textosExtra.Length; i++) {
                    textosExtra[i]="";
                }
                colocarMarcadorYCercanosFlecha(xKNewton, yKNewton, textosExtra, escalaNewtonFlecha, colorMarcadoresNewton, colorFlechasNewton, 0.3f);

                //Hacer la siguiente iteracion
                float[] xK2 = iterarNewtonMethod(xKNewton, yKNewton, objetivos);
                xKNewton = xK2[0];
                yKNewton = xK2[1];
            } else if(jugadorActual == 3) {
                if(!estadoMetodoCombinado){
                    // Modo GD
                    float[] gradienteEvaluado = gradiente(xKCombinado, yKCombinado, objetivos);
                    if(GDUsaFlecha) {
                        colocarMarcadorConFlecha(xKCombinado, yKCombinado, funcionPrincipal(xKCombinado, yKCombinado, objetivos).ToString("n"), -gradienteEvaluado[0], -gradienteEvaluado[1], escalaGradienteFlecha, colorMarcadoresMetodoCombinado, colorFlechasMetodoCombinado);
                    } else {
                        string[] textosExtra = new string[9];
                        for(int i = 0; i < textosExtra.Length; i++) {
                            textosExtra[i] = "";
                        }

                        colocarMarcadorYCercanos(xKCombinado, yKCombinado, textosExtra, colorMarcadoresMetodoCombinado, 0.3f);
                    }

                    //Ver si es mejor cambiar a NM
                    //Iterar GD
                    float[] gradienteEvaluadoGD = gradiente(xKCombinado, yKCombinado, objetivos);

                    float xKGradiente2 = xKCombinado - 0.1f* gradienteEvaluadoGD[0];
                    float yKGradiente2 = yKCombinado - 0.1f* gradienteEvaluadoGD[1];

                    //Iterar NM
                    float[] xK2=siguienteIteracionNewton(xKCombinado, yKCombinado, objetivos);

                    Debug.Log("Puntos de GD:"+ funcionPrincipal(xKGradiente2, yKGradiente2, objetivos)+" Puntos de NM:"+ funcionPrincipal(xK2[0], xK2[1], objetivos));
                    if(funcionPrincipal(xKGradiente2, yKGradiente2, objetivos) < funcionPrincipal(xK2[0], xK2[1], objetivos)) {
                        xKCombinado=xK2[0];
                        yKCombinado=xK2[1];
                        estadoMetodoCombinado=true;
                    } else {
                        // Si no hay casi puntos o se ha salido del mapa, volver a empezar
                        if((funcionPrincipal(xKGradiente2, yKGradiente2, objetivos) < 0.001f) || (xKGradiente2 > 10 || xKGradiente2 < -10) || (yKGradiente2 > 10 || yKGradiente2 < -10)) {
                            xKGradiente2 = Random.Range((float)-10, 10);
                            yKGradiente2 = Random.Range((float)-10, 10);
                            Debug.Log("ReseteandoGD:" + xKGradiente2 + " " + yKGradiente2);
                        }
                        Debug.Log("Valor:" + funcionPrincipal(xKGradiente2, yKGradiente2, objetivos));
                        xKCombinado=xKGradiente2;
                        yKCombinado=yKGradiente2;
                    }

                } else {
                    // Modo NM
                    string[] textosExtra = new string[9];
                    for(int i = 0; i < textosExtra.Length; i++) {
                        textosExtra[i] = "";
                    }
                    colocarMarcadorYCercanosFlecha(xKCombinado, yKCombinado, textosExtra, escalaNewtonFlecha, colorMarcadoresMetodoCombinado, colorFlechasMetodoCombinado, 0.3f);

                    //Hacer la siguiente iteracion
                    float[] xK2 = iterarNewtonMethod(xKCombinado, yKCombinado, objetivos);
                    xKCombinado = xK2[0];
                    yKCombinado = xK2[1];
                }
            }

            if(aQuienTocaLanzar==0) yaMedido=true;
            
        }

        if(jugadorActual==3 && !yaMedido) {
            indicadorCanyon.GetComponent<MeshRenderer>().material.color=colorMarcadoresMetodoCombinado;
        } else if(jugadorActual==2 && !yaMedido) {
            indicadorCanyon.GetComponent<MeshRenderer>().material.color=colorMarcadoresNewton;
        } else if(jugadorActual==1 && !yaMedido) {
            indicadorCanyon.GetComponent<MeshRenderer>().material.color=colorMarcadoresGradient;
        } else {
            indicadorCanyon.GetComponent<MeshRenderer>().material.color=colorMarcadoresJugador;
        }
        

        // Actualizacion de UI
        textoPotencia.GetComponent<Text>().text="Potència: "+potencia;
        textoAngulo.GetComponent<Text>().text="Angle: "+angulo;

        float[] valoresCalculadosGradientTexto=deCoordenadasAPotenciaAngulo(xKGradiente, yKGradiente);
        textoPotenciaGradiente.GetComponent<Text>().text="Potència: "+valoresCalculadosGradientTexto[0];
        textoAnguloGradiente.GetComponent<Text>().text="Angle: "+valoresCalculadosGradientTexto[1];
        float[] valoresCalculadosNewtonTexto = deCoordenadasAPotenciaAngulo(xKNewton, yKNewton);
        textoPotenciaNewton.GetComponent<Text>().text="Potència: "+valoresCalculadosNewtonTexto[0] + "";
        textoAnguloNewton.GetComponent<Text>().text="Angle: "+valoresCalculadosNewtonTexto[1] + "";
        float[] valoresCalculadosCombinadoTexto = deCoordenadasAPotenciaAngulo(xKCombinado, yKCombinado);
        textoPotenciaCombinado.GetComponent<Text>().text = "Potència: " + valoresCalculadosCombinadoTexto[0] + "";
        textoAnguloCombinado.GetComponent<Text>().text = "Angle: " + valoresCalculadosCombinadoTexto[1] + "";


        if(rivalActual == 0) {
            botonEscogerRival.transform.GetChild(0).GetComponent<Text>().text = "No jugant contra ningú";
        } else if(rivalActual == 1) {
            botonEscogerRival.transform.GetChild(0).GetComponent<Text>().text = "Jugant contra descens del gradient";
        } else if(rivalActual==2){
            botonEscogerRival.transform.GetChild(0).GetComponent<Text>().text = "Jugant contra mètode de Newton";
        } else {
            botonEscogerRival.transform.GetChild(0).GetComponent<Text>().text = "Jugant contra tots dos mètodes";
        }

        if(GDUsaFlecha) {
            botonEscogerModoGradiente.transform.GetChild(0).GetComponent<Text>().text = "Fletxa per mostrar el gradient";
        } else {
            botonEscogerModoGradiente.transform.GetChild(0).GetComponent<Text>().text = "Més marcadors per mostrar el gradient";
        }
        // 0-> Indicador normal  1-> Indicador con flecha  2-> Indicadores cercanos  3-> Indicadores cercanos con flecha
        if(tipoIndicadorJugador == 0) {
            botonEscogerIndicadorJugador.transform.GetChild(0).GetComponent<Text>().text = "Marcador normal";
        } else if(tipoIndicadorJugador == 1) {
            botonEscogerIndicadorJugador.transform.GetChild(0).GetComponent<Text>().text = "Marcador amb fletxa";
        } else if(tipoIndicadorJugador == 2) {
            botonEscogerIndicadorJugador.transform.GetChild(0).GetComponent<Text>().text = "Marcadors propers";
        } else if(tipoIndicadorJugador == 3) {
            botonEscogerIndicadorJugador.transform.GetChild(0).GetComponent<Text>().text = "Marcadors propers amb fletxa";
        }

        if(animacionEsferaActivada) {
            botonEscogerAnimacion.transform.GetChild(0).GetComponent<Text>().text = "Animació de la pilota activada";
        } else {
            botonEscogerAnimacion.transform.GetChild(0).GetComponent<Text>().text = "Animació de la pilota desactivada";
        }

    }

    /*float[] simulacionMetodos(int iteracionesMaximasArg, float alphaGDArg, float toleranciaArg, bool mostrarDebug=false) {
        
        bool estadoMetodoCombinadoS=false;
        float xKGradienteS=Random.Range((float)-10, 10);
        float yKGradienteS=Random.Range((float)-10, 10);
        float xKNewtonS=Random.Range((float)-10, 10);
        float yKNewtonS=Random.Range((float)-10, 10);
        float xKCombinadoS=Random.Range((float)-10, 10);
        float yKCombinadoS=Random.Range((float)-10, 10);

        int iteracionesGD=iteracionesMaximasArg+1;
        int iteracionesNM=iteracionesMaximasArg+1;
        int iteracionesMC=iteracionesMaximasArg+1;

        Objetivo[] objetivosS=reinicializarObjetivos(3, mostrarDebug);

        for(int i=0; i<iteracionesMaximasArg; i++) {
            // Gradient descent
            float[] xK2G = iterarGradientDescent(xKGradienteS, yKGradienteS, alphaGDArg, objetivosS, mostrarDebug);
            xKGradienteS = xK2G[0];
            yKGradienteS = xK2G[1];

            // Parar si ya está cerca de un objetivo
            bool condicion=false;
            for(int j=0; j<objetivosS.Length; j++) {
                if(Mathf.Abs(objetivosS[j].a-xKGradienteS)< toleranciaArg && Mathf.Abs(objetivosS[j].b-yKGradienteS)<toleranciaArg) {
                    iteracionesGD=i;
                    condicion=true;
                    if(mostrarDebug) if(mostrarDebug) Debug.Log("GD ha llegado a:"+xKGradienteS+" "+yKGradienteS+" El objetivo conseguido es:"+objetivosS[j].a+" "+objetivosS[j].b);
                    break;
                }
            }
            if(condicion) break;
        }
        if(mostrarDebug) Debug.Log("Empezando NM");
        for(int i=0; i<iteracionesMaximasArg; i++) {
            // Newton method
            float[] xK2N = iterarNewtonMethod(xKNewtonS, yKNewtonS, objetivosS, mostrarDebug);
            xKNewtonS = xK2N[0];
            yKNewtonS = xK2N[1];

            // Parar si ya está cerca de un objetivo
            bool condicion=false;
            for(int j=0; j<objetivosS.Length; j++) {
                if(Mathf.Abs(objetivosS[j].a-xKNewtonS)<toleranciaArg && Mathf.Abs(objetivosS[j].b-yKNewtonS)<toleranciaArg) {
                    iteracionesNM=i;
                    condicion=true;
                    if(mostrarDebug) Debug.Log("NM ha llegado a:"+xKNewtonS+" "+yKNewtonS+" El objetivo conseguido es:"+objetivosS[j].a+" "+objetivosS[j].b);
                    break;
                }
            }
            if(condicion) break;
        }
        if(mostrarDebug) Debug.Log("Empezando MC");
        for(int i=0; i<iteracionesMaximasArg; i++) {
            // Metodo combinado
            if(!estadoMetodoCombinadoS) {
                // Modo GD
                //Ver si es mejor cambiar a NM
                //Iterar GD
                float[] gradienteEvaluadoGD = gradiente(xKCombinadoS, yKCombinadoS, objetivosS);
                float xKGradiente2 = xKCombinadoS - alphaGDSimulacion * gradienteEvaluadoGD[0];
                float yKGradiente2 = yKCombinadoS - alphaGDSimulacion * gradienteEvaluadoGD[1];

                //Iterar NM
                float[] xK2 = siguienteIteracionNewton(xKCombinadoS, yKCombinadoS, objetivosS);

                if(mostrarDebug) Debug.Log("Valor (MC GD):" + funcionPrincipal(xKGradiente2, yKGradiente2, objetivosS));

                if(funcionPrincipal(xKGradiente2, yKGradiente2, objetivosS) < funcionPrincipal(xK2[0], xK2[1], objetivosS)) {
                    xKCombinadoS = xK2[0];
                    yKCombinadoS = xK2[1];
                    estadoMetodoCombinadoS = true;
                    if(mostrarDebug) Debug.Log("GDCambioANM:"+funcionPrincipal(xKGradiente2, yKGradiente2, objetivosS)+" Coordenadas GD:"+xKGradiente2+" "+yKGradiente2+" Puntos NM:"+funcionPrincipal(xK2[0], xK2[1], objetivosS)+" Coordenadas NM:"+ xK2[0]+" "+xK2[1]);
                } else {
                    // Si no hay casi puntos o se ha salido del mapa, volver a empezar
                    if((funcionPrincipal(xKGradiente2, yKGradiente2, objetivosS) < 0.001f) || (xKGradiente2 > 10 || xKGradiente2 < -10) || (yKGradiente2 > 10 || yKGradiente2 < -10)) {
                        if(mostrarDebug) Debug.Log("Reseteando MC GD");
                        xKGradiente2 = Random.Range((float)-10, 10);
                        yKGradiente2 = Random.Range((float)-10, 10);
                    }
                    xKCombinadoS = xKGradiente2;
                    yKCombinadoS = yKGradiente2;

                }
            } else {
                // Modo NM
                //Hacer la siguiente iteracion
                float[] xK2 = iterarNewtonMethod(xKCombinadoS, yKCombinadoS, objetivosS, mostrarDebug);
                xKCombinadoS = xK2[0];
                yKCombinadoS = xK2[1];
                if(mostrarDebug) Debug.Log("Coordenadas MC NM:"+ xKCombinadoS+" "+yKCombinadoS);
            }

            // Parar si ya está cerca de un objetivo
            bool condicion=false;
            for(int j=0; j<objetivosS.Length; j++) {
                if(Mathf.Abs(objetivosS[j].a-xKCombinadoS)<toleranciaArg && Mathf.Abs(objetivosS[j].b-yKCombinadoS)<toleranciaArg) {
                    iteracionesMC=i;
                    condicion=true;
                    if(mostrarDebug) Debug.Log("MC ha llegado a:" + xKCombinadoS + " " + yKCombinadoS + " El objetivo conseguido es:" + objetivosS[j].a + " " + objetivosS[j].b);
                    break;
                }
            }
            if(condicion) break;
        }
        string textoParaImprimir="IteracionesGD:"+iteracionesGD+" IteracionesNM:"+iteracionesNM+" IteracionesMC:"+iteracionesMC;
        for(int i=0; i<objetivosS.Length; i++) {
            textoParaImprimir+=" Objetivo "+i+":"+objetivosS[i].a+" "+objetivosS[i].b;
        }
        if(mostrarDebug) Debug.Log(textoParaImprimir);

        float[] resultado = { iteracionesGD, iteracionesNM, iteracionesMC };
        return resultado;
    }*/

    IEnumerator animacionEsfera(GameObject objeto, float potenciaArg, float anguloHorizontal) {
        float t=0;
        anguloHorizontal=anguloHorizontal*Mathf.Deg2Rad;
        potenciaArg=(float)potenciaArg/multiplicadorPotencia;
        objeto.SetActive(true);

        for(int i=0; i<1000; i++) {
            t=(float)i/50;
            float posX=t*potenciaArg*Mathf.Sqrt(2)*0.5f;
            float posY=t*potenciaArg*Mathf.Sqrt(2)*0.5f -0.5f*gravedad*t*t +0.00001f;
            objeto.transform.position=new Vector3(posX*Mathf.Cos(anguloHorizontal), posY, posX*Mathf.Sin(anguloHorizontal));
            if(posY<-0.3f) break;
            yield return new WaitForSecondsRealtime(0.01f);
        }
        //objeto.SetActive(false);
    }

    float funcionPrincipal(float x, float y, Objetivo[] objs) {
        float resultado=0;
        for(int i=0; i<objs.Length; i++) {
            resultado=resultado + 6.78f*Mathf.Log10(1+Mathf.Pow(1.405f, -((x-objs[i].a)*(x-objs[i].a) + (y-objs[i].b)*(y-objs[i].b) -objs[i].c)));
        }
        return resultado;
    }

    float[] gradiente(float x, float y, Objetivo[] objs) {

        float resultado1=0;
        float resultado2=0;
        for(int i=0; i<objs.Length; i++) {
            float a=objs[i].a;
            float b=objs[i].b;
            float c=objs[i].c;

            resultado1=resultado1+ ((-6.78f*Mathf.Log(1.405f)*(-2*x+2*a)*Mathf.Pow(1.405f, (-x*x -y*y -a*a -b*b +2*a*x +2*b*y +c)))
                /(Mathf.Log(10)*(1+ Mathf.Pow(1.405f, (-x*x -y*y -a*a -b*b +2*a*x +2*b*y +c)))));

            resultado2=resultado2+ ((-6.78f*Mathf.Log(1.405f)*(-2*y+2*b)*Mathf.Pow(1.405f, (-x*x -y*y -a*a -b*b +2*a*x +2*b*y +c)))
                /(Mathf.Log(10)*(1+ Mathf.Pow(1.405f, (-x*x -y*y -a*a -b*b +2*a*x +2*b*y +c)))));

        }
        float[] gradienteFinal={ resultado1, resultado2 };
        return gradienteFinal;

    }

    float[] hessian(float x, float y, Objetivo[] objs) {

        float funcionU=0;
        float funcionV=0;
        float funcionW=0;

        float derivadaUx=0;
        float derivadaUy=0; //derivadaUy=derivadaWx
        float derivadaVx=0;
        float derivadaVy=0;
        float derivadaWy=0;

        for(int i=0; i<objs.Length; i++) {
            float a=objs[i].a;
            float b=objs[i].b;
            float c=objs[i].c;

            funcionU = (-6.78f*Mathf.Log(1.405f)*(-2*x+2*a)*Mathf.Pow(1.405f, (-x*x -y*y -a*a -b*b +2*a*x +2*b*y +c)));
            funcionV = (Mathf.Log(10)*(1+ Mathf.Pow(1.405f, (-x*x -y*y -a*a -b*b +2*a*x +2*b*y +c))));
            funcionW = (-6.78f*Mathf.Log(1.405f)*(-2*y+2*b)*Mathf.Pow(1.405f, (-x*x -y*y -a*a -b*b +2*a*x +2*b*y +c)));

            derivadaUx += 6.78f*Mathf.Log(1.405f)*(2*Mathf.Pow(1.405f, (-x*x -y*y -a*a -b*b +2*a*x +2*b*y +c)) -((-2*x +2*a)*(-2*x +2*a)*Mathf.Log(1.405f)*Mathf.Pow(1.405f, (-x*x -y*y -a*a -b*b +2*a*x +2*b*y +c))));
            derivadaUy += -6.78f*Mathf.Log(1.405f)*Mathf.Log(1.405f)*(-2*x +2*a)*(-2*y +2*b)*Mathf.Pow(1.405f, (-x*x -y*y -a*a -b*b +2*a*x +2*b*y +c));

            derivadaVx += Mathf.Log(10)*Mathf.Log(1.405f)*(-2*x +2*a)*Mathf.Pow(1.405f, (-x*x -y*y -a*a -b*b +2*a*x +2*b*y +c));
            derivadaVy += Mathf.Log(10)*Mathf.Log(1.405f)*(-2*y +2*b)*Mathf.Pow(1.405f, (-x*x -y*y -a*a -b*b +2*a*x +2*b*y +c));

            derivadaWy += 6.78f*Mathf.Log(1.405f)*(2*Mathf.Pow(1.405f, (-x*x -y*y -a*a -b*b +2*a*x +2*b*y +c)) -((-2*y +2*b)*(-2*y +2*b)*Mathf.Log(1.405f)*Mathf.Pow(1.405f, (-x*x -y*y -a*a -b*b +2*a*x +2*b*y +c))));
        }

        float resultado1 = ((derivadaUx*funcionV)-(funcionU*derivadaVx))/(funcionV*funcionV);
        float resultado2 = ((derivadaUy*funcionV)-(funcionU*derivadaVy))/(funcionV*funcionV);
        float resultado3 = ((derivadaUy*funcionV)-(funcionW*derivadaVx))/(funcionV*funcionV);
        float resultado4 = ((derivadaWy*funcionV)-(funcionW*derivadaVy))/(funcionV*funcionV);

        float[] hessianFinal = {resultado1, resultado2, resultado3, resultado4};
        return hessianFinal;
    }

    float[] iterarGradientDescent(float xK, float yK, float alpha, Objetivo[] objs, bool mostrarDebug=false) {
        float[] gradienteEvaluado=gradiente(xK, yK, objs);

        float xK2 = xK -alpha*gradienteEvaluado[0];
        float yK2 = yK -alpha*gradienteEvaluado[1];

        // Si no hay casi puntos o se ha salido del mapa, volver a empezar
        if((funcionPrincipal(xK2, yK2, objs)<0.001f) || (xK2>10 || xK2<-10) || (yK2>10 || yK2<-10)) {
            xK2=Random.Range((float)-10, 10);
            yK2=Random.Range((float)-10, 10);
            if(mostrarDebug) Debug.Log("ReseteandoGD:"+ xK2+" "+yK2);
        }
        if(mostrarDebug) Debug.Log("Valor:"+ funcionPrincipal(xK2, yK2, objs));

        float[] resultado = {xK2, yK2};
        return resultado;
    }

    float[] iterarNewtonMethod(float xK, float yK, Objetivo[] objs, bool mostrarDebug=false) {
        float[] xK2= siguienteIteracionNewton(xK, yK, objs);

        if(mostrarDebug) Debug.Log("Valor (NM):"+ funcionPrincipal(xK2[0], xK2[1], objs));
        // Si no hay casi puntos o se ha salido del mapa, volver a empezar
        if((funcionPrincipal(xK2[0], xK2[1], objs)<0.001f) || (xK2[0]>10 || xK2[0]<-10) || (xK2[1]>10 || xK2[1]<-10)) {
            xK2[0]=Random.Range((float)-10, 10);
            xK2[1]=Random.Range((float)-10, 10);
            if(mostrarDebug) Debug.Log("ReseteandoNM:" + xK2[0] + " " + xK2[1]);
        }

        float[] resultado = {xK2[0], xK2[1]};
        return resultado;
    }

    float[] siguienteIteracionNewton(float xK, float yK, Objetivo[] objs) {
        float[] gradienteEvaluado = gradiente(xK, yK, objs);
        float[] hessianEvaluado = hessian(xK, yK, objs);

        float determinante = hessianEvaluado[0] * hessianEvaluado[3] - hessianEvaluado[1] * hessianEvaluado[2];

        if(determinante == 0) {
            float[] p = { xK, yK };
            return p;
        }
        float hessian1 = hessianEvaluado[3] * (1 / determinante);
        float hessian2 = -hessianEvaluado[1] * (1 / determinante);
        float hessian3 = -hessianEvaluado[2] * (1 / determinante);
        float hessian4 = hessianEvaluado[0] * (1 / determinante);

        float vectorFinal1 = (hessian1 * gradienteEvaluado[0]) + (hessian2 * gradienteEvaluado[1]);
        float vectorFinal2 = (hessian3 * gradienteEvaluado[0]) + (hessian4 * gradienteEvaluado[1]);

        float xK2 = xK - vectorFinal1;
        float yK2 = yK - vectorFinal2;

        float[] resultado = { xK2, yK2 };
        return resultado;
    }

    void colocarMarcador(float x, float y, string texto, Color colorDelMarcador) {
        GameObject.Instantiate(marcador, new Vector3(x, ((float)(indiceMarcador + 1) / 100000), y), Quaternion.Euler(0, 0, 0), contenedorMarcadores.transform).SetActive(true);

        
        Color.RGBToHSV(colorDelMarcador, out float hue, out float saturation, out float vue);
        //Debug.Log("Saturacion:"+(saturation*funcionPrincipal(x, y, objetivos))/10 + " ContenidoTexto:"+ limitarTexto(texto, 5)+" ContenidoFuncionPrincipal:"+ funcionPrincipal(x, y, objetivos)+" Texto:"+texto);

        contenedorMarcadores.transform.GetChild(indiceMarcador).GetChild(2).gameObject.SetActive(false);
        contenedorMarcadores.transform.GetChild(indiceMarcador).GetChild(1).GetComponent<SpriteRenderer>().color=Color.HSVToRGB(hue, (saturation * funcionPrincipal(x, y, objetivos)) / 10, vue);

        contenedorMarcadores.transform.GetChild(indiceMarcador).gameObject.name = "Marcador " + indiceMarcador;
        Transform textoDelMarcador=contenedorMarcadores.transform.GetChild(indiceMarcador).GetChild(0);
        textoDelMarcador.gameObject.GetComponent<TextMeshPro>().text = limitarTexto(texto, 5);

        //Color del anillo
        contenedorMarcadores.transform.GetChild(indiceMarcador).GetChild(3).GetComponent<SpriteRenderer>().color=colorDelMarcador;

        //Evitar que el texto se coloque siempre encima
        contenedorMarcadores.transform.GetChild(indiceMarcador).GetChild(1).GetComponent<SpriteRenderer>().sortingOrder=indiceMarcador*4;
        contenedorMarcadores.transform.GetChild(indiceMarcador).GetChild(3).GetComponent<SpriteRenderer>().sortingOrder=indiceMarcador*4+1;
        textoDelMarcador.GetComponent<TextMeshPro>().sortingOrder=indiceMarcador*4+3;

        indiceMarcador++;
    }

    void colocarMarcadorFinal(float x, float y, string texto, Color colorDelMarcador) {
        GameObject.Instantiate(marcadorFinal, new Vector3(x, ((float)(indiceMarcador + 1) / 100000), y), Quaternion.Euler(0, 0, 0), contenedorMarcadores.transform).SetActive(true);

        Color.RGBToHSV(colorDelMarcador, out float hue, out float saturation, out float vue);
        
        contenedorMarcadores.transform.GetChild(indiceMarcador).GetChild(2).gameObject.SetActive(false);
        contenedorMarcadores.transform.GetChild(indiceMarcador).GetChild(1).GetComponent<SpriteRenderer>().color = Color.HSVToRGB(hue, (saturation * funcionPrincipal(x, y, objetivos)) / 10, vue);

        contenedorMarcadores.transform.GetChild(indiceMarcador).gameObject.name = "Marcador " + indiceMarcador;
        Transform textoDelMarcador = contenedorMarcadores.transform.GetChild(indiceMarcador).GetChild(0);
        textoDelMarcador.gameObject.GetComponent<TextMeshPro>().text = limitarTexto(texto, 5);

        //Evitar que el texto se coloque siempre encima
        contenedorMarcadores.transform.GetChild(indiceMarcador).GetChild(1).GetComponent<SpriteRenderer>().sortingOrder=indiceMarcador*4;
        textoDelMarcador.GetComponent<TextMeshPro>().sortingOrder = indiceMarcador*4+2;

        indiceMarcador++;
    }

    void colocarMarcadorConFlecha(float x, float y, string texto, float magnitudX, float magnitudY, float escalaFlecha, Color colorDelMarcador, Color colorFlecha) {
        GameObject.Instantiate(marcador, new Vector3(x, ((float)(indiceMarcador + 1) / 100000), y), Quaternion.Euler(0, 0, 0), contenedorMarcadores.transform).SetActive(true);

        Color.RGBToHSV(colorDelMarcador, out float hue, out float saturation, out float vue);
        contenedorMarcadores.transform.GetChild(indiceMarcador).GetChild(1).GetComponent<SpriteRenderer>().color=Color.HSVToRGB(hue, (saturation * funcionPrincipal(x, y, objetivos)) / 10, vue);

        GameObject flechaActual = contenedorMarcadores.transform.GetChild(indiceMarcador).GetChild(2).GetChild(0).gameObject;
        flechaActual.GetComponent<MeshRenderer>().material.color = colorFlecha;
        
        if(float.IsNaN(-calcularArctTan(magnitudX, magnitudY))) {
            flechaActual.transform.parent.rotation = Quaternion.Euler(0, 0, 0);
            escalaFlecha=0;
        } else {
            flechaActual.transform.parent.rotation = Quaternion.Euler(0, -calcularArctTan(magnitudX, magnitudY), 0);
        }

        float escalaParaFlecha=escalaFlecha*Mathf.Sqrt(magnitudX*magnitudX + magnitudY*magnitudY);
        flechaActual.transform.parent.localScale=new Vector3(escalaParaFlecha, escalaParaFlecha, escalaParaFlecha/4);

        contenedorMarcadores.transform.GetChild(indiceMarcador).gameObject.name = "Marcador " + indiceMarcador;
        Transform textoDelMarcador = contenedorMarcadores.transform.GetChild(indiceMarcador).GetChild(0);
        
        textoDelMarcador.gameObject.GetComponent<TextMeshPro>().text = limitarTexto(texto, 5);

        //Color del anillo
        contenedorMarcadores.transform.GetChild(indiceMarcador).GetChild(3).GetComponent<SpriteRenderer>().color=colorDelMarcador;

        //Evitar que el texto se coloque siempre encima
        contenedorMarcadores.transform.GetChild(indiceMarcador).GetChild(1).GetComponent<SpriteRenderer>().sortingOrder=indiceMarcador*4;
        contenedorMarcadores.transform.GetChild(indiceMarcador).GetChild(3).GetComponent<SpriteRenderer>().sortingOrder=indiceMarcador*4+1;
        contenedorMarcadores.transform.GetChild(indiceMarcador).GetChild(2).GetChild(0).GetComponent<SortingGroup>().sortingOrder=indiceMarcador*4+2;
        textoDelMarcador.GetComponent<TextMeshPro>().sortingOrder=indiceMarcador*4+3;

        indiceMarcador++;
    }

    void colocarMarcadorYCercanos(float x, float y, string[] textosMarcadores, Color colorDelMarcador, float radioMarcadoresCercanos) {
        // textosMarcadores es una lista que contiene los textos que se tienen que colocar en los marcadores extras, el primer elemento es el marcador central
        colocarMarcador(x, y, textosMarcadores[0]+funcionPrincipal(x, y, objetivos).ToString("n"), colorDelMarcador);
        for(int i=1; i<textosMarcadores.Length; i++) {
            float posicionMarcadorExteriorX=x+radioMarcadoresCercanos*Mathf.Cos(map(i, 1, textosMarcadores.Length, 0, 360)*Mathf.Deg2Rad);
            float posicionMarcadorExteriorY=y+radioMarcadoresCercanos*Mathf.Sin(map(i, 1, textosMarcadores.Length, 0, 360)*Mathf.Deg2Rad);
            colocarMarcador(posicionMarcadorExteriorX, posicionMarcadorExteriorY, textosMarcadores[i]+funcionPrincipal(posicionMarcadorExteriorX, posicionMarcadorExteriorY, objetivos).ToString("n"), colorDelMarcador);
        }
    }

    void colocarMarcadorYCercanosFlecha(float x, float y, string[] textosMarcadores, float escalaFlechas, Color colorDelMarcador, Color colorDeFlechas, float radioMarcadoresCercanos) {
        // textosMarcadores es una lista que contiene los textos que se tienen que colocar en los marcadores extras, el primer elemento es el marcador central

        float[] gradienteEvaluado = gradiente(x, y, objetivos);

        colocarMarcadorConFlecha(x, y, textosMarcadores[0]+funcionPrincipal(x, y, objetivos).ToString("n"), -gradienteEvaluado[0], -gradienteEvaluado[1], escalaFlechas, colorDelMarcador, colorDeFlechas);


        for(int i = 1; i < textosMarcadores.Length; i++) {
            float posicionMarcadorExteriorX = x + radioMarcadoresCercanos * Mathf.Cos(map(i, 1, textosMarcadores.Length, 0, 360) * Mathf.Deg2Rad);
            float posicionMarcadorExteriorY = y + radioMarcadoresCercanos * Mathf.Sin(map(i, 1, textosMarcadores.Length, 0, 360) * Mathf.Deg2Rad);

            gradienteEvaluado = gradiente(posicionMarcadorExteriorX, posicionMarcadorExteriorY, objetivos);

            colocarMarcadorConFlecha(posicionMarcadorExteriorX, posicionMarcadorExteriorY, textosMarcadores[i]+funcionPrincipal(posicionMarcadorExteriorX, posicionMarcadorExteriorY, objetivos).ToString("n"), -gradienteEvaluado[0], -gradienteEvaluado[1], escalaFlechas, colorDelMarcador, colorDeFlechas);
        }
    }

    void lanzarEsfera(float potenciaArg, float anguloArg, Color colorParaEsfera) {
        esfera.transform.position = new Vector3(0, 0.001f, 0);
        esfera.GetComponent<MeshRenderer>().material.color=colorParaEsfera;

        StartCoroutine(animacionEsfera(esfera, potenciaArg, anguloArg));

        yaMedido = false;
        //posicionX = 0;
        //posicionY = 0;
    }

    Objetivo[] reinicializarObjetivos(int maximosObjetivosArg, bool mostrarDebug=true) {
        int numeroAleatorio = Random.Range(1, maximosObjetivosArg + 1);

        Objetivo[] resultado = new Objetivo[numeroAleatorio];

        resultado[0] = new Objetivo(Random.Range((float)-10, 10), Random.Range((float)-10, 10), 10);
        if(mostrarDebug) Debug.Log("Objetivo 0:  A:"+resultado[0].a+" B:"+resultado[0].b+" C:"+resultado[0].c);
        for(int i=1; i<(numeroAleatorio); i++) {
            resultado[i] = new Objetivo(Random.Range((float)-10, 10), Random.Range((float)-10, 10), Random.Range(1, 9));
            if(mostrarDebug) Debug.Log("Objetivo "+i+":  A:"+resultado[i].a+" B:"+resultado[i].b+" C:"+resultado[i].c);
        }
        return resultado;
    }

    float calcularArctTan(float ladoX, float ladoY) {
        float numero=Mathf.Atan(ladoY / ladoX) * Mathf.Rad2Deg;
        if(ladoX < 0) {
            numero += 180;
        }
        if(numero < 0) {
            numero+=360;
        }
        return numero;
    }

    float[] deCoordenadasAPotenciaAngulo(float posX, float posY) {
        float potenciaCalculada = multiplicadorPotencia * Mathf.Sqrt(gravedad * Mathf.Sqrt(posX * posX + posY * posY));
        float anguloCalculado = calcularArctTan(posX, posY);
        float[] resultado= { potenciaCalculada, anguloCalculado };
        return resultado;
    }


    float map(float s, float a1, float a2, float b1, float b2) {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    string limitarTexto(string textoArg, int longitudTexto) {
        if(textoArg.Length > longitudTexto) {
            return textoArg.Substring(0, longitudTexto);
        } else {
            return textoArg;
        }
    }

}

class Objetivo {
    public float a;
    public float b;
    public float c;

    public Objetivo(float parametroA, float parametroB, float parametroC) {
        a=parametroA;
        b=parametroB;
        c=parametroC;
    }
}