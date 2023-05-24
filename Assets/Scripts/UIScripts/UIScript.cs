using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIScript : MonoBehaviour
{
    private enum estatsSimulacio { SIMULANT, ATURAT, PAUSAT };
    private estatsSimulacio estat;
    private enum btnSeleccionat { GENERADOR, CUA, PROCESSADOR, SORTIDA, JUNTAR, DESJUNTAR, ELIMINAR, CAP, ENTITATSTEMPORALS };
    private btnSeleccionat seleccionat;
    public Button generadorButton;
    public Button cuaButton;
    public Button processadorButton;
    public Button sortidaButton;
    public Button juntarButton;
    public Button desjuntarButton;
    public Button eliminarButton;
    public Button comencarPausar;
    public Button reiniciar;
    public Button entitatsTemporals;
    public Button seguentEsdev;

    public GameObject motorSimulador;
    private GameObject generadorPrefab;
    private GameObject cuaPrefab;

    private GameObject processadorPrefab;
    private GameObject sortidaPrefab;
    private static UIScript instancia;

    private UIScript() { }

    public static UIScript Instancia {
        get {
            if (instancia == null){
                instancia = FindObjectOfType<UIScript>();

                if (instancia == null){
                    GameObject singletonObject = new GameObject();
                    instancia = singletonObject.AddComponent<UIScript>();
                    singletonObject.name = typeof(UIScript).ToString();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instancia;
        }
    }




    [SerializeField] public Texture2D[] imatgesCursor = new Texture2D[6];
    [SerializeField] public Texture2D[] imatgesStartPause = new Texture2D[2];
    private GameObject[] ajuntar = new GameObject[2];
    private GameObject[] desjuntar = new GameObject[2];

    // Start is called before the first frame update
    void Start()
    {
        seleccionat = btnSeleccionat.CAP;
        estat = estatsSimulacio.ATURAT;
        comencarPausar.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(imatgesStartPause[0], new Rect(0, 0, imatgesStartPause[0].width, imatgesStartPause[0].height), new Vector2(0.5f, 0.5f));

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            if (estat == estatsSimulacio.ATURAT && objecteLlibreriaSeleccionat() && !RatoliSobreBotonsUI())
            {
                if (seleccionat == btnSeleccionat.GENERADOR || seleccionat == btnSeleccionat.CUA || seleccionat == btnSeleccionat.PROCESSADOR || seleccionat == btnSeleccionat.SORTIDA) instanciarObjecte();
            }  
            if ((motorSimulador.GetComponent<MotorSimuladorScript>().AlgunDetallsObert()) && (!RatoliSobreAlgunObjece() && !RatoliSobreDetalls())){
                motorSimulador.GetComponent<MotorSimuladorScript>().TancaDetallsObert();
            }
            if (estat == estatsSimulacio.ATURAT && seleccionat == btnSeleccionat.ENTITATSTEMPORALS){
                if (!RatoliSobreDetallsEntitats()){
                    entitatsTemporals.gameObject.transform.parent.transform.GetChild(1).gameObject.SetActive(false);
                    seleccionarOpcio(btnSeleccionat.CAP);
                }
            }
        }
    }

    public bool RatoliSobreBotonsUI(){
        return 
            RectTransformUtility.RectangleContainsScreenPoint(generadorButton.GetComponent<RectTransform>(), Input.mousePosition) ||
            RectTransformUtility.RectangleContainsScreenPoint(cuaButton.GetComponent<RectTransform>(), Input.mousePosition) ||
            RectTransformUtility.RectangleContainsScreenPoint(processadorButton.GetComponent<RectTransform>(), Input.mousePosition) ||
            RectTransformUtility.RectangleContainsScreenPoint(sortidaButton.GetComponent<RectTransform>(), Input.mousePosition) ||
            RectTransformUtility.RectangleContainsScreenPoint(juntarButton.GetComponent<RectTransform>(), Input.mousePosition) ||
            RectTransformUtility.RectangleContainsScreenPoint(desjuntarButton.GetComponent<RectTransform>(), Input.mousePosition) ||
            RectTransformUtility.RectangleContainsScreenPoint(eliminarButton.GetComponent<RectTransform>(), Input.mousePosition) ||
            RectTransformUtility.RectangleContainsScreenPoint(entitatsTemporals.GetComponent<RectTransform>(), Input.mousePosition);
    }

    public bool RatoliSobreDetallsEntitats(){

        var image = entitatsTemporals.gameObject.transform.parent.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Image>();
        if (RectTransformUtility.RectangleContainsScreenPoint(image.rectTransform, Input.mousePosition))
        {
            return true;
        }
        return false;
    }

    public bool objecteLlibreriaSeleccionat(){
        return seleccionat == btnSeleccionat.GENERADOR || seleccionat == btnSeleccionat.CUA || seleccionat == btnSeleccionat.PROCESSADOR || seleccionat == btnSeleccionat.SORTIDA;   
    }

    public bool RatoliSobreDetalls(){
        return motorSimulador.GetComponent<MotorSimuladorScript>().RatoliSobreDetalls();
    }

    public bool RatoliSobreAlgunObjece(){
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -Camera.main.transform.position.z;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPosition, Camera.main.transform.forward);

        if (hit.collider != null && hit.collider.CompareTag("ObjecteLlibreria"))
        {
            return true;
        }
        return false;
    }

    public void botoGeneradorClicat(){
        seleccionarOpcio(btnSeleccionat.GENERADOR);
    }

    public void botoCuaClicat(){
        seleccionarOpcio(btnSeleccionat.CUA);
    }

    public void botoProcessadorClicat(){
        seleccionarOpcio(btnSeleccionat.PROCESSADOR);
    }

    public void botoSortidaClicat(){
        seleccionarOpcio(btnSeleccionat.SORTIDA);
    }

    public void botoJuntarClicat(){
        seleccionarOpcio(btnSeleccionat.JUNTAR);
    }

    public void botoDesjuntarClicat(){
        seleccionarOpcio(btnSeleccionat.DESJUNTAR);
    }

    public void botoEliminarClicat(){
        seleccionarOpcio(btnSeleccionat.ELIMINAR);
    }

    public void botoEntitatsTemporalsClicat(){
        seleccionarOpcio(btnSeleccionat.ENTITATSTEMPORALS);
        entitatsTemporals.gameObject.transform.parent.transform.GetChild(1).gameObject.SetActive(true);
    }

    public void botoComencarPausaClicat(){
        if (estat == estatsSimulacio.SIMULANT) {
            estat = estatsSimulacio.PAUSAT;
            comencarPausar.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(imatgesStartPause[0], new Rect(0, 0, imatgesStartPause[0].width, imatgesStartPause[0].height), new Vector2(0.5f, 0.5f));
        } else if (estat == estatsSimulacio.PAUSAT){
            seleccionarOpcio(btnSeleccionat.CAP);
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            comencarPausar.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(imatgesStartPause[1], new Rect(0, 0, imatgesStartPause[1].width, imatgesStartPause[1].height), new Vector2(0.5f, 0.5f));
            estat = estatsSimulacio.SIMULANT;
        } else {  
            comencarPausar.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(imatgesStartPause[1], new Rect(0, 0, imatgesStartPause[1].width, imatgesStartPause[1].height), new Vector2(0.5f, 0.5f));
            estat = estatsSimulacio.SIMULANT;
            motorSimulador.GetComponent<MotorSimuladorScript>().IniciaSimulacio();
        }
    }

    public void botoReiniciaClicat(){
        estat = estatsSimulacio.ATURAT;
        comencarPausar.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(imatgesStartPause[0], new Rect(0, 0, imatgesStartPause[1].width, imatgesStartPause[0].height), new Vector2(0.5f, 0.5f));
        motorSimulador.GetComponent<MotorSimuladorScript>().ReiniciarSimulador();
        seguentEsdev.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Següent Esdeveniment\nTemps (u.t.): 0";
    }

    public void botoStepClicat(){
        if (estat != estatsSimulacio.SIMULANT){
            motorSimulador.GetComponent<MotorSimuladorScript>().ExecutarSeguentEsdeveniment();
            float tempsSegEsdv = motorSimulador.GetComponent<MotorSimuladorScript>().ObteTempsSeguentEsdeveniment();
            seguentEsdev.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Següent Esdeveniment\nTemps (u.t.): " + tempsSegEsdv.ToString();
        }
    }

    private void seleccionarOpcio(btnSeleccionat seleccionatNou){
        if (estat == estatsSimulacio.ATURAT){
            if (seleccionat == btnSeleccionat.JUNTAR) ajuntar = new GameObject[2];
            else if (seleccionat == btnSeleccionat.DESJUNTAR) desjuntar = new GameObject[2];
            if (seleccionatNou != seleccionat){
                deseleccionarBackground(seleccionat);
                if (seleccionatNou != btnSeleccionat.CAP){
                    seleccionarBackground(seleccionatNou);
                }
                seleccionat = seleccionatNou;
            }
            else if (seleccionatNou == seleccionat){
                deseleccionarBackground(seleccionatNou);
                seleccionat = btnSeleccionat.CAP;
                seleccionarBackground(seleccionat);
            }
        }
    }

    private void deseleccionarBackground(btnSeleccionat deseleccionar){
        // Afegir canvi d'imatge
        switch (deseleccionar){
            case btnSeleccionat.GENERADOR:
                generadorButton.GetComponent<Image>().color = Color.white;
                break;
            case btnSeleccionat.CUA:
                cuaButton.GetComponent<Image>().color = Color.white;
                break;
            case btnSeleccionat.PROCESSADOR:
                processadorButton.GetComponent<Image>().color = Color.white;
                break;
            case btnSeleccionat.SORTIDA:
                sortidaButton.GetComponent<Image>().color = Color.white;
                break;
            case btnSeleccionat.JUNTAR:
                juntarButton.GetComponent<Image>().color = Color.white;
                break;
            case btnSeleccionat.DESJUNTAR:
                desjuntarButton.GetComponent<Image>().color = Color.white;
                break;
            case btnSeleccionat.ELIMINAR:
                eliminarButton.GetComponent<Image>().color = Color.white;
                break;
            default:
                break;
        }
    }

    private void seleccionarBackground(btnSeleccionat seleccionatNou){
        switch (seleccionatNou){
            case btnSeleccionat.GENERADOR:
                generadorButton.GetComponent<Image>().color = Color.green;
                Cursor.SetCursor(imatgesCursor[0], Vector2.zero, CursorMode.Auto);
                break;
            case btnSeleccionat.CUA:
                cuaButton.GetComponent<Image>().color = Color.green;
                Cursor.SetCursor(imatgesCursor[1], Vector2.zero, CursorMode.Auto);
                break;
            case btnSeleccionat.PROCESSADOR:
                processadorButton.GetComponent<Image>().color = Color.green;
                Cursor.SetCursor(imatgesCursor[2], Vector2.zero, CursorMode.Auto);
                break;
            case btnSeleccionat.SORTIDA:
                sortidaButton.GetComponent<Image>().color = Color.green;
                Cursor.SetCursor(imatgesCursor[3], Vector2.zero, CursorMode.Auto);
                break;
            case btnSeleccionat.JUNTAR:
                juntarButton.GetComponent<Image>().color = Color.green;
                Cursor.SetCursor(imatgesCursor[4], Vector2.zero, CursorMode.Auto);
                break;
            case btnSeleccionat.DESJUNTAR:
                desjuntarButton.GetComponent<Image>().color = Color.green;
                Cursor.SetCursor(imatgesCursor[5], Vector2.zero, CursorMode.Auto);
                break;
            case btnSeleccionat.ELIMINAR:
                eliminarButton.GetComponent<Image>().color = Color.red;
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                break;
        }
    }

    private void instanciarObjecte(){
        Vector3 mousePosition = Input.mousePosition;
        // Convert the mouse position to a world position relative to the camera
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        switch (seleccionat){
            case btnSeleccionat.GENERADOR:
                motorSimulador.GetComponent<MotorSimuladorScript>().creaObjecteFill(0, worldPosition);
                deseleccionarBackground(seleccionat);
                seleccionat = btnSeleccionat.CAP;
                seleccionarBackground(seleccionat);
                break;
            case btnSeleccionat.CUA:
                motorSimulador.GetComponent<MotorSimuladorScript>().creaObjecteFill(1, worldPosition);
                deseleccionarBackground(seleccionat);
                seleccionat = btnSeleccionat.CAP;
                seleccionarBackground(seleccionat);
                break;
            case btnSeleccionat.PROCESSADOR:
                motorSimulador.GetComponent<MotorSimuladorScript>().creaObjecteFill(2, worldPosition);
                deseleccionarBackground(seleccionat);
                seleccionat = btnSeleccionat.CAP;
                seleccionarBackground(seleccionat);
                break;
            case btnSeleccionat.SORTIDA:
                motorSimulador.GetComponent<MotorSimuladorScript>().creaObjecteFill(3, worldPosition);
                deseleccionarBackground(seleccionat);
                seleccionat = btnSeleccionat.CAP;
                seleccionarBackground(seleccionat);
                break;
        }
    }

    public void ajuntarObjectes(GameObject objectePerJuntar){
        if (ajuntar[0] == null) ajuntar[0] = objectePerJuntar;
        else{
            ajuntar[1] = objectePerJuntar;
            ajuntar[0].GetComponent<IObjectes>().afegeixSeguentObjecte(ajuntar[1]);
            ajuntar = new GameObject[2];
            seleccionarOpcio(btnSeleccionat.JUNTAR);
        }
    }

    public void desjuntarObjectes(GameObject objectePerDesjuntar){
        if (desjuntar[0] == null) desjuntar[0] = objectePerDesjuntar;
        else{
            desjuntar[1] = objectePerDesjuntar;
            desjuntar[0].GetComponent<IObjectes>().desajuntarSeguentObjecte(desjuntar[1]);
            desjuntar = new GameObject[2];
            seleccionarOpcio(btnSeleccionat.DESJUNTAR);
        }
    }

    public int obteEstatSimulador(){
        return (int)estat;
    }

    public int obteBotoSeleccionat(){
        return (int)seleccionat;
    }
}
