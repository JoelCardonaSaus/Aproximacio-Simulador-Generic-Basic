using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIScript : MonoBehaviour
{
    private enum btnSeleccionat { GENERADOR, CUA, PROCESSADOR, SORTIDA, JUNTAR, DESJUNTAR, CAP };
    private btnSeleccionat seleccionat;
    public Button generadorButton;
    public Button cuaButton;
    public Button processadorButton;
    public Button sortidaButton;
    public Button juntarButton;
    public Button desjuntarButton;

    public GameObject motorSimulador;
    private float botoClikatCooldown = 0f;
    private GameObject generadorPrefab;
    private GameObject cuaPrefab;

    private GameObject processadorPrefab;
    private GameObject sortidaPrefab;



    [SerializeField] public Texture2D[] imatgesCursor = new Texture2D[6];

    // Start is called before the first frame update
    void Start()
    {
        seleccionat = btnSeleccionat.CAP;
        generadorPrefab = Resources.Load("LlibreriaObjectes/Generador/Generador") as GameObject;
        cuaPrefab = Resources.Load("LlibreriaObjectes/Cua/Cua") as GameObject;
        processadorPrefab = Resources.Load("LlibreriaObjectes/Processador/Processador") as GameObject;
        sortidaPrefab = Resources.Load("LlibreriaObjectes/Sortida/Sortida") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && botoClikatCooldown<=0)
        {
            intentaInstanciarObjecte();
        }
        if (botoClikatCooldown > 0) botoClikatCooldown -= Time.deltaTime;
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

    private void seleccionarOpcio(btnSeleccionat seleccionatNou){
        botoClikatCooldown = 0.25f;
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
            case btnSeleccionat.CAP:
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
            case btnSeleccionat.CAP:
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                break;
        }
    }

    private void intentaInstanciarObjecte(){
        Vector3 mousePosition = Input.mousePosition;
        // Convert the mouse position to a world position relative to the camera
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        switch (seleccionat){
            case btnSeleccionat.GENERADOR:
                // Paasar-li l'objecte creat al motor
                GameObject generadorNou = Instantiate(generadorPrefab, worldPosition , Quaternion.identity);
                motorSimulador.GetComponent<MotorSimuladorScript>().afegirObjecteLlista(generadorNou);
                deseleccionarBackground(seleccionat);
                seleccionat = btnSeleccionat.CAP;
                seleccionarBackground(seleccionat);
                break;
            case btnSeleccionat.CUA:
                motorSimulador.GetComponent<MotorSimuladorScript>().afegirObjecteLlista(Instantiate(cuaPrefab, worldPosition, Quaternion.identity));
                deseleccionarBackground(seleccionat);
                seleccionat = btnSeleccionat.CAP;
                seleccionarBackground(seleccionat);
                break;
            case btnSeleccionat.PROCESSADOR:
                motorSimulador.GetComponent<MotorSimuladorScript>().afegirObjecteLlista(Instantiate(processadorPrefab, worldPosition, Quaternion.identity));
                deseleccionarBackground(seleccionat);
                seleccionat = btnSeleccionat.CAP;
                seleccionarBackground(seleccionat);
                break;
            case btnSeleccionat.SORTIDA:
                motorSimulador.GetComponent<MotorSimuladorScript>().afegirObjecteLlista(Instantiate(sortidaPrefab, worldPosition, Quaternion.identity));
                deseleccionarBackground(seleccionat);
                seleccionat = btnSeleccionat.CAP;
                seleccionarBackground(seleccionat);
                break;
        }
    }

}
