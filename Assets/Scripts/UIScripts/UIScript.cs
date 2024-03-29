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
    private enum btnSeleccionat { GENERADOR, CUA, PROCESSADOR, SORTIDA, JUNTAR, DESJUNTAR, ELIMINAR, CAP, CONFIG, ERROR };
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
    public Button configuracio;
    public Button seguentEsdev;
    public Button sortir;

    private GameObject generadorPrefab;
    private GameObject cuaPrefab;

    private GameObject processadorPrefab;
    private GameObject sortidaPrefab;
    public GameObject contentView;
    public GameObject prefabLogs;
    public GameObject finestraErrors;
    public Scrollbar barra;
    public TextMeshProUGUI tempsActual;
    public GameObject logs;

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




    [SerializeField] public Texture2D[] imatgesStartPause = new Texture2D[2];
    private GameObject[] ajuntar = new GameObject[2];
    private GameObject[] desjuntar = new GameObject[2];

    // Start is called before the first frame update
    void Start()
    {
        seleccionat = btnSeleccionat.CAP;
        estat = estatsSimulacio.ATURAT;
        //comencarPausar.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(imatgesStartPause[0], new Rect(0, 0, imatgesStartPause[0].width, imatgesStartPause[0].height), new Vector2(0.5f, 0.5f));

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            if (estat == estatsSimulacio.ATURAT && ObjecteLlibreriaSeleccionat() && !RatoliSobreBotonsUI())
            {
                if (seleccionat == btnSeleccionat.GENERADOR || seleccionat == btnSeleccionat.CUA || seleccionat == btnSeleccionat.PROCESSADOR || seleccionat == btnSeleccionat.SORTIDA) InstanciarObjecte();
            }  
            if ((MotorSimuladorScript.Instancia.AlgunDetallsObert()) && (!RatoliSobreAlgunObjece() && !RatoliSobreDetalls())){
                MotorSimuladorScript.Instancia.TancaDetallsObert();
            }
            if (estat == estatsSimulacio.ATURAT && seleccionat == btnSeleccionat.CONFIG){
                if (!RatoliSobreDetallsConfiguracio()){
                    configuracio.gameObject.transform.parent.transform.GetChild(1).gameObject.SetActive(false);
                    SeleccionarOpcio(btnSeleccionat.CAP);
                }
            }
        }

        
        
    }

    public void aturarAplicacio(){
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
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
            RectTransformUtility.RectangleContainsScreenPoint(configuracio.GetComponent<RectTransform>(), Input.mousePosition);
    }

    public bool RatoliSobreDetallsConfiguracio(){

        var image = configuracio.gameObject.transform.parent.transform.GetChild(1).gameObject.transform.GetChild(0).GetComponent<Image>();
        if (RectTransformUtility.RectangleContainsScreenPoint(image.rectTransform, Input.mousePosition))
        {
            return true;
        }
        return false;
    }

    public bool ObjecteLlibreriaSeleccionat(){
        return seleccionat == btnSeleccionat.GENERADOR || seleccionat == btnSeleccionat.CUA || seleccionat == btnSeleccionat.PROCESSADOR || seleccionat == btnSeleccionat.SORTIDA;   
    }

    public bool RatoliSobreDetalls(){
        return MotorSimuladorScript.Instancia.RatoliSobreDetalls();
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

    public void BotoGeneradorClicat(){
        if (seleccionat != btnSeleccionat.ERROR) SeleccionarOpcio(btnSeleccionat.GENERADOR);
    }

    public void BotoCuaClicat(){
        if (seleccionat != btnSeleccionat.ERROR) SeleccionarOpcio(btnSeleccionat.CUA);
    }

    public void BotoProcessadorClicat(){
        if (seleccionat != btnSeleccionat.ERROR) SeleccionarOpcio(btnSeleccionat.PROCESSADOR);
    }

    public void BotoSortidaClicat(){
        if (seleccionat != btnSeleccionat.ERROR) SeleccionarOpcio(btnSeleccionat.SORTIDA);
    }

    public void BotoJuntarClicat(){
        if (seleccionat != btnSeleccionat.ERROR) SeleccionarOpcio(btnSeleccionat.JUNTAR);
    }

    public void BotoDesjuntarClicat(){
        if (seleccionat != btnSeleccionat.ERROR) SeleccionarOpcio(btnSeleccionat.DESJUNTAR);
    }

    public void BotoEliminarClicat(){
        if (seleccionat != btnSeleccionat.ERROR) SeleccionarOpcio(btnSeleccionat.ELIMINAR);
    }

    public void BotoConfiguracioClicat(){
        if (seleccionat != btnSeleccionat.ERROR) {
            SeleccionarOpcio(btnSeleccionat.CONFIG);
            configuracio.gameObject.transform.parent.transform.GetChild(1).gameObject.SetActive(true);
        }
    }

    public void BotoComencarPausaClicat(){
        if (seleccionat != btnSeleccionat.ERROR){
            if (estat == estatsSimulacio.SIMULANT) {
                estat = estatsSimulacio.PAUSAT;
                comencarPausar.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(imatgesStartPause[0], new Rect(0, 0, imatgesStartPause[0].width, imatgesStartPause[0].height), new Vector2(0.5f, 0.5f));
            } else if (estat == estatsSimulacio.PAUSAT){
                SeleccionarOpcio(btnSeleccionat.CAP);
                comencarPausar.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(imatgesStartPause[1], new Rect(0, 0, imatgesStartPause[1].width, imatgesStartPause[1].height), new Vector2(0.5f, 0.5f));
                estat = estatsSimulacio.SIMULANT;
                logs.SetActive(true);
            } else {  
                comencarPausar.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(imatgesStartPause[1], new Rect(0, 0, imatgesStartPause[1].width, imatgesStartPause[1].height), new Vector2(0.5f, 0.5f));
                estat = estatsSimulacio.SIMULANT;
                if (MotorSimuladorScript.Instancia.estat == MotorSimuladorScript.estats.ATURAT) MotorSimuladorScript.Instancia.IniciaSimulacio();
                logs.SetActive(true);
            }
        }
    }

    public void BotoReiniciaClicat(){
        if (seleccionat != btnSeleccionat.ERROR){
            estat = estatsSimulacio.ATURAT;
            comencarPausar.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(imatgesStartPause[0], new Rect(0, 0, imatgesStartPause[1].width, imatgesStartPause[0].height), new Vector2(0.5f, 0.5f));
            MotorSimuladorScript.Instancia.ReiniciarSimulador();
            logs.SetActive(false);
            seguentEsdev.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Següent Esdeveniment\nTemps (s): 0";
            foreach (Transform child in contentView.transform) {
                Destroy(child.gameObject);
            }

            ReiniciarBarra();
        }
    }

    public void BotoStepClicat(){
        if (seleccionat != btnSeleccionat.ERROR){
            if (estat != estatsSimulacio.SIMULANT){
                //estat = estatsSimulacio.SIMULANT;
                logs.SetActive(true);
                MotorSimuladorScript.Instancia.ExecutarSeguentEsdeveniment();
                float tempsSegEsdv = MotorSimuladorScript.Instancia.ObteTempsSeguentEsdeveniment();
                seguentEsdev.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "Següent Esdeveniment\nTemps (s): " + tempsSegEsdv.ToString();
            }
        }
    }

    public void MostrarError(string missatgeError){
        finestraErrors.transform.GetChild(0).transform.GetChild(0).GetComponent<TMP_Text>().text = missatgeError;
        finestraErrors.SetActive(true);
        estat = estatsSimulacio.ATURAT;
        SeleccionarOpcio(btnSeleccionat.ERROR);
    }

    public void AcceptarError(){
        finestraErrors.SetActive(false);
        SeleccionarOpcio(btnSeleccionat.CAP);
    }

    private void SeleccionarOpcio(btnSeleccionat seleccionatNou){
        if (estat == estatsSimulacio.ATURAT){
            if (seleccionat == btnSeleccionat.JUNTAR) ajuntar = new GameObject[2];
            else if (seleccionat == btnSeleccionat.DESJUNTAR) desjuntar = new GameObject[2];
            if (seleccionatNou != seleccionat){
                DeseleccionarBackground(seleccionat);
                if (seleccionatNou != btnSeleccionat.CAP){
                    SeleccionarBackground(seleccionatNou);
                }
                seleccionat = seleccionatNou;
            }
            else if (seleccionatNou == seleccionat){
                DeseleccionarBackground(seleccionatNou);
                seleccionat = btnSeleccionat.CAP;
                SeleccionarBackground(seleccionat);
            }
        }
    }

    private void DeseleccionarBackground(btnSeleccionat deseleccionar){
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

    private void SeleccionarBackground(btnSeleccionat seleccionatNou){
        switch (seleccionatNou){
            case btnSeleccionat.GENERADOR:
                generadorButton.GetComponent<Image>().color = Color.green;
                break;
            case btnSeleccionat.CUA:
                cuaButton.GetComponent<Image>().color = Color.green;
                break;
            case btnSeleccionat.PROCESSADOR:
                processadorButton.GetComponent<Image>().color = Color.green;
                break;
            case btnSeleccionat.SORTIDA:
                sortidaButton.GetComponent<Image>().color = Color.green;
                break;
            case btnSeleccionat.JUNTAR:
                juntarButton.GetComponent<Image>().color = Color.green;
                break;
            case btnSeleccionat.DESJUNTAR:
                desjuntarButton.GetComponent<Image>().color = Color.green;
                break;
            case btnSeleccionat.ELIMINAR:
                eliminarButton.GetComponent<Image>().color = Color.red;
                break;
            default:
                break;
        }
    }

    private void InstanciarObjecte(){
        Vector3 mousePosition = Input.mousePosition;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        int objecteId;
        switch (seleccionat){
            case btnSeleccionat.GENERADOR:
                objecteId = 0;
                break;
            case btnSeleccionat.CUA:
                objecteId = 1;
                break;
            case btnSeleccionat.PROCESSADOR:
                objecteId = 2;
                break;
            case btnSeleccionat.SORTIDA:
                objecteId = 3;                
                break;
            default:
                objecteId = -1;
                break;
        }

        if (objecteId != -1){
            MotorSimuladorScript.Instancia.CreaObjecteFill(objecteId, worldPosition);
            DeseleccionarBackground(seleccionat);
            seleccionat = btnSeleccionat.CAP;
            SeleccionarBackground(seleccionat);
        }
    }

    public void AjuntarObjectes(GameObject objectePerJuntar){
        if (ajuntar[0] == null) ajuntar[0] = objectePerJuntar;
        else{
            ajuntar[1] = objectePerJuntar;
            ajuntar[0].GetComponent<LlibreriaObjectes>().AfegeixSeguentObjecte(ajuntar[1]);
            ajuntar = new GameObject[2];
            SeleccionarOpcio(btnSeleccionat.JUNTAR);
        }
    }

    public void DesjuntarObjectes(GameObject objectePerDesjuntar){
        if (desjuntar[0] == null) desjuntar[0] = objectePerDesjuntar;
        else{
            desjuntar[1] = objectePerDesjuntar;
            desjuntar[0].GetComponent<LlibreriaObjectes>().DesajuntarSeguentObjecte(desjuntar[1]);
            desjuntar = new GameObject[2];
            SeleccionarOpcio(btnSeleccionat.DESJUNTAR);
        }
    }

    public int ObteEstatSimulador(){
        return (int)estat;
    }

    public int ObteBotoSeleccionat(){
        return (int)seleccionat;
    }

    public void UltimEsdeveniment(Esdeveniment e){
        if (prefabLogs != null){
            var nouText = Instantiate(prefabLogs);
            string esdev = "";
            if (e.tipusEsdeveniment == Esdeveniment.Tipus.PROCESSOS) esdev = "eProces";
            else if (e.tipusEsdeveniment == Esdeveniment.Tipus.ARRIBADES) esdev = "eArribada";
            else if (e.tipusEsdeveniment == Esdeveniment.Tipus.eNotificacioDisponible) esdev = "eNotificacioDisponible";
            else if (e.tipusEsdeveniment == Esdeveniment.Tipus.eRepEntitat) esdev = "eRepEntitat";

            TMP_Text text1 = nouText.transform.GetChild(0).GetComponent<TMP_Text>();
            TMP_Text text2 = nouText.transform.GetChild(1).GetComponent<TMP_Text>();
            TMP_Text text3 = nouText.transform.GetChild(2).GetComponent<TMP_Text>();
            text1.text = e.obteConsumidor().transform.name;
            text1.color = Color.green;
            text2.text = e.temps.ToString();
            text2.color = Color.green;
            text3.text = esdev;
            text3.color = Color.green;
            nouText.transform.SetParent(contentView.transform);
            nouText.transform.localScale = new Vector3(1,1,1);
            if (contentView.transform.childCount > 1){
                contentView.transform.GetChild(contentView.transform.childCount-2).transform.GetChild(0).GetComponent<TMP_Text>().color = Color.black;
                contentView.transform.GetChild(contentView.transform.childCount-2).transform.GetChild(1).GetComponent<TMP_Text>().color = Color.black;
                contentView.transform.GetChild(contentView.transform.childCount-2).transform.GetChild(2).GetComponent<TMP_Text>().color = Color.black;
            }
            ActualitzaBarra();
        }
    }

    public void ActualitzaBarra(){
        Vector2 mida = contentView.GetComponent<RectTransform>().sizeDelta;
        mida.y += 50;
        contentView.GetComponent<RectTransform>().sizeDelta = mida;
        if (contentView.transform.childCount > 4){
            Vector3 pos = contentView.GetComponent<RectTransform>().anchoredPosition;
            pos.y = mida.y-200;
            contentView.GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }

    public void ReiniciarBarra(){
        Vector2 mida = contentView.GetComponent<RectTransform>().sizeDelta;
        mida.y = 0;
        contentView.GetComponent<RectTransform>().sizeDelta = mida;
    
        Vector3 pos = contentView.GetComponent<RectTransform>().anchoredPosition;
        pos.y = 0;
        contentView.GetComponent<RectTransform>().anchoredPosition = pos;
    }

    public void ActualitzarTempsActual(float tActual){
        if(tempsActual != null) tempsActual.text = "Temps actual: " + tActual;
    }
}
