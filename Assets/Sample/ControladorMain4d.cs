// This script was forked from CameraControl4D.cs

//#########[---------------------------]#########
//#########[  GENERATED FROM TEMPLATE  ]#########
//#########[---------------------------]#########
#define USE_4D
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class ControladorMain4D : BasicCamera4D {
    public const float MOVE_SPEED = 60.0f;
    public const float JUMP_SPEED = 6.0f;
    public const float PLAYER_RADIUS = 0.3f;
    public const float CAM_HEIGHT = 0.05f;
    public const float VOLUME_TIME = 0.75f;
    public const float GRAVITY_RATE = 90.0f; //Degrees / Sec
    public const float GRAVITY_SMOOTH = 0.25f;
    public const float ZOOM_RATE = 1.1f;
    public const float ZOOM_MAX = 8.0f;
    public const float ZOOM_MIN = 0.3f;
    public const float ZOOM_SMOOTH = 0.05f;
    public static int PROJECTION_MODE = 0;

    public CompassContainer compass;
    public float lookYZ = 0.0f;

    [System.NonSerialized] public Quaternion m0Quaternion = Quaternion.identity;
    [System.NonSerialized] public Quaternion m1 = Quaternion.identity;
    [System.NonSerialized] public bool locked = false;
    [System.NonSerialized] public bool lockViews = false; //Locks volume, shadow, and slice views
    [System.NonSerialized] public bool volumeMode = false;

    private Vector4 smoothGravityDirection = (Vector4)Vector3.up;
    private Vector4 intermediateGravityDirection = (Vector4)Vector3.up;
    protected Matrix4x4 gravityMatrix = Matrix4x4.identity;
    protected bool fastGravity = false;
    private float volumeInterp = 0.0f;
    protected float volumeSmooth = 0.0f;
    protected float volumeStartYZ = 0.0f;
    protected float smoothAngX = 0.0f;
    protected float smoothAngY = 0.0f;
    protected float zoom = 1.0f;
    protected float targetZoom = 1.0f;
    protected float volumeHeight = 0.05f; //CAM_HEIGHT
    protected float runMultiplier = 2.0f;
    private bool oddFrame = true;

    [System.NonSerialized] public bool sliceEnabled = true;
    [System.NonSerialized] public int shadowMode = 1;

    private Quaternion vrLastOrientation = Quaternion.identity;
    private Vector3 vrLastPosition = Vector3.zero;
    private Quaternion vrM1Unfiltered = Quaternion.identity;
    public static Quaternion vrCompassShift = Quaternion.Euler(90, 0, 0) * Quaternion.Euler(0, 0, 180);



    public GameObject marcador;
    public GameObject contenedorMarcadores;
    private int indiceMarcador;

    public GameObject esfera;
    public float potencia;
    public float anguloTheta;
    public float anguloPhi;

    public float multiplicadorPotencia;
    public float alphaGD;
    private Objetivo[] objetivos;

    public GameObject flecha;

    public GameObject volumeLine;
    private bool yaMedido;
    private float potenciaLanzamientoActual;
    private float anguloThetaLanzamientoActual;
    private float anguloPhiLanzamientoActual;
    public float gravedad;
    public Color colorMarcadoresJugador;
    public Color colorFlechasJugador;
    public Color colorMarcadoresGradient;
    public Color colorFlechasGradient;
    public Color colorMarcadoresNewton;
    public Color colorFlechasNewton;
    public Color colorMarcadoresMetodoCombinado;
    public Color colorFlechasMetodoCombinado;
    public int tipoIndicadorJugador;
    public float escalaGradienteFlecha;
    public bool GDUsaFlecha;

    // Parámetros para las simulaciones
    public float toleranciaSimulacion;
    public float alphaGDSimulacion;
    public int iteracionesSimulacion;

    private float xKGradiente;
    private float yKGradiente;
    private float zKGradiente;

    private float xKNewton;
    private float yKNewton;
    private float zKNewton;

    private float xKCombinado;
    private float yKCombinado;
    private float zKCombinado;

    private int jugadorActual;

    public bool animacionEsferaActivada;

    public GameObject textoIndicadorPuntosSeleccionado;
    public GameObject botonAumentarMarcadorSeleccionado;
    public GameObject botonDisminuirMarcadorSeleccionado;
    private int indiceMarcadorSeleccionado;
    public bool marcadorSeleccionadoAuto;

    public GameObject indicadorMarcadorSeleccionado;
    public GameObject botonAumentarPotencia;
    public GameObject botonDisminuirPotencia;
    public GameObject botonAumentarAnguloTheta;
    public GameObject botonDisminuirAnguloTheta;
    public GameObject botonAumentarAnguloPhi;
    public GameObject botonDisminuirAnguloPhi;
    public GameObject textoPotencia;
    public GameObject textoAnguloTheta;
    public GameObject textoAnguloPhi;

    public GameObject textoPotenciaGradient;
    public GameObject textoAnguloThetaGradient;
    public GameObject textoAnguloPhiGradient;
    public GameObject textoPotenciaNewton;
    public GameObject textoAnguloThetaNewton;
    public GameObject textoAnguloPhiNewton;
    public GameObject textoPotenciaCombinado;
    public GameObject textoAnguloThetaCombinado;
    public GameObject textoAnguloPhiCombinado;

    public GameObject botonEscogerModoGradiente;
    public GameObject botonEscogerIndicadorJugador;
    public GameObject botonEscogerAnimacion;
    public GameObject botonEscogerAutoCercano;

    private bool juegoFinalizado;

    private bool estadoMetodoCombinado;

    public GameObject botonIniciarSimulacion;
    public TMP_InputField inputCantidadSim;
    public TMP_InputField inputIteracionesSim;
    public TMP_InputField inputAlphaGDSim;
    public TMP_InputField inputEpsilonSim;
    public TMP_InputField inputToleranciaSim;
    public TMP_InputField inputIndicadoresSim;
    public GameObject textoResultadoSim;
    public GameObject botonDimensionesSim;
    private bool dimensionesSim = false; // false->3D  true->4D
    private bool simulacionCalculandose = false;
    private IEnumerator coroutineSimulacion;

    public GameObject botonAbrirPanelSimulacion;
    private bool panelSimulacionAbierto=false;

    public GameObject textoControles;

    private float[] resultadoSim;

    public float prueba;


    protected override void Start() {
        base.Start();
        InputManager.HideCursor(false);
        colliderRadius = PLAYER_RADIUS;
        useGravity=false;


        indiceMarcador=0;

        objetivos =reinicializarObjetivos(3);
        yaMedido = true;
        potenciaLanzamientoActual = 0;
        anguloThetaLanzamientoActual = 0;
        anguloPhiLanzamientoActual = 0;
        gravedad = 9.8f;
        reinicializarMetodo(out xKGradiente, out yKGradiente, out zKGradiente);
        reinicializarMetodo(out xKNewton, out yKNewton, out zKNewton);
        reinicializarMetodo(out xKCombinado, out yKCombinado, out zKCombinado);
        estadoMetodoCombinado = false;
        esfera.SetActive(false);
        jugadorActual = 0;
        indiceMarcadorSeleccionado = 0;
        juegoFinalizado=false;

        botonAumentarMarcadorSeleccionado.GetComponent<Button>().onClick.AddListener(() => {
            if(indiceMarcador!=0){
                indiceMarcadorSeleccionado++;
                if(indiceMarcadorSeleccionado>=indiceMarcador) indiceMarcadorSeleccionado=0;
            }
        });
        botonDisminuirMarcadorSeleccionado.GetComponent<Button>().onClick.AddListener(() => {
            if(indiceMarcador!=0){
                indiceMarcadorSeleccionado--;
                if(indiceMarcadorSeleccionado<0) indiceMarcadorSeleccionado=indiceMarcador-1;
            }
        });

        botonAumentarPotencia.GetComponent<Button>().onClick.AddListener(() => {
            potencia++;
            if (potencia > 1300) potencia = 1300;
        });
        botonDisminuirPotencia.GetComponent<Button>().onClick.AddListener(() => {
            potencia--;
            if (potencia<0) potencia = 0;
        });
        botonAumentarAnguloTheta.GetComponent<Button>().onClick.AddListener(() => {
            anguloTheta+=0.25f;
            if (anguloTheta>=360) anguloTheta = 0;
        });
        botonDisminuirAnguloTheta.GetComponent<Button>().onClick.AddListener(() => {
            anguloTheta-=0.25f;
            if (anguloTheta < 0) anguloTheta = 359.75f;
        });
        botonAumentarAnguloPhi.GetComponent<Button>().onClick.AddListener(() => {
            anguloPhi += 0.25f;
            if (anguloPhi >= 360) anguloPhi = 0;
        });
        botonDisminuirAnguloPhi.GetComponent<Button>().onClick.AddListener(() => {
            anguloPhi -= 0.25f;
            if (anguloPhi < 0) anguloPhi = 359.75f;
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
        botonEscogerAutoCercano.GetComponent<Button>().onClick.AddListener(() => {
            marcadorSeleccionadoAuto=!marcadorSeleccionadoAuto;
        });

        botonIniciarSimulacion.GetComponent<Button>().onClick.AddListener(() => {
            //realizarSimulacion();
            if (simulacionCalculandose) {
                StopCoroutine(coroutineSimulacion);
                simulacionCalculandose = false;
                textoResultadoSim.GetComponent<TextMeshProUGUI>().text = "La simulació s'ha cancel·lat";
            } else {
                coroutineSimulacion = realizarSimulacionEnParalelo();
                StartCoroutine(coroutineSimulacion);
            }
            
        });
        botonDimensionesSim.GetComponent<Button>().onClick.AddListener(() => {
            dimensionesSim = !dimensionesSim;
        });
        botonAbrirPanelSimulacion.GetComponent<Button>().onClick.AddListener(() => {
            panelSimulacionAbierto = !panelSimulacionAbierto;
        });



        inputCantidadSim.text = "100";
        inputIteracionesSim.text = "500";
        inputAlphaGDSim.text = "0,1";
        inputEpsilonSim.text = "0,001";
        inputToleranciaSim.text = "0,05";
        inputIndicadoresSim.text = "3";

        /*float[] pruebsda = new ControladorPrincipal().simulacionMetodos(100, 0.5f, 0.1f);
        Debug.Log(pruebsda[0]);*/

    }

    public override void Reset() {
        base.Reset();
        lookYZ = 0.0f;
        m0Quaternion = Quaternion.identity;
        m1 = Quaternion.identity;
        locked = false;
        smoothAngX = 0.0f;
        smoothAngY = 0.0f;
        volumeMode = false;
        volumeInterp = 0.0f;
        volumeSmooth = 0.0f;
        volumeStartYZ = 0.0f;
        smoothGravityDirection = (Vector4)Vector3.up;
        intermediateGravityDirection = (Vector4)Vector3.up;
        gravityMatrix = Matrix4x4.identity;
        fastGravity = false;
        zoom = 1.0f;
        targetZoom = 1.0f;
    }

    public virtual void StartGame() {}

    public float CamHeight() {
        //In VR, camera height is built-in, don't need to add anything extra.
        float headHeight = (UnityEngine.XR.XRSettings.enabled ? 0.0f : CAM_HEIGHT);
        return Mathf.Lerp(headHeight, volumeHeight, volumeSmooth);
    }

    public bool IsVolumeTransition() {
        return (volumeInterp != 0.0f && volumeInterp != 1.0f);
    }

    public void HandleLooking() {
        //Apply looking
        float mouseSmooth = Mathf.Pow(2.0f, -Time.deltaTime / InputManager.CAM_SMOOTHING);
        float angX = InputManager.GetAxis(InputManager.AxisBind.LookHorizontal);
        float angY = InputManager.GetAxis(InputManager.AxisBind.LookVertical);

        if (Time.deltaTime == 0.0f) {
            smoothAngX = 0.0f;
            smoothAngY = 0.0f;
        } else {
            smoothAngX = smoothAngX * mouseSmooth + angX * (1.0f - mouseSmooth);
            smoothAngY = smoothAngY * mouseSmooth + angY * (1.0f - mouseSmooth);
        }

        //Update rotations
        if (UnityEngine.XR.XRSettings.enabled) {
            if (VRInputManager.GetKeyDown(VRInputManager.VRButton.Rotate)) {
                VRInputManager.HandOrientation(VRInputManager.Hand.Left, out vrLastOrientation);
                vrM1Unfiltered = m1;
                vrLastOrientation = vrCompassShift * vrLastOrientation;
            }
            if (VRInputManager.GetKey(VRInputManager.VRButton.Rotate)) {
                if (volumeMode) {
                    //TODO: Handle volume mode
                } else {
                    if (VRInputManager.HandOrientation(VRInputManager.Hand.Left, out Quaternion newOrientation)) {
                        newOrientation = vrCompassShift * newOrientation;
#if USE_5D
                        //TODO: Handle 5D
#else
                        vrM1Unfiltered = vrM1Unfiltered * vrLastOrientation * Quaternion.Inverse(newOrientation);
                        m1 = Quaternion.Slerp(vrM1Unfiltered, m1, Mathf.Pow(2.0f, -8.0f * Time.deltaTime));
#endif
                        vrLastOrientation = newOrientation;
                    } else {
                        vrLastOrientation = Quaternion.identity;
                    }
                }
            }
        } else {
            if (InputManager.GetKey(InputManager.KeyBind.Look4D)) {
                if (volumeMode) {
                    //m1 = m1 * Quaternion.Euler(0.0f, smoothAngX, 0.0f);
                    m1 = m1 * Quaternion.Euler(-smoothAngY, smoothAngX, 0.0f);
                } else {
                    m1 = m1 * Quaternion.Euler(-smoothAngY, smoothAngX, 0.0f);
                }
#if USE_5D
            } else if (InputManager.GetKey(InputManager.KeyBind.Look5D)) {
                Quaternion q = Quaternion.Euler(-smoothAngX, -smoothAngY, 0.0f);
                m1 = m1 * new Isocline(q, q);
            } else if (InputManager.GetKey(InputManager.KeyBind.LookSpin)) {
                Quaternion q = Quaternion.Euler(0.0f, 0.0f, smoothAngX);
                m1 = m1 * new Isocline(q, q);
#endif
            } else if(Input.GetMouseButton(0)){
                if (volumeMode) {
                    m1 = m1 * Quaternion.Euler(-smoothAngY, 0.0f, -smoothAngX);
                } else {
                    m1 = m1 * Quaternion.Euler(0.0f, 0.0f, -smoothAngX);
                    lookYZ += smoothAngY;
                    lookYZ = Mathf.Clamp(lookYZ, -89.0f, 89.0f);
                }
                
            }
        }
    }

    public Vector4 HandleMoving() {
        //Get movement force and jump
        Vector4 accel = Vector4.zero;
        accel.x = InputManager.GetAxis(InputManager.AxisBind.MoveLeftRight);
        if (InputManager.GetKey(InputManager.KeyBind.Left)) {
            accel.x = -1.0f;
        }
        if (InputManager.GetKey(InputManager.KeyBind.Right)) {
            accel.x = 1.0f;
        }
        accel.z = InputManager.GetAxis(InputManager.AxisBind.MoveForwardBack);
        if (InputManager.GetKey(InputManager.KeyBind.Backward)) {
            accel.z = -1.0f;
        }
        if (InputManager.GetKey(InputManager.KeyBind.Forward)) {
            accel.z = 1.0f;
        }
        accel.w = InputManager.GetAxis(InputManager.AxisBind.MoveAnaKata);
        if (InputManager.GetKey(InputManager.KeyBind.Kata)) {
            accel.w = -1.0f;
        }
        if (InputManager.GetKey(InputManager.KeyBind.Ana)) {
            accel.w = 1.0f;
        }
        accel.y = volumeSmooth * accel.w;
        
        if(Input.GetKey(KeyCode.Q)){ accel.y = -1.0f; }
        if(Input.GetKey(KeyCode.E)){ accel.y = 1.0f; }

        accel.w = (volumeSmooth - 1.0f) * accel.w;
#if USE_5D
        accel.v = InputManager.GetAxis(InputManager.AxisBind.MoveSursumDeorsum);
        if (InputManager.GetKey(InputManager.KeyBind.Sursum)) {
            accel.v = -1.0f;
        }
        if (InputManager.GetKey(InputManager.KeyBind.Deorsum)) {
            accel.v = 1.0f;
        }
#endif
        if (accel != Vector4.zero) {
            accel = camMatrix * accel;
            float mag = Mathf.Min(accel.magnitude, 1.0f);
            if (useGravity) {
                accel -= smoothGravityDirection * Vector4.Dot(smoothGravityDirection, accel);
            }
            accel = accel.normalized * mag;
            if (InputManager.GetKey(InputManager.KeyBind.Run)) {
                accel *= runMultiplier;
            }

            // Ir más lento
            if (Input.GetKey(KeyCode.LeftControl)) {
                accel /= runMultiplier;
            }
        }

        return accel;
    }

    /*public void HandleMovingGlobal() {
        //Get movement force and jump
        Vector4 accel = Vector4.zero;
        accel.x = InputManager.GetAxis(InputManager.AxisBind.MoveLeftRight);
        if (InputManager.GetKey(InputManager.KeyBind.Left)) {
            accel.x = -1.0f;
        }
        if (InputManager.GetKey(InputManager.KeyBind.Right)) {
            accel.x = 1.0f;
        }
        accel.z = InputManager.GetAxis(InputManager.AxisBind.MoveForwardBack);
        if (InputManager.GetKey(InputManager.KeyBind.Backward)) {
            accel.z = -1.0f;
        }
        if (InputManager.GetKey(InputManager.KeyBind.Forward)) {
            accel.z = 1.0f;
        }
        accel.w = InputManager.GetAxis(InputManager.AxisBind.MoveAnaKata);
        if (InputManager.GetKey(InputManager.KeyBind.Kata)) {
            accel.w = -1.0f;
        }
        if (InputManager.GetKey(InputManager.KeyBind.Ana)) {
            accel.w = 1.0f;
        }
        accel.y = volumeSmooth * accel.w;

        if (Input.GetKey(KeyCode.Q)) { accel.y = -1.0f; }
        if (Input.GetKey(KeyCode.E)) { accel.y = 1.0f; }

        accel.w = (volumeSmooth - 1.0f) * accel.w;

        // Ir más rápido
        if (InputManager.GetKey(InputManager.KeyBind.Run)) {
            accel *= runMultiplier;
        }

        // Ir más lento
        if (Input.GetKey(KeyCode.LeftControl)) {
            accel /= runMultiplier;
        }

        this.position4D.y += accel[1] * (Time.deltaTime * MOVE_SPEED) * 0.05f;

        if (accel != Vector4.zero) {
            accel = camMatrix * accel;
            float mag = Mathf.Min(accel.magnitude, 1.0f);
            if (useGravity) {
                accel -= smoothGravityDirection * Vector4.Dot(smoothGravityDirection, accel);
            }
            accel = accel.normalized * mag;
            
        }
        //velocity += HandleMoving() * (Time.deltaTime * MOVE_SPEED);
        //Debug.Log("Accel:"+accel+" CamMatrix:"+camMatrix);
        //this.position4D += accel * (Time.deltaTime * MOVE_SPEED)*0.05f;
        this.position4D.x += accel[0] * (Time.deltaTime * MOVE_SPEED) * 0.05f;
        
        this.position4D.z += accel[2] * (Time.deltaTime * MOVE_SPEED) * 0.05f;
        this.position4D.w += accel[3] * (Time.deltaTime * MOVE_SPEED) * 0.05f;


        //return accel;
    }*/


    protected virtual void Update() {

        if (Input.GetKey(KeyCode.Escape)) {
            Application.Quit();
        }

        if (InputManager.GetKeyDown(InputManager.KeyBind.Reset) && !panelSimulacionAbierto) {
            for(int i = 0; i<contenedorMarcadores.transform.childCount; i++){
                Destroy(contenedorMarcadores.transform.GetChild(i).gameObject);
            }
            indiceMarcador = 0;
            objetivos = reinicializarObjetivos(3);
            juegoFinalizado = false;

            reinicializarMetodo(out xKGradiente, out yKGradiente, out zKGradiente);
            reinicializarMetodo(out xKNewton, out yKNewton, out zKNewton);
            reinicializarMetodo(out xKCombinado, out yKCombinado, out zKCombinado);
            estadoMetodoCombinado = false;
        }

        if(Input.GetKeyDown(KeyCode.Return) && false){
            /*colocarMarcador(2, 2, 2, ""+funcionPrincipal(2, 2, 2, objetivos), new Color(0, 0, 1));
            colocarMarcador(3, 3, 3, ""+funcionPrincipal(2, 2, 2, objetivos), new Color(0, 0, 1));
            colocarMarcador(4, 4, 4, ""+funcionPrincipal(2, 2, 2, objetivos), new Color(0, 0, 1));
            colocarMarcador(5, 5, 5, ""+funcionPrincipal(2, 2, 2, objetivos), new Color(0, 0, 1));*/

            int cantidadSimulaciones=1;

            float mediaGD=0;
            float mediaNM=0;
            float mediaMC=0;

            for(int i=0; i<cantidadSimulaciones; i++) {
                float[] resultadosSimulacionActual=simulacionMetodos(iteracionesSimulacion, alphaGDSimulacion, toleranciaSimulacion, 0.001f, 3, true);
                mediaGD+=resultadosSimulacionActual[0];
                mediaNM+=resultadosSimulacionActual[1];
                mediaMC+=resultadosSimulacionActual[2];
            }
            mediaGD/=cantidadSimulaciones;
            mediaNM/=cantidadSimulaciones;
            mediaMC/=cantidadSimulaciones;

            // float[] resultadosSimulacion=simulacionMetodos(iteracionesSimulacion, alphaGDSimulacion, toleranciaSimulacion);

            Debug.Log("Resultados: GD:"+ mediaGD + " NM:"+ mediaNM + " MC:"+ mediaMC);
        }

        bool seHaLanzadoSinAnimacion = false;
        float cambioValores=1;

        if(InputManager.GetKey(InputManager.KeyBind.Run)) {
            cambioValores*=2;
        }
        if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftAlt)) {
            cambioValores*=0.25f;
        }

        if(Input.GetKey(KeyCode.O) && !panelSimulacionAbierto) {
            potencia-=cambioValores*4;
            if(potencia<=0) potencia=0;
        }
        if(Input.GetKey(KeyCode.P)&& !panelSimulacionAbierto) {
            potencia+=cambioValores*4;
            if(potencia>1300) potencia=1300;
        }
        if(Input.GetKey(KeyCode.LeftArrow)&& !panelSimulacionAbierto) {
            anguloTheta-=cambioValores;
            if(anguloTheta<0) anguloTheta=360-cambioValores;
        }
        if(Input.GetKey(KeyCode.RightArrow)&& !panelSimulacionAbierto) {
            anguloTheta+=cambioValores;
            if(anguloTheta>=360) anguloTheta=0;
        }
        if(Input.GetKey(KeyCode.UpArrow)&& !panelSimulacionAbierto) {
            anguloPhi+=cambioValores;
            if(anguloPhi>=360) anguloPhi=0;
        }
        if(Input.GetKey(KeyCode.DownArrow)&& !panelSimulacionAbierto) {
            anguloPhi-=cambioValores;
            if(anguloPhi<0) anguloPhi=360-cambioValores;
        }

        if(Input.GetKeyDown(KeyCode.F) && !juegoFinalizado && !panelSimulacionAbierto) {
            juegoFinalizado = true;
            for(int i=0; i<objetivos.Length; i++) {
                colocarMarcadorFinal(objetivos[i].a, objetivos[i].b, objetivos[i].d, Color.gray);
            }
        }

        if(InputManager.GetKeyDown(InputManager.KeyBind.Putt) && yaMedido && potencia>=0 && !juegoFinalizado&& !panelSimulacionAbierto){
            //flecha.GetComponent<Object4D>().localRotation4D=Transform4D.FromToRotation(new Vector4(1, 2, 3, 4), new Vector4(2, 2, 2, 2));
            /*flecha.GetComponent<Object4D>().localRotation4D=Matrix4x4.zero;
            flecha.GetComponent<Object4D>().localRotation4D=Transform4D.PlaneRotation(prueba, 0, 1);*/
            //Debug.Log(flecha.GetComponent<Object4D>().localRotation4D);
            
            if (animacionEsferaActivada) {
                lanzarEsfera(potencia, anguloTheta, anguloPhi, colorMarcadoresJugador);
            } else {
                seHaLanzadoSinAnimacion = true;
                yaMedido = false;
            }

            potenciaLanzamientoActual = potencia;
            anguloThetaLanzamientoActual = anguloTheta;
            anguloPhiLanzamientoActual = anguloPhi;

            jugadorActual = 0;
            yaMedido = false;
        }

        if(Input.GetKeyDown(KeyCode.G) && yaMedido && !juegoFinalizado&& !panelSimulacionAbierto){
            jugadorActual = 1;
            if (animacionEsferaActivada) {
                float[] valoresCalculados = deCartesianoAEsferico(xKGradiente, yKGradiente, zKGradiente);
                lanzarEsfera(valoresCalculados[0], valoresCalculados[1], valoresCalculados[2], colorMarcadoresGradient);
                Debug.Log("Lanzamiento GD:  Potencia:"+ valoresCalculados[0]+" AnguloTheta:"+ valoresCalculados[1]+" AnguloPhi"+ valoresCalculados[2]);
            } else {
                seHaLanzadoSinAnimacion = true;
                yaMedido = false;
            }
        }
        if(Input.GetKeyDown(KeyCode.N) && yaMedido && !juegoFinalizado&& !panelSimulacionAbierto){
            jugadorActual = 2;
            if (animacionEsferaActivada) {
                float[] valoresCalculados = deCartesianoAEsferico(xKNewton, yKNewton, zKNewton);
                lanzarEsfera(valoresCalculados[0], valoresCalculados[1], valoresCalculados[2], colorMarcadoresNewton);
                Debug.Log("Lanzamiento NM:  Potencia:"+ valoresCalculados[0]+" AnguloTheta:"+ valoresCalculados[1]+" AnguloPhi"+ valoresCalculados[2]);
            } else {
                seHaLanzadoSinAnimacion = true;
                yaMedido = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.H) && yaMedido && !juegoFinalizado&& !panelSimulacionAbierto) {
            jugadorActual = 3;
            if (animacionEsferaActivada) {
                float[] valoresCalculados = deCartesianoAEsferico(xKCombinado, yKCombinado, zKCombinado);
                lanzarEsfera(valoresCalculados[0], valoresCalculados[1], valoresCalculados[2], colorMarcadoresMetodoCombinado);
                Debug.Log("Lanzamiento MC:  Potencia:" + valoresCalculados[0] + " AnguloTheta:" + valoresCalculados[1] + " AnguloPhi" + valoresCalculados[2]);
            } else {
                seHaLanzadoSinAnimacion = true;
                yaMedido = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.C)) {
            if (textoControles.activeSelf) {
                textoControles.SetActive(false);
            } else {
                textoControles.SetActive(true);
            }
        }



        // Cambiar orientacion de la flecha
        if (yaMedido || jugadorActual==0) {
            flecha.GetComponent<Object4D>().localRotation4D=Transform4D.PlaneRotation(-anguloPhi, 0, 3)*Transform4D.PlaneRotation(anguloTheta, 0, 2);
        } else if(jugadorActual == 1){
            float[] angulosCalculados=deCartesianoAEsferico(xKGradiente, yKGradiente, zKGradiente);
            flecha.GetComponent<Object4D>().localRotation4D=Transform4D.PlaneRotation(-angulosCalculados[2], 0, 3)*Transform4D.PlaneRotation(angulosCalculados[1], 0, 2);
        } else if(jugadorActual == 2){
            float[] angulosCalculados=deCartesianoAEsferico(xKNewton, yKNewton, zKNewton);
            flecha.GetComponent<Object4D>().localRotation4D=Transform4D.PlaneRotation(-angulosCalculados[2], 0, 3)*Transform4D.PlaneRotation(angulosCalculados[1], 0, 2);
        } else if(jugadorActual == 3){
            float[] angulosCalculados=deCartesianoAEsferico(xKCombinado, yKCombinado, zKCombinado);
            flecha.GetComponent<Object4D>().localRotation4D=Transform4D.PlaneRotation(-angulosCalculados[2], 0, 3)*Transform4D.PlaneRotation(angulosCalculados[1], 0, 2);
        } 
        if((esfera.transform.position.y<=0 || seHaLanzadoSinAnimacion) && !yaMedido) {
            if(jugadorActual==0) {
                float potenciaReal = potenciaLanzamientoActual / multiplicadorPotencia;
                float radio = (potenciaReal * potenciaReal) / gravedad;

                float posX=radio*Mathf.Sin(anguloThetaLanzamientoActual*Mathf.Deg2Rad)*Mathf.Cos(anguloPhiLanzamientoActual*Mathf.Deg2Rad);
                float posW=radio*Mathf.Sin(anguloThetaLanzamientoActual*Mathf.Deg2Rad)*Mathf.Sin(anguloPhiLanzamientoActual*Mathf.Deg2Rad);
                float posZ=radio*Mathf.Cos(anguloThetaLanzamientoActual*Mathf.Deg2Rad);

                // Texto: colocarMarcador(posX, posZ, posW, , colorMarcadoresJugador);
                if(tipoIndicadorJugador==3) {
                    colocarMarcadorYCercanosFlecha(posX, posW, posZ, 25, 0.75f, escalaGradienteFlecha, colorMarcadoresJugador, colorFlechasJugador);
                } else if(tipoIndicadorJugador==2) {
                    colocarMarcadorYCercanos(posX, posW, posZ, 25, 0.75f, colorMarcadoresJugador);
                } else if (tipoIndicadorJugador==1) {
                    float[] gradienteEvaluado = gradiente(posX, posW, posZ, objetivos);
                    colocarMarcadorConFlecha(posX, posW, posZ, -gradienteEvaluado[0], -gradienteEvaluado[1], -gradienteEvaluado[2], escalaGradienteFlecha, colorMarcadoresJugador, colorFlechasJugador);
                } else {
                    colocarMarcador(posX, posW, posZ, colorMarcadoresJugador);
                }

            } else if(jugadorActual == 1) {
                float[] gradienteEvaluado = gradiente(xKGradiente, yKGradiente, zKGradiente, objetivos);

                if(GDUsaFlecha) {
                    colocarMarcadorConFlecha(xKGradiente, yKGradiente, zKGradiente, -gradienteEvaluado[0], -gradienteEvaluado[1], -gradienteEvaluado[2], escalaGradienteFlecha, colorMarcadoresGradient, colorFlechasGradient);
                } else {
                    colocarMarcadorYCercanos(xKGradiente, yKGradiente, zKGradiente, 25, 0.75f, colorMarcadoresGradient);
                }

                // Hacer la siguiente iteración
                float[] xK2 = iterarGradientDescent(xKGradiente, yKGradiente, zKGradiente, alphaGD, objetivos);
                xKGradiente = xK2[0];
                yKGradiente = xK2[1];
                zKGradiente = xK2[2];
            } else if(jugadorActual == 2) {
                colocarMarcadorYCercanosFlecha(xKNewton, yKNewton, zKNewton, 25, 0.75f, escalaGradienteFlecha, colorMarcadoresNewton, colorFlechasNewton);

                // Hacer la siguiente iteración
                float[] xK2 = iterarNewtonMethod(xKNewton, yKNewton, zKNewton, objetivos);
                xKNewton = xK2[0];
                yKNewton = xK2[1];
                zKNewton = xK2[2];
            } else if(jugadorActual == 3) {
                if (!estadoMetodoCombinado) {
                    // Modo GD
                    float[] gradienteEvaluado = gradiente(xKCombinado, yKCombinado, zKCombinado, objetivos);
                    if (GDUsaFlecha) {
                        colocarMarcadorConFlecha(xKCombinado, yKCombinado, zKCombinado, -gradienteEvaluado[0], -gradienteEvaluado[1], -gradienteEvaluado[2], escalaGradienteFlecha, colorMarcadoresMetodoCombinado, colorFlechasMetodoCombinado);
                    } else {
                        colocarMarcadorYCercanos(xKCombinado, yKCombinado, zKCombinado, 25, 0.75f, colorMarcadoresGradient);
                    }

                    // Ver si es mejor cambiar a NM
                    // Iterar GD
                    float[] iteracionGD=siguienteIteracionGD(xKCombinado, yKCombinado, zKCombinado, alphaGD, objetivos);
                    // Iterar NM
                    float[] iteracionNM=siguienteIteracionNM(xKCombinado, yKCombinado, zKCombinado, objetivos);

                    if(funcionPrincipal(iteracionGD[0], iteracionGD[1], iteracionGD[2], objetivos)<funcionPrincipal(iteracionNM[0], iteracionNM[1], iteracionNM[2], objetivos)) {
                        // NM tiene mas puntos, hay que cambiar
                        estadoMetodoCombinado = true;
                        xKCombinado = iteracionNM[0];
                        yKCombinado = iteracionNM[1];
                        zKCombinado = iteracionNM[2];
                    } else {
                        // Hacer la siguiente iteración (en GD)
                        float[] xK2 = iterarGradientDescent(xKCombinado, yKCombinado, zKCombinado, alphaGD, objetivos);
                        xKCombinado = xK2[0];
                        yKCombinado = xK2[1];
                        zKCombinado = xK2[2];
                    }
                } else {
                    // Modo NM
                    colocarMarcadorYCercanosFlecha(xKCombinado, yKCombinado, zKCombinado, 25, 0.75f, escalaGradienteFlecha, colorMarcadoresMetodoCombinado, colorFlechasMetodoCombinado);

                    // Hacer la siguiente iteración
                    float[] xK2 = iterarNewtonMethod(xKCombinado, yKCombinado, zKCombinado, objetivos);
                    xKCombinado = xK2[0];
                    yKCombinado = xK2[1];
                    zKCombinado = xK2[2];
                }
            }

            yaMedido = true;
        }

        // Mostrar cual es el marcador seleccionado
        /*if(indiceMarcador!=0) {
            Transform marcadorSeleccionado = contenedorMarcadores.transform.GetChild(indiceMarcadorSeleccionado);

            Color colorOriginal= marcadorSeleccionado.GetChild(1).GetComponent<MeshRenderer>().material.color;
            Color.RGBToHSV(colorOriginal, out float hue, out float saturation, out float vue);
            marcadorSeleccionado.GetChild(1).GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(hue, saturation, vue/100);

            int siguienteMarcador=indiceMarcadorSeleccionado+1;
            if(siguienteMarcador>=indiceMarcador) siguienteMarcador=0;
            marcadorSeleccionado = contenedorMarcadores.transform.GetChild(siguienteMarcador);
            colorOriginal = marcadorSeleccionado.GetChild(1).GetComponent<MeshRenderer>().material.color;
            Color.RGBToHSV(colorOriginal, out hue, out saturation, out vue);
            marcadorSeleccionado.GetChild(1).GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(hue, saturation, 1);
            Debug.Log(Color.HSVToRGB(hue, saturation, vue * 100));

            int anteriorMarcador=indiceMarcadorSeleccionado-1;
            if(anteriorMarcador<0) anteriorMarcador=indiceMarcador-1;
            marcadorSeleccionado = contenedorMarcadores.transform.GetChild(anteriorMarcador);
            colorOriginal = marcadorSeleccionado.GetChild(1).GetComponent<MeshRenderer>().material.color;
            Color.RGBToHSV(colorOriginal, out hue, out saturation, out vue);
            marcadorSeleccionado.GetChild(1).GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(hue, saturation, 1);
        }*/
        
        
        
        //Debug.Log(this.position4D);


        //Actualización UI
        /*if (indiceMarcador == 0) {
            textoIndicadorPuntosSeleccionado.GetComponent<TextMeshProUGUI>().text = "Aún no hay ningún marcador";
        } else {
            Transform marcadorSeleccionado = contenedorMarcadores.transform.GetChild(indiceMarcadorSeleccionado);
            textoIndicadorPuntosSeleccionado.GetComponent<TextMeshProUGUI>().text = "Puntos marcador #"+indiceMarcadorSeleccionado+": "+funcionPrincipal(marcadorSeleccionado.position.x, marcadorSeleccionado.GetComponent<Object4D>().positionW, marcadorSeleccionado.position.z, objetivos);
        }*/
        textoPotencia.GetComponent<TextMeshProUGUI>().text = "Potència: " + potencia;
        textoAnguloTheta.GetComponent<TextMeshProUGUI>().text = "Angle θ: " + anguloTheta;
        textoAnguloPhi.GetComponent<TextMeshProUGUI>().text = "Angle ϕ: " + anguloPhi;

        float[] valoresCalculadosGradientTexto=deCartesianoAEsferico(xKGradiente, yKGradiente, zKGradiente);
        textoPotenciaGradient.GetComponent<TextMeshProUGUI>().text = "Potència: " + valoresCalculadosGradientTexto[0];
        textoAnguloThetaGradient.GetComponent<TextMeshProUGUI>().text = "Angle θ: " + valoresCalculadosGradientTexto[1];
        textoAnguloPhiGradient.GetComponent<TextMeshProUGUI>().text = "Angle ϕ: " + valoresCalculadosGradientTexto[2];

        float[] valoresCalculadosNewtonTexto = deCartesianoAEsferico(xKNewton, yKNewton, zKNewton);
        textoPotenciaNewton.GetComponent<TextMeshProUGUI>().text = "Potència: " + valoresCalculadosNewtonTexto[0];
        textoAnguloThetaNewton.GetComponent<TextMeshProUGUI>().text = "Angle θ: " + valoresCalculadosNewtonTexto[1];
        textoAnguloPhiNewton.GetComponent<TextMeshProUGUI>().text = "Angle ϕ: " + valoresCalculadosNewtonTexto[2];

        float[] valoresCalculadosCombinadoTexto = deCartesianoAEsferico(xKCombinado, yKCombinado, zKCombinado);
        textoPotenciaCombinado.GetComponent<TextMeshProUGUI>().text = "Potència: " + valoresCalculadosCombinadoTexto[0];
        textoAnguloThetaCombinado.GetComponent<TextMeshProUGUI>().text = "Angle θ: " + valoresCalculadosCombinadoTexto[1];
        textoAnguloPhiCombinado.GetComponent<TextMeshProUGUI>().text = "Angle ϕ: " + valoresCalculadosCombinadoTexto[2];

        if (GDUsaFlecha) {
            botonEscogerModoGradiente.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Fletxa per mostrar el gradient";
        } else {
            botonEscogerModoGradiente.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Més marcadors per mostrar el gradient";
        }
        if(tipoIndicadorJugador == 0) {
            botonEscogerIndicadorJugador.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Marcador normal";
        } else if(tipoIndicadorJugador == 1) {
            botonEscogerIndicadorJugador.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Marcador amb fletxa";
        } else if(tipoIndicadorJugador == 2) {
            botonEscogerIndicadorJugador.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Marcadors propers";
        } else if(tipoIndicadorJugador == 3) {
            botonEscogerIndicadorJugador.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Marcadors propers amb fletxa";
        }
        if(animacionEsferaActivada) {
            botonEscogerAnimacion.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Animació de la pilota activada";
        } else {
            botonEscogerAnimacion.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Animació de la pilota desactivada";
        }


        if (marcadorSeleccionadoAuto) {
            botonAumentarMarcadorSeleccionado.GetComponent<Button>().interactable = false;
            botonDisminuirMarcadorSeleccionado.GetComponent<Button>().interactable = false;
            botonEscogerAutoCercano.GetComponent<Image>().color = new Color(0, 1, 0);
        } else {
            if (indiceMarcador == 0) {
                botonAumentarMarcadorSeleccionado.GetComponent<Button>().interactable = false;
                botonDisminuirMarcadorSeleccionado.GetComponent<Button>().interactable = false;
            } else {
                botonAumentarMarcadorSeleccionado.GetComponent<Button>().interactable = true;
                botonDisminuirMarcadorSeleccionado.GetComponent<Button>().interactable = true;
            }
            botonEscogerAutoCercano.GetComponent<Image>().color = new Color(1, 0, 0);
        }

        if (indiceMarcador == 0) {
            textoIndicadorPuntosSeleccionado.GetComponent<TextMeshProUGUI>().text = "Encara no hi ha cap marcador";
            indicadorMarcadorSeleccionado.SetActive(false);
        } else {
            indicadorMarcadorSeleccionado.SetActive(true);
            if (marcadorSeleccionadoAuto) {
                // Encontrar cual es el marcador mas cercano al jugador
                int indiceMarcadorMasCercano = 0;
                float distanciaMarcadorMasCercano = 999;
                for (int i = 0; i < indiceMarcador; i++) {
                    Vector3 posicionRealJugador = new Vector3(this.position4D.x, this.position4D.w, this.position4D.z);
                    Transform marcadorActual = contenedorMarcadores.transform.GetChild(i);
                    Vector3 posicionRealMarcadorActual = new Vector3(marcadorActual.position.x, marcadorActual.GetComponent<Object4D>().positionW, marcadorActual.position.z);
                    if (Vector3.Distance(posicionRealJugador, posicionRealMarcadorActual) < distanciaMarcadorMasCercano) {
                        indiceMarcadorMasCercano = i;
                        distanciaMarcadorMasCercano = Vector3.Distance(posicionRealJugador, posicionRealMarcadorActual);
                    }
                }
                

                Transform marcadorSeleccionado = contenedorMarcadores.transform.GetChild(indiceMarcadorMasCercano);
                indicadorMarcadorSeleccionado.transform.position = new Vector3(marcadorSeleccionado.position.x, marcadorSeleccionado.position.y + 0.0001f, marcadorSeleccionado.position.z);
                indicadorMarcadorSeleccionado.GetComponent<Object4D>().positionW = marcadorSeleccionado.gameObject.GetComponent<Object4D>().positionW;


                textoIndicadorPuntosSeleccionado.GetComponent<TextMeshProUGUI>().text = "Punts marcador #" + (indiceMarcadorMasCercano+1) + ": " + funcionPrincipal(marcadorSeleccionado.position.x, marcadorSeleccionado.GetComponent<Object4D>().positionW, marcadorSeleccionado.position.z, objetivos);
               

            } else {
                Transform marcadorSeleccionado = contenedorMarcadores.transform.GetChild(indiceMarcadorSeleccionado);
                indicadorMarcadorSeleccionado.transform.position = new Vector3(marcadorSeleccionado.position.x, marcadorSeleccionado.position.y + 0.0001f, marcadorSeleccionado.position.z);
                indicadorMarcadorSeleccionado.GetComponent<Object4D>().positionW = marcadorSeleccionado.gameObject.GetComponent<Object4D>().positionW;

                textoIndicadorPuntosSeleccionado.GetComponent<TextMeshProUGUI>().text = "Punts marcador #" + (indiceMarcadorSeleccionado+1) + ": " + funcionPrincipal(marcadorSeleccionado.position.x, marcadorSeleccionado.GetComponent<Object4D>().positionW, marcadorSeleccionado.position.z, objetivos);
            }
        }

        if (dimensionesSim) {
            botonDimensionesSim.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Dimensions: 3";
        } else {
            botonDimensionesSim.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Dimensions: 2";
        }
        if (simulacionCalculandose) {
            botonIniciarSimulacion.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Parar simulació";
        } else {
            botonIniciarSimulacion.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Iniciar simulació";
        }
        if (panelSimulacionAbierto) {
            botonAbrirPanelSimulacion.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Tancar opcions simulació";
            botonIniciarSimulacion.transform.parent.gameObject.SetActive(true);
        } else {
            botonAbrirPanelSimulacion.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Obrir opcions simulació";
            botonIniciarSimulacion.transform.parent.gameObject.SetActive(false);
        }
        

        if (panelSimulacionAbierto) {
            locked=true;
            lockViews=true;
        } else {
            locked=false;
            lockViews=false;
        }






        //Update gravity matrix
        float maxAngle = Time.deltaTime * GRAVITY_RATE;
        float gravitySmooth = Mathf.Pow(2.0f, -Time.deltaTime / GRAVITY_SMOOTH);
        float angle = Transform4D.Angle(gravityDirection, smoothGravityDirection);
        if (fastGravity || angle > 60.0f) {
            intermediateGravityDirection = gravityDirection;
        } else {
            intermediateGravityDirection = Transform4D.RotateTowards(intermediateGravityDirection, gravityDirection, maxAngle);
            angle = Transform4D.Angle(intermediateGravityDirection, smoothGravityDirection);
        }
        smoothGravityDirection = Transform4D.RotateTowards(intermediateGravityDirection, smoothGravityDirection, gravitySmooth * angle);
        gravityMatrix = Transform4D.OrthoIterate(Transform4D.FromToRotation(gravityMatrix.GetColumn(1), smoothGravityDirection) * gravityMatrix);

        //Check if shadows should be enabled/disabled
        if (!lockViews && InputManager.GetKeyDown(InputManager.KeyBind.ShadowToggle)) {
            shadowMode = (shadowMode + 1) % 3;
            UpdateCameraMask();
        }
        if (!lockViews && InputManager.GetKeyDown(InputManager.KeyBind.SliceToggle)) {
            sliceEnabled = !sliceEnabled;
            UpdateCameraMask();
        }

        //Check for volume mode change
        if (!lockViews && InputManager.GetKeyDown(InputManager.KeyBind.VolumeView)) {
            if (!volumeMode) { volumeStartYZ = lookYZ; }
            volumeMode = !volumeMode;
        }

        //Interpolate volume change
        volumeInterp = Mathf.Clamp01(volumeInterp + (volumeMode ? Time.deltaTime : -Time.deltaTime) / VOLUME_TIME);
        volumeSmooth = Mathf.SmoothStep(0.0f, 1.0f, volumeInterp);
        Shader.SetGlobalFloat(minCheckerID, volumeSmooth * 0.125f);
        if (volumeMode) {
            lookYZ = Mathf.Lerp(volumeStartYZ, 0.0f, volumeSmooth);

            volumeLine.SetActive(true);
        } else {
            volumeLine.SetActive(false);
        }

        //Handle camera and player inputs or seek
        if (!locked && !panelSimulacionAbierto) {
            HandleLooking();
            velocity += HandleMoving() * (Time.deltaTime * MOVE_SPEED);
            //HandleMovingGlobal();
        }

        //Update compasses
        if (compass) {
            compass.SetRotations(m0Quaternion, m1);
        }

        //Disable overriding up-down look in VR
        if (UnityEngine.XR.XRSettings.enabled) {
            lookYZ = 0.0f;
        }

        //Create the camera matrix
        camMatrix = CreateCamMatrix(m1, lookYZ);

        //Update the m0 quaternion
        m0Quaternion = Quaternion.Slerp(Quaternion.Euler(-lookYZ, 0.0f, 0.0f), Quaternion.Euler(0.0f, 0.0f, 90.0f), volumeSmooth);

    }

    /*void realizarSimulacion() {
        /*int cantidadSimulaciones = Convert.ToInt32(inputCantidadSim.text);

        float mediaGD = 0;
        float mediaNM = 0;
        float mediaMC = 0;

        for (int i = 0; i < cantidadSimulaciones; i++) {
            float[] resultadosSimulacionActual = simulacionMetodos(iteracionesSimulacion, alphaGDSimulacion, toleranciaSimulacion, true);
            mediaGD += resultadosSimulacionActual[0];
            mediaNM += resultadosSimulacionActual[1];
            mediaMC += resultadosSimulacionActual[2];
        }
        mediaGD /= cantidadSimulaciones;
        mediaNM /= cantidadSimulaciones;
        mediaMC /= cantidadSimulaciones;

        // float[] resultadosSimulacion=simulacionMetodos(iteracionesSimulacion, alphaGDSimulacion, toleranciaSimulacion);

        Debug.Log("Resultados: GD:" + mediaGD + " NM:" + mediaNM + " MC:" + mediaMC);*//*
        
        int cantidadSimulaciones=int.Parse(inputCantidadSim.text);
        float mediaGD = 0;
        float mediaNM = 0;
        float mediaMC = 0;

        for (int i = 0; i < cantidadSimulaciones; i++) {
            float[] resultadosSimulacionActual = new float[3];
            if (dimensionesSim) {
                resultadosSimulacionActual = simulacionMetodos(int.Parse(inputIteracionesSim.text), float.Parse(inputAlphaGDSim.text), float.Parse(inputToleranciaSim.text), float.Parse(inputEpsilonSim.text), int.Parse(inputIndicadoresSim.text), true);
            } else {
                resultadosSimulacionActual = Calculos2d.simulacionMetodos(int.Parse(inputIteracionesSim.text), float.Parse(inputAlphaGDSim.text), float.Parse(inputToleranciaSim.text), float.Parse(inputEpsilonSim.text), int.Parse(inputIndicadoresSim.text), false);
            }
            mediaGD += resultadosSimulacionActual[0];
            mediaNM += resultadosSimulacionActual[1];
            mediaMC += resultadosSimulacionActual[2];

            // Poner el porcentaje
            textoResultadoSim.GetComponent<TextMeshProUGUI>().text = (i / cantidadSimulaciones) * 100 + "%";
        }
        mediaGD /= cantidadSimulaciones;
        mediaNM /= cantidadSimulaciones;
        mediaMC /= cantidadSimulaciones;

        textoResultadoSim.GetComponent<TextMeshProUGUI>().text = "Resultados: GD:" + mediaGD + " NM:" + mediaNM + " MC:" + mediaMC;
    }*/

    IEnumerator realizarSimulacionEnParalelo() {
        simulacionCalculandose = true;
        int cantidadSimulaciones = int.Parse(inputCantidadSim.text);
        float mediaGD = 0;
        float mediaNM = 0;
        float mediaMC = 0;

        for (int i = 0; i < cantidadSimulaciones; i++) {
            float[] resultadosSimulacionActual = new float[3];
            if (dimensionesSim) {
                resultadosSimulacionActual = simulacionMetodos(int.Parse(inputIteracionesSim.text), float.Parse(inputAlphaGDSim.text), float.Parse(inputToleranciaSim.text), float.Parse(inputEpsilonSim.text), int.Parse(inputIndicadoresSim.text), false);
            } else {
                resultadosSimulacionActual = Calculos2d.simulacionMetodos(int.Parse(inputIteracionesSim.text), float.Parse(inputAlphaGDSim.text), float.Parse(inputToleranciaSim.text), float.Parse(inputEpsilonSim.text), int.Parse(inputIndicadoresSim.text), false);
            }
            
            mediaGD += resultadosSimulacionActual[0];
            mediaNM += resultadosSimulacionActual[1];
            mediaMC += resultadosSimulacionActual[2];

            // Poner el porcentaje
            textoResultadoSim.GetComponent<TextMeshProUGUI>().text = (((float)i / (float)cantidadSimulaciones) * 100) + "%";

            yield return null;
        }
        mediaGD /= cantidadSimulaciones;
        mediaNM /= cantidadSimulaciones;
        mediaMC /= cantidadSimulaciones;

        textoResultadoSim.GetComponent<TextMeshProUGUI>().text = "GD:" + mediaGD + " NM:" + mediaNM + " MC:" + mediaMC;
        simulacionCalculandose = false;
    }

    float[] simulacionMetodos(int iteracionesMaximasArg, float alphaGDArg, float toleranciaArg, float epsilon = 0.001f, int cantidadMaximaObjetivos = 3, bool mostrarDebug = false) {
        bool estadoMetodoCombinadoS=false;
        reinicializarMetodo(out float xKGradienteS, out float yKGradienteS, out float zKGradienteS);
        reinicializarMetodo(out float xKNewtonS, out float yKNewtonS, out float zKNewtonS);
        reinicializarMetodo(out float xKCombinadoS, out float yKCombinadoS, out float zKCombinadoS);

        int iteracionesGD=iteracionesMaximasArg+1;
        int iteracionesNM=iteracionesMaximasArg+1;
        int iteracionesMC=iteracionesMaximasArg+1;

        Objetivo[] objetivosS = reinicializarObjetivos(cantidadMaximaObjetivos, mostrarDebug);

        for (int i = 0; i < iteracionesMaximasArg; i++) {
            // Gradient descent
            float[] xK2G = iterarGradientDescent(xKGradienteS, yKGradienteS, zKGradienteS, alphaGDArg, objetivosS, epsilon, mostrarDebug);
            xKGradienteS = xK2G[0];
            yKGradienteS = xK2G[1];
            zKGradienteS = xK2G[2];

            // Parar si ya está cerca de un objetivo
            bool condicion = false;
            for (int j=0; j<objetivosS.Length; j++) {
                if(Mathf.Abs(objetivosS[j].a-xKGradienteS)<toleranciaArg && Mathf.Abs(objetivosS[j].b-yKGradienteS)<toleranciaArg && Mathf.Abs(objetivosS[j].d-zKGradienteS)<toleranciaArg) {
                    iteracionesGD=i;
                    condicion=true;
                    if(mostrarDebug) Debug.Log("GD ha llegado a:" + xKGradienteS + " " + yKGradienteS + " " + zKGradienteS + " El objetivo conseguido es:" + objetivosS[j].a + " " + objetivosS[j].b + " " + objetivosS[j].d);
                    break;
                }
            }
            if(condicion) break;
        }
        if(mostrarDebug) Debug.Log("Empezando NM");
        for(int i=0; i<iteracionesMaximasArg; i++) {
            // Newton method
            float[] xK2N = iterarNewtonMethod(xKNewtonS, yKNewtonS, zKNewtonS, objetivosS, epsilon, mostrarDebug);
            xKNewtonS = xK2N[0];
            yKNewtonS = xK2N[1];
            zKNewtonS = xK2N[2];

            // Parar si ya está cerca de un objetivo
            bool condicion=false;
            for(int j=0; j<objetivosS.Length; j++) {
                if(Mathf.Abs(objetivosS[j].a-xKNewtonS)<toleranciaArg && Mathf.Abs(objetivosS[j].b-yKNewtonS)<toleranciaArg && Mathf.Abs(objetivosS[j].d-zKNewtonS)<toleranciaArg) {
                    iteracionesNM=i;
                    condicion=true;
                    if(mostrarDebug) Debug.Log("NM ha llegado a:"+xKNewtonS+" "+yKNewtonS+" "+zKNewtonS+" El objetivo conseguido es:"+objetivosS[j].a+" "+objetivosS[j].b+" "+objetivosS[j].d);
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
                // Iterar GD
                float[] iteracionGD=siguienteIteracionGD(xKCombinadoS, yKCombinadoS, zKCombinadoS, alphaGDArg, objetivosS);
                // Iterar NM
                float[] iteracionNM=siguienteIteracionNM(xKCombinadoS, yKCombinadoS, zKCombinadoS, objetivosS);

                if(mostrarDebug) Debug.Log("Valor (MC GD):" + funcionPrincipal(iteracionGD[0], iteracionGD[1], iteracionGD[2], objetivosS));

                if(funcionPrincipal(iteracionGD[0], iteracionGD[1], iteracionGD[2], objetivosS) < funcionPrincipal(iteracionNM[0], iteracionNM[1], iteracionNM[2], objetivosS)) {
                    xKCombinadoS = iteracionNM[0];
                    yKCombinadoS = iteracionNM[1];
                    zKCombinadoS = iteracionNM[2];
                    estadoMetodoCombinadoS = true;
                    if(mostrarDebug) Debug.Log("GDCambioANM:"+funcionPrincipal(iteracionGD[0], iteracionGD[1], iteracionGD[2], objetivosS)+" Coordenadas GD:"+iteracionGD[0]+" "+iteracionGD[1]+" "+iteracionGD[2]+" Puntos NM:" +funcionPrincipal(iteracionNM[0], iteracionNM[1], iteracionNM[2], objetivosS)+" Coordenadas NM:"+ iteracionNM[0]+" "+iteracionNM[1]+" "+ iteracionNM[2]);
                } else {
                    // Si no hay casi puntos o se ha salido del mapa, volver a empezar
                    if((funcionPrincipal(iteracionGD[0], iteracionGD[1], iteracionGD[2], objetivosS) < epsilon) || (iteracionGD[0] > 10 || iteracionGD[0] < -10) || (iteracionGD[1] > 10 || iteracionGD[1] < -10) || (iteracionGD[2] > 10 || iteracionGD[2] < -10)) {
                        if(mostrarDebug) Debug.Log("Reseteando MC GD");
                        reinicializarMetodo(out iteracionGD[0], out iteracionGD[1], out iteracionGD[2]);
                    }
                    xKCombinadoS = iteracionGD[0];
                    yKCombinadoS = iteracionGD[1];
                    zKCombinadoS = iteracionGD[2];
                }
            } else {
                // Modo NM
                //Hacer la siguiente iteracion
                float[] xK2 = iterarNewtonMethod(xKCombinadoS, yKCombinadoS, zKCombinadoS, objetivosS, epsilon, mostrarDebug);
                xKCombinadoS = xK2[0];
                yKCombinadoS = xK2[1];
                zKCombinadoS = xK2[2];
                if(mostrarDebug) Debug.Log("Coordenadas MC NM:"+ xKCombinadoS+" "+yKCombinadoS+" "+zKCombinadoS);
            }

            // Parar si ya está cerca de un objetivo
            bool condicion=false;
            for(int j=0; j<objetivosS.Length; j++) {
                if(Mathf.Abs(objetivosS[j].a-xKCombinadoS)<toleranciaArg && Mathf.Abs(objetivosS[j].b-yKCombinadoS)<toleranciaArg && Mathf.Abs(objetivosS[j].d-zKCombinadoS)<toleranciaArg) {
                    iteracionesMC=i;
                    condicion=true;
                    if(mostrarDebug) Debug.Log("MC ha llegado a:" + xKCombinadoS + " " + yKCombinadoS + " " + zKCombinadoS + " El objetivo conseguido es:" + objetivosS[j].a + " " + objetivosS[j].b + " " + objetivosS[j].d);
                    break;
                }
            }
            if(condicion) break;
        }

        if(mostrarDebug) {
            string textoParaImprimir="IteracionesGD:"+iteracionesGD+" IteracionesNM:"+iteracionesNM+" IteracionesMC:"+iteracionesMC;
            for(int i=0; i<objetivosS.Length; i++) {
                textoParaImprimir+=" Objetivo "+i+":"+objetivosS[i].a+" "+objetivosS[i].b+" "+objetivosS[i].d;
            }
            Debug.Log(textoParaImprimir);
        }
        
        float[] resultado = { iteracionesGD, iteracionesNM, iteracionesMC };
        return resultado;
    }

    void lanzarEsfera(float potenciaArg, float anguloThetaArg, float anguloPhiArg, Color colorParaEsfera) {
        esfera.transform.position=new Vector3(0, 0.0001f, 0);
        Color colorFinal=colorParaEsfera;
        colorFinal.a *= 0.05f;
        esfera.GetComponent<MeshRenderer>().material.color = colorFinal;
        StartCoroutine(animacionEsfera(esfera, potenciaArg, anguloThetaArg, anguloPhiArg));

        yaMedido = false;
    }
    
    IEnumerator animacionEsfera(GameObject objeto, float potenciaArg, float angulo1Arg, float angulo2Arg){
        objeto.SetActive(true);

        float t=0;
        angulo1Arg*=Mathf.Deg2Rad;
        angulo2Arg*=Mathf.Deg2Rad;
        potenciaArg=(float)potenciaArg/multiplicadorPotencia;

        for(int i=0; i<1000; i++){
            t=(float)i/50;
            float radio=t*potenciaArg*Mathf.Sqrt(2)*0.5f;
            float posY=t*potenciaArg*Mathf.Sqrt(2)*0.5f -0.5f*gravedad*t*t +0.00001f;

            float posX=radio*Mathf.Sin(angulo1Arg)*Mathf.Cos(angulo2Arg);
            float posW=radio*Mathf.Sin(angulo1Arg)*Mathf.Sin(angulo2Arg);
            float posZ=radio*Mathf.Cos(angulo1Arg);
            objeto.transform.position=new Vector3(posX, posY, posZ);
            objeto.GetComponent<Object4D>().positionW=posW;

            //Debug.Log("Posicion:"+objeto.transform.position+" W:"+objeto.GetComponent<Object4D>().positionW);

            if(posY<-0.5f) break;
            yield return new WaitForSecondsRealtime(0.01f);
        }
        objeto.SetActive(false);
    }

    Objetivo[] reinicializarObjetivos(int maximosObjetivosArg, bool mostrarDebug=true) {
        int numeroAleatorio=Random.Range(1, maximosObjetivosArg+1);
        Objetivo[] resultado=new Objetivo[numeroAleatorio];

        resultado[0] = new Objetivo(Random.Range((float)-10, 10), Random.Range((float)-10, 10), Random.Range((float)-10, 10), 10);
        //resultado[0] = new Objetivo(2, 2, 2, 10);
        if(mostrarDebug) Debug.Log("Objetivo 0:  A:"+resultado[0].a +" B:"+resultado[0].b+" Z:"+resultado[0].d+" C:"+resultado[0].c);
        for(int i=1; i<numeroAleatorio; i++){
            resultado[i] = new Objetivo(Random.Range((float)-10, 10), Random.Range((float)-10, 10), Random.Range((float)-10, 10), Random.Range(1, 9));
            if(mostrarDebug) Debug.Log("Objetivo "+i+":  A:"+resultado[i].a +" B:"+resultado[i].b+" Z:"+resultado[i].d+" C:"+resultado[i].c);
        }
        return resultado;
    }

    float funcionPrincipal(float x, float y, float z, Objetivo[] objs) {
        float resultado=0;
        for(int i=0; i<objs.Length; i++) {
            resultado+=6.78f*Mathf.Log10(1+Mathf.Pow(1.405f, -((x-objs[i].a)*(x-objs[i].a) +(y-objs[i].b)*(y-objs[i].b) +(z-objs[i].d)*(z-objs[i].d) -objs[i].c)));
        }
        return resultado;
    }

    float[] gradiente(float x, float y, float z, Objetivo[] objs) {
        float resultado1 = 0;
        float resultado2 = 0;
        float resultado3 = 0;

        for(int i=0; i<objs.Length; i++) {
            float a=objs[i].a;
            float b=objs[i].b;
            float d=objs[i].d;
            float c=objs[i].c;

            float formulaPotencia=Mathf.Pow(1.405f, (-x*x -y*y -z*z -a*a -b*b -d*d +2*a*x +2*y*b +2*z*d +c));

            resultado1+=(-6.78f*Mathf.Log(1.405f)*(-2*x +2*a)* formulaPotencia) /(Mathf.Log(10)*(1+formulaPotencia));
            resultado2+=(-6.78f*Mathf.Log(1.405f)*(-2*y +2*b)* formulaPotencia) /(Mathf.Log(10)*(1+formulaPotencia));
            resultado3+=(-6.78f*Mathf.Log(1.405f)*(-2*z +2*d)* formulaPotencia) /(Mathf.Log(10)*(1+formulaPotencia));
        }
        float[] resultado = { resultado1, resultado2, resultado3 };
        return resultado;
    }

    float[] hessian(float x, float y, float z, Objetivo[] objs) {

        float derivadaXX=0;
        float derivadaXY=0;
        float derivadaXZ=0;
        float derivadaYX=0;
        float derivadaYY=0;
        float derivadaYZ=0;
        float derivadaZX=0;
        float derivadaZY=0;
        float derivadaZZ=0;

        for (int i=0; i<objs.Length; i++) {
            float a=objs[i].a;
            float b=objs[i].b;
            float d=objs[i].d;
            float c=objs[i].c;


            float funcionP=Mathf.Pow(1.405f, (-x*x -y*y -z*z -a*a -b*b -d*d +2*a*x +2*y*b +2*z*d +c));
            float funcionH=(-6.78f*Mathf.Log(1.405f)*funcionP)/(Mathf.Log(10)*(1+funcionP));

            float derivadaPx=Mathf.Log(1.405f)*(-2*x +2*a)*funcionP;
            float derivadaPy=Mathf.Log(1.405f)*(-2*y +2*b)*funcionP;
            float derivadaPz=Mathf.Log(1.405f)*(-2*z +2*d)*funcionP;

            float derivadaHx=((-6.78f*Mathf.Log(1.405f))/Mathf.Log(10))*
                ((derivadaPx*(1+funcionP)-(funcionP*derivadaPx))/((1+funcionP)*(1+funcionP)));
            float derivadaHy=((-6.78f*Mathf.Log(1.405f))/Mathf.Log(10))*
                ((derivadaPy*(1+funcionP)-(funcionP*derivadaPy))/((1+funcionP)*(1+funcionP)));
            float derivadaHz=((-6.78f*Mathf.Log(1.405f))/Mathf.Log(10))*
                ((derivadaPz*(1+funcionP)-(funcionP*derivadaPz))/((1+funcionP)*(1+funcionP)));


            derivadaXX+=-2*funcionH+((-2*x+2*a)*derivadaHx);
            derivadaXY+=(-2*x+2*a)*derivadaHy;
            derivadaXZ+=(-2*x+2*a)*derivadaHz;

            derivadaYX+=(-2*y+2*b)*derivadaHx;
            derivadaYY+=-2*funcionH+((-2*y+2*b)*derivadaHy);
            derivadaYZ+=(-2*y+2*b)*derivadaHz;

            derivadaZX+=(-2*z+2*d)*derivadaHx;
            derivadaZY+=(-2*z+2*d)*derivadaHy;
            derivadaZZ+=-2*(funcionH)+((-2*z+2*d)*derivadaHz);

        }

        float[] resultado = { derivadaXX, derivadaXY, derivadaXZ, derivadaYX, derivadaYY, derivadaYZ, derivadaZX, derivadaZY, derivadaZZ };
        return resultado;
    }

    float[] siguienteIteracionGD(float xK, float yK, float zK, float alpha, Objetivo[] objs){
        float[] gradienteEvaluado=gradiente(xK, yK, zK, objs);

        float xK2 = xK -alpha*gradienteEvaluado[0];
        float yK2 = yK -alpha*gradienteEvaluado[1];
        float zK2 = zK -alpha*gradienteEvaluado[2];

        float[] resultado = { xK2, yK2, zK2 };
        return resultado;
    }

    float[] iterarGradientDescent(float xK,  float yK, float zK, float alpha, Objetivo[] objs, float epsilon=0.001f, bool mostrarDebug = false) {
        float[] siguienteIteracion= siguienteIteracionGD(xK, yK, zK, alpha, objs);

        float xK2 = siguienteIteracion[0];
        float yK2 = siguienteIteracion[1];
        float zK2 = siguienteIteracion[2];

        if(mostrarDebug) Debug.Log("Valor:"+ funcionPrincipal(xK2, yK2, zK2, objs));
        // Si se ha salido del mapa o casi no tiene puntos, resetear
        if ((funcionPrincipal(xK2, yK2, zK2, objs)<epsilon) || xK2>10 || xK2<-10 || yK2>10 || yK2<-10 || zK2>10 || zK2 < -10) {
            reinicializarMetodo(out xK2, out yK2, out zK2);
            if(mostrarDebug) Debug.Log("ReseteandoGD:"+ xK2+" "+yK2+" "+zK2);
        }
        
        float[] resultado= { xK2, yK2, zK2 };
        return resultado;
    }

    float[] siguienteIteracionNM(float xK, float yK, float zK, Objetivo[] objs) {
        float[] gradienteEvaluado = gradiente(xK, yK, zK, objs);
        float[] hessianEvaluado = hessian(xK, yK, zK, objs);

        float[] hessian1 = { hessianEvaluado[0], hessianEvaluado[1], hessianEvaluado[2] };
        float[] hessian2 = { hessianEvaluado[3], hessianEvaluado[4], hessianEvaluado[5] };
        float[] hessian3 = { hessianEvaluado[6], hessianEvaluado[7], hessianEvaluado[8] };

        float[][] hessianFinal={ hessian1, hessian2, hessian3 };

        float determinante=hessianFinal[0][0]*hessianFinal[1][1]*hessianFinal[2][2] 
            +hessianFinal[1][0]*hessianFinal[2][1]*hessianFinal[0][2] 
            +hessianFinal[0][1]*hessianFinal[1][2]*hessianFinal[2][0] 
            -hessianFinal[2][0]*hessianFinal[1][1]*hessianFinal[0][2] 
            -hessianFinal[2][1]*hessianFinal[1][2]*hessianFinal[0][0] 
            -hessianFinal[0][1]*hessianFinal[1][0]*hessianFinal[2][2];

        if (determinante == 0) {
            float[] p = { xK, yK, zK };
            return p;
        }

        float[] matrizTranspuesta1 = { (hessianFinal[1][1]*hessianFinal[2][2] -hessianFinal[1][2]*hessianFinal[2][1]), 
            -(hessianFinal[1][0]*hessianFinal[2][2] -hessianFinal[1][2]*hessianFinal[2][0]), 
            (hessianFinal[1][0]*hessianFinal[2][1] -hessianFinal[1][1]*hessianFinal[2][0]) };
        float[] matrizTranspuesta2 = { -(hessianFinal[0][1]*hessianFinal[2][2] -hessianFinal[0][2]*hessianFinal[2][1]), 
            (hessianFinal[0][0]*hessianFinal[2][2] -hessianFinal[0][2]*hessianFinal[2][0]), 
            -(hessianFinal[0][0]*hessianFinal[2][1] -hessianFinal[0][1]*hessianFinal[2][0]) };
        float[] matrizTranspuesta3 = { (hessianFinal[0][1]*hessianFinal[1][2] -hessianFinal[0][2]*hessianFinal[1][1]), 
            -(hessianFinal[0][0]*hessianFinal[1][2] -hessianFinal[0][2]*hessianFinal[1][0]), 
            (hessianFinal[0][0]*hessianFinal[1][1] -hessianFinal[0][1]*hessianFinal[1][0]) };

        float[][] matrizTranspuesta = { matrizTranspuesta1, matrizTranspuesta2, matrizTranspuesta3 };

        float vectorFinal1=(1/determinante)*((gradienteEvaluado[0]*matrizTranspuesta[0][0])+(gradienteEvaluado[1]*matrizTranspuesta[0][1])+(gradienteEvaluado[2]*matrizTranspuesta[0][2]));
        float vectorFinal2=(1/determinante)*((gradienteEvaluado[0]*matrizTranspuesta[1][0])+(gradienteEvaluado[1]*matrizTranspuesta[1][1])+(gradienteEvaluado[2]*matrizTranspuesta[1][2]));
        float vectorFinal3=(1/determinante)*((gradienteEvaluado[0]*matrizTranspuesta[2][0])+(gradienteEvaluado[1]*matrizTranspuesta[2][1])+(gradienteEvaluado[2]*matrizTranspuesta[2][2]));


        float xK2=xK -vectorFinal1;
        float yK2=yK -vectorFinal2;
        float zK2=zK -vectorFinal3;

        float[] resultado = { xK2, yK2, zK2 };
        return resultado;
    }

    float[] iterarNewtonMethod(float xK, float yK, float zK, Objetivo[] objs, float epsilon = 0.001f, bool mostrarDebug = false) {

        float[] siguienteIteracion= siguienteIteracionNM(xK, yK, zK, objs);

        float xK2 = siguienteIteracion[0];
        float yK2 = siguienteIteracion[1];
        float zK2 = siguienteIteracion[2];

        if(mostrarDebug) Debug.Log("Valor (NM):"+ funcionPrincipal(xK2, yK2, zK2, objs));
        // Si se ha salido del mapa o casi no tiene puntos, resetear
        if ((funcionPrincipal(xK2, yK2, zK2, objs)<epsilon) || xK2>10 || xK2<-10 || yK2>10 || yK2<-10 || zK2>10 || zK2 < -10) {
            reinicializarMetodo(out xK2, out yK2, out zK2);
            if (mostrarDebug) Debug.Log("ReseteandoNM:" + xK2 + " " + yK2+" "+zK2);
        }
        float[] resultado= { xK2, yK2, zK2 };
        return resultado;
    }

    void colocarMarcador(float x, float y, float z, Color colorDelMarcador, bool mostrarDebug=false) {
        if(mostrarDebug) Debug.Log("Posicion marcador colocado:  x:"+x+" y:"+y+" z:"+z);

        GameObject marcadorCreado=GameObject.Instantiate(marcador, contenedorMarcadores.transform);
        marcadorCreado.SetActive(true);
        marcadorCreado.transform.GetChild(2).gameObject.SetActive(false);

        marcadorCreado.transform.position=new Vector3(x, ((float)(indiceMarcador+1)/1000)+0.03f, z);
        marcadorCreado.GetComponent<Object4D>().positionW=y;
        marcadorCreado.name="Marcador "+indiceMarcador;
        
        Color.RGBToHSV(colorDelMarcador, out float hue, out float saturation, out float vue);
        marcadorCreado.transform.GetChild(0).GetComponent<MeshRenderer>().material.color=Color.HSVToRGB(hue, (saturation*funcionPrincipal(x, y, z, objetivos))/10, vue);
        if(mostrarDebug) Debug.Log("Color1:"+(saturation*funcionPrincipal(x, y, z, objetivos))/10);
        
        //Color del anillo
        marcadorCreado.transform.GetChild(1).GetComponent<MeshRenderer>().material.color=colorDelMarcador;

        //Debug.Log(marcadorCreado.transform.GetChild(0).GetComponent<MeshRenderer>().material.color);
        //Debug.Log(marcadorCreado.transform.position+" "+marcadorCreado.GetComponent<Object4D>().positionW);

        indiceMarcador++;
        indiceMarcadorSeleccionado = indiceMarcador-1;
    }

    void colocarMarcadorFinal(float x, float y, float z, Color colorDelMarcador, bool mostrarDebug=false) {
        if(mostrarDebug) Debug.Log("Posicion marcador final colocado:  x:"+x+" y:"+y+" z:"+z);

        GameObject marcadorCreado=GameObject.Instantiate(marcador, contenedorMarcadores.transform);
        marcadorCreado.SetActive(true);
        marcadorCreado.transform.GetChild(2).gameObject.SetActive(false);

        marcadorCreado.transform.position=new Vector3(x, ((float)(indiceMarcador+1)/1000)+0.03f, z);
        marcadorCreado.GetComponent<Object4D>().positionW=y;
        marcadorCreado.name="Marcador "+indiceMarcador;
        
        marcadorCreado.transform.GetChild(0).GetComponent<MeshRenderer>().material.color = colorDelMarcador;
        
        //Color del anillo
        marcadorCreado.transform.GetChild(1).GetComponent<MeshRenderer>().material.color=colorDelMarcador;

        //Debug.Log(marcadorCreado.transform.GetChild(0).GetComponent<MeshRenderer>().material.color);
        //Debug.Log(marcadorCreado.transform.position+" "+marcadorCreado.GetComponent<Object4D>().positionW);

        indiceMarcador++;
        indiceMarcadorSeleccionado = indiceMarcador-1;
    }

    void colocarMarcadorConFlecha(float x, float y, float z, float magnitudX, float magnitudY, float magnitudZ, float escalaFlecha, Color colorDelMarcador, Color colorFlecha, bool mostrarDebug=false) {
        GameObject marcadorCreado=GameObject.Instantiate(marcador, contenedorMarcadores.transform);
        marcadorCreado.SetActive(true);

        marcadorCreado.transform.position=new Vector3(x, ((float)(indiceMarcador+1)/1000)+0.03f, z);
        if(mostrarDebug) Debug.Log("Altura:" + ((float)(indiceMarcador + 1) / 100000));
        marcadorCreado.GetComponent<Object4D>().positionW=y;
        marcadorCreado.name="Marcador "+indiceMarcador;
        
        Color.RGBToHSV(colorDelMarcador, out float hue, out float saturation, out float vue);
        marcadorCreado.transform.GetChild(0).GetComponent<MeshRenderer>().material.color=Color.HSVToRGB(hue, (saturation*funcionPrincipal(x, y, z, objetivos))/10, vue);
        if(mostrarDebug) Debug.Log("Color1:"+(saturation*funcionPrincipal(x, y, z, objetivos))/10);

        Transform contenedorFlechaActual = marcadorCreado.transform.GetChild(2);
        contenedorFlechaActual.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material.color = colorFlecha;
        contenedorFlechaActual.GetChild(0).GetChild(1).GetComponent<MeshRenderer>().material.color = colorFlecha;

        float[] angulosCalculados=deCartesianoAEsferico(magnitudX, magnitudY, magnitudZ);
        contenedorFlechaActual.GetComponent<Object4D>().localRotation4D=Transform4D.PlaneRotation(-angulosCalculados[2], 0, 3)*Transform4D.PlaneRotation(angulosCalculados[1], 0, 2);

        float escalaParaFlecha =escalaFlecha*angulosCalculados[0]/multiplicadorPotencia;
        contenedorFlechaActual.localScale=new Vector3(escalaParaFlecha/3, 1, escalaParaFlecha);
        contenedorFlechaActual.GetComponent<Object4D>().scaleW = escalaParaFlecha/3;

        //Color del anillo
        marcadorCreado.transform.GetChild(1).GetComponent<MeshRenderer>().material.color=colorDelMarcador;

        //Debug.Log(marcadorCreado.transform.GetChild(0).GetComponent<MeshRenderer>().material.color);
        //Debug.Log(marcadorCreado.transform.position+" "+marcadorCreado.GetComponent<Object4D>().positionW);
        //Debug.Log("X:"+x+" Y:"+y+" Z:"+z+"  MagnitudX:"+magnitudX+" MagnitudY:"+magnitudY+" MagnitudZ"+magnitudZ);
        //colocarMarcador(magnitudX,magnitudY,magnitudZ, colorDelMarcador);
        //Objetivo[] dsa={ new Objetivo(2, 2, 2, 10) };
        //Debug.Log("Objetivo: a:" + dsa[0].a + " b:" + dsa[0].b + " e:" + dsa[0].e + " c:" + dsa[0].c);
        //Debug.Log("Gradiente en 3, 3, 3: "+gradiente(3, 3, 3, dsa)[0]+" "+ gradiente(3, 3, 3, dsa)[1]+" "+ gradiente(3, 3, 3, dsa)[2]);

        indiceMarcador++;
        indiceMarcadorSeleccionado = indiceMarcador-1;
    }

    void colocarMarcadorYCercanos(float x, float y, float z, int cantidadMarcadoresCercanos, float radioMarcadoresCercanos, Color colorDeMarcadores, bool mostrarDebug=false) {
        if(mostrarDebug) Debug.Log("Colocando en:  x:"+x+" y:"+y+" z:"+z);
        colocarMarcador(x, y, z, colorDeMarcadores);

        float phi=Mathf.PI * (Mathf.Sqrt(5)-1);
        for(int i=1; i<cantidadMarcadoresCercanos; i++) {
            float posY=1-((i-1)/((float)(cantidadMarcadoresCercanos-1)))*2;
            float radioCalculado=Mathf.Sqrt(1-posY*posY);
            float theta=phi*(i-1);
            float posX=Mathf.Cos(theta)*radioCalculado*radioMarcadoresCercanos;
            float posZ=Mathf.Sin(theta)*radioCalculado*radioMarcadoresCercanos;
            colocarMarcador(x+posX, y+(posY*radioMarcadoresCercanos), z+posZ, colorDeMarcadores);
        }
    }

    void colocarMarcadorYCercanosFlecha(float x, float y, float z, int cantidadMarcadoresCercanos, float radioMarcadoresCercanos, float escalaFlechas, Color colorDeMarcadores, Color colorDeFlechas, bool mostrarDebug=false) {
        float[] gradienteEvaluado = gradiente(x, y, z, objetivos);

        colocarMarcadorConFlecha(x, y, z, -gradienteEvaluado[0], -gradienteEvaluado[1], -gradienteEvaluado[2], escalaFlechas, colorDeMarcadores, colorDeFlechas);
        if(mostrarDebug) Debug.Log("Marcador central en:  x:"+x+" y:"+y+" z:"+z);
        float phi=Mathf.PI * (Mathf.Sqrt(5)-1);
        for(int i=1; i<cantidadMarcadoresCercanos; i++) {
            float posY=1-((i-1)/((float)(cantidadMarcadoresCercanos-1)))*2;
            float radioCalculado=Mathf.Sqrt(1-posY*posY);
            float theta=phi*(i-1);
            float posX=Mathf.Cos(theta)*radioCalculado*radioMarcadoresCercanos;
            float posZ=Mathf.Sin(theta)*radioCalculado*radioMarcadoresCercanos;
            posY*=radioMarcadoresCercanos;

            gradienteEvaluado = gradiente(x+posX, y+posY, z+posZ, objetivos);

            colocarMarcadorConFlecha(x+posX, y+posY, z+posZ, -gradienteEvaluado[0], -gradienteEvaluado[1], -gradienteEvaluado[2], escalaFlechas, colorDeMarcadores, colorDeFlechas);
            if(mostrarDebug) Debug.Log("Marcador auxiliar "+i+" en:  x:"+(x+posX)+" y:"+(y+posY)+" z:"+(z+posZ));
        }
    }

    float[] deCartesianoAEsferico(float x, float y, float z) {
        float escalaCalculada=multiplicadorPotencia*Mathf.Sqrt(gravedad*Mathf.Sqrt(x*x +y*y +z*z));
        float anguloThetaCalculado=0;
        float anguloPhiCalculado=0;

        if (z > 0) {
            anguloThetaCalculado=Mathf.Atan(Mathf.Sqrt(x*x +y*y)/z);
        } else if(z<0) {
            anguloThetaCalculado=Mathf.PI+Mathf.Atan(Mathf.Sqrt(x*x +y*y)/z);
        } else if(z==0 && Mathf.Sqrt(x*x +y*y)!=0){
            anguloThetaCalculado=Mathf.PI/2;
        }

        if (x > 0) {
            anguloPhiCalculado=Mathf.Atan(y/x);
        } else if(x<0 && y>=0) {
            anguloPhiCalculado=Mathf.PI+Mathf.Atan(y/x);
        } else if(x<0 && y<0) {
            anguloPhiCalculado=-Mathf.PI+Mathf.Atan(y/x);
        } else if(x==0 && y>0) {
            anguloPhiCalculado=Mathf.PI/2;
        } else if(x==0 && y<0) {
            anguloPhiCalculado=-Mathf.PI/2;
        }

        anguloThetaCalculado*=Mathf.Rad2Deg;
        anguloPhiCalculado*=Mathf.Rad2Deg;

        // Evitar numeros negativos
        if(anguloThetaCalculado < 0) anguloThetaCalculado+=360;
        if(anguloPhiCalculado < 0) anguloPhiCalculado+=360;

        float[] resultado = { escalaCalculada, anguloThetaCalculado, anguloPhiCalculado };
        return resultado;
    }
    
    void reinicializarMetodo(out float xK, out float yK, out float zK) {
        xK = Random.Range((float)-10, 10);
        yK = Random.Range((float)-10, 10);
        zK = Random.Range((float)-10, 10);
    }



    protected virtual void FixedUpdate() {
        //Handle the physics for the player
        //NOTE: Player doesn't move quickly, it's okay to update every other frame.
        oddFrame = !oddFrame;
        if (!locked && oddFrame) {
            //Double the time-step since this only happens every other frame.
            colliderPosition4D = UpdatePhysics(colliderPosition4D, 2.0f * Time.fixedDeltaTime);
        }
    }

    protected virtual void UpdateZoom() {
        targetZoom *= Mathf.Pow(ZOOM_RATE, -InputManager.GetAxis(InputManager.AxisBind.Zoom));
        targetZoom = Mathf.Clamp(targetZoom, ZOOM_MIN, ZOOM_MAX);
        float zoomSmooth = Mathf.Pow(2.0f, -Time.deltaTime / ZOOM_SMOOTH);
        zoom = zoom * zoomSmooth + targetZoom * (1.0f - zoomSmooth);
    }

    public override Vector4 camPosition4D {
        get {
            Vector4 result = position4D;
            result += smoothGravityDirection * CamHeight();
            return result;
        }
        set {
            Vector4 result = value;
            result -= smoothGravityDirection * CamHeight();
            position4D = result;
        }
    }

    public Vector4 colliderPosition4D {
        get {
            Vector4 result = position4D;
            result += smoothGravityDirection * colliderRadius;
            return result;
        }
        set {
            Vector4 result = value;
            result -= smoothGravityDirection * colliderRadius;
            position4D = result;
        }
    }

    public Matrix4x4 CreateCamMatrix(Quaternion m1Rot, float yz) {
        //Up-Forward
        Matrix4x4 mainRot = Transform4D.Slerp(Transform4D.PlaneRotation(yz, 1, 2), Transform4D.PlaneRotation(90.0f, 1, 3), volumeSmooth);

        //Combine with secondary rotation
        return gravityMatrix * Transform4D.SkipY(m1Rot) * mainRot;
    }

    public void UpdateCameraMask() {
        //NOTE: Using the static variable from CameraControl4D intentionally so it affects both
        if (!sliceEnabled && shadowMode == 0) { shadowMode = 1; }
        if (CameraControl4D.PROJECTION_MODE != 0 && shadowMode == 1) { shadowMode = 2; }
        UpdateCameraMask(shadowMode, sliceEnabled);
    }

    struct Objetivo {
        public float a; //Pos x
        public float b; //Pos y
        public float d; //Pos z
        public float c; //Puntos

        public Objetivo(float argA, float argB, float argD, float argC) {
            a = argA;
            b = argB;
            d = argD;
            c = argC;
        }
    }

    class Calculos2d {

        public static float funcionPrincipal(float x, float y, Objetivo2d[] objs) {
            float resultado=0;
            for(int i=0; i<objs.Length; i++) {
                resultado=resultado + 6.78f*Mathf.Log10(1+Mathf.Pow(1.405f, -((x-objs[i].a)*(x-objs[i].a) + (y-objs[i].b)*(y-objs[i].b) -objs[i].c)));
            }
            return resultado;
        }

        public static float[] gradiente(float x, float y, Objetivo2d[] objs) {

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

        public static float[] hessian(float x, float y, Objetivo2d[] objs) {

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

        public static float[] iterarGradientDescent(float xK, float yK, float alpha, Objetivo2d[] objs, float epsilon = 0.001f, bool mostrarDebug=false) {
            float[] gradienteEvaluado=gradiente(xK, yK, objs);

            float xK2 = xK -alpha*gradienteEvaluado[0];
            float yK2 = yK -alpha*gradienteEvaluado[1];

            // Si no hay casi puntos o se ha salido del mapa, volver a empezar
            if((funcionPrincipal(xK2, yK2, objs)<epsilon) || (xK2>10 || xK2<-10) || (yK2>10 || yK2<-10)) {
                xK2=Random.Range((float)-10, 10);
                yK2=Random.Range((float)-10, 10);
                if(mostrarDebug) Debug.Log("ReseteandoGD:"+ xK2+" "+yK2);
            }
            if(mostrarDebug) Debug.Log("Valor:"+ funcionPrincipal(xK2, yK2, objs));

            float[] resultado = {xK2, yK2};
            return resultado;
        }

        public static float[] iterarNewtonMethod(float xK, float yK, Objetivo2d[] objs, float epsilon = 0.001f, bool mostrarDebug=false) {
            float[] xK2= siguienteIteracionNewton(xK, yK, objs);

            if(mostrarDebug) Debug.Log("Valor (NM):"+ funcionPrincipal(xK2[0], xK2[1], objs));
            // Si no hay casi puntos o se ha salido del mapa, volver a empezar
            if((funcionPrincipal(xK2[0], xK2[1], objs)<epsilon) || (xK2[0]>10 || xK2[0]<-10) || (xK2[1]>10 || xK2[1]<-10)) {
                xK2[0]=Random.Range((float)-10, 10);
                xK2[1]=Random.Range((float)-10, 10);
                if(mostrarDebug) Debug.Log("ReseteandoNM:" + xK2[0] + " " + xK2[1]);
            }

            float[] resultado = {xK2[0], xK2[1]};
            return resultado;
        }

        public static float[] siguienteIteracionNewton(float xK, float yK, Objetivo2d[] objs) {
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

        public static float[] simulacionMetodos(int iteracionesMaximasArg, float alphaGDArg, float toleranciaArg, float epsilon = 0.001f, int cantidadMaximaObjetivos = 3, bool mostrarDebug=false) {
        
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

            Objetivo2d[] objetivosS=reinicializarObjetivos(cantidadMaximaObjetivos, mostrarDebug);

            for(int i=0; i<iteracionesMaximasArg; i++) {
                // Gradient descent
                float[] xK2G = iterarGradientDescent(xKGradienteS, yKGradienteS, alphaGDArg, objetivosS, epsilon, mostrarDebug);
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
                float[] xK2N = iterarNewtonMethod(xKNewtonS, yKNewtonS, objetivosS, epsilon, mostrarDebug);
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
                    float xKGradiente2 = xKCombinadoS - alphaGDArg * gradienteEvaluadoGD[0];
                    float yKGradiente2 = yKCombinadoS - alphaGDArg * gradienteEvaluadoGD[1];

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
                        if((funcionPrincipal(xKGradiente2, yKGradiente2, objetivosS) < epsilon) || (xKGradiente2 > 10 || xKGradiente2 < -10) || (yKGradiente2 > 10 || yKGradiente2 < -10)) {
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
                    float[] xK2 = iterarNewtonMethod(xKCombinadoS, yKCombinadoS, objetivosS, epsilon, mostrarDebug);
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
        }

        public static Objetivo2d[] reinicializarObjetivos(int maximosObjetivosArg, bool mostrarDebug = true) {
            int numeroAleatorio = Random.Range(1, maximosObjetivosArg + 1);

            Objetivo2d[] resultado = new Objetivo2d[numeroAleatorio];

            resultado[0] = new Objetivo2d(Random.Range((float)-10, 10), Random.Range((float)-10, 10), 10);
            if (mostrarDebug) Debug.Log("Objetivo 0:  A:" + resultado[0].a + " B:" + resultado[0].b + " C:" + resultado[0].c);
            for (int i = 1; i < (numeroAleatorio); i++) {
                resultado[i] = new Objetivo2d(Random.Range((float)-10, 10), Random.Range((float)-10, 10), Random.Range(1, 9));
                if (mostrarDebug) Debug.Log("Objetivo " + i + ":  A:" + resultado[i].a + " B:" + resultado[i].b + " C:" + resultado[i].c);
            }
            return resultado;
        }

        

    }
    
    public class Objetivo2d {
        public float a;
        public float b;
        public float c;

        public Objetivo2d(float parametroA, float parametroB, float parametroC) {
            a = parametroA;
            b = parametroB;
            c = parametroC;
        }
    }



}

