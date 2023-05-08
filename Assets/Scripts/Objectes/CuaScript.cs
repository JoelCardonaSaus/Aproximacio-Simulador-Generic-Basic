using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CuaScript : MonoBehaviour, IObjectes
{
    
    public int capacitatMaxima = -1; // -1 == No hi ha capacitat màxima, >0 capacitat màxima de la cua
    public List<GameObject> SeguentsObjectes;
    public enum politiquesEnrutament { PRIMERDISPONIBLE, RANDOM };
    public politiquesEnrutament enrutament;
    private Queue<GameObject> cuaObjecte;
    private Dictionary<GameObject, double> tempsObjecteCua;
    private Queue<GameObject> objectesRebutjats;
    private enum states { BUIT, NOBUIT };
    private states estat;


    void Start()
    {
    }

    void Update()
    {
    }

    public void IniciaSimulacio(){
        estat = states.BUIT;
        objectesRebutjats = new Queue<GameObject>();
        tempsObjecteCua = new Dictionary<GameObject, double>();
        cuaObjecte = new Queue<GameObject>();
    }

    public void intentaEliminarObjecteSeguents(GameObject objecte){
        if (SeguentsObjectes.Contains(objecte)) SeguentsObjectes.Remove(objecte);
    }

    public bool estaDisponible(GameObject objecteLlibreria)
    {
        if (capacitatMaxima == -1 || cuaObjecte.Count < capacitatMaxima){
            return true;
        } 
        else{
            if (!objectesRebutjats.Contains(objecteLlibreria))  objectesRebutjats.Enqueue(objecteLlibreria);
            return false;
        }
    }

    public void repEntitat(GameObject entitat, GameObject objecteLlibreria)
    {
        entitat.transform.position = transform.position + new Vector3(0,+1,0);
        if (estat == states.NOBUIT){
            cuaObjecte.Enqueue(entitat);
            float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
            tempsObjecteCua.Add(entitat, tActual);
        }
        else if (estat == states.BUIT){
            int nDisponible = cercaDisponible();
            if (nDisponible != -1){
                SeguentsObjectes[nDisponible].GetComponent<IObjectes>().repEntitat(entitat, this.gameObject);
                tempsObjecteCua.Add(entitat, 0);
            } else {
                cuaObjecte.Enqueue(entitat);
                float tActual = transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual();
                tempsObjecteCua.Add(entitat, tActual);
                estat = states.NOBUIT;
            }
        }
    }

    public int cercaDisponible(){   
        IObjectes SeguentObj;

        // Comprovem que almenys hi ha un objecte disponible
        if (enrutament == politiquesEnrutament.PRIMERDISPONIBLE){
            for (int i = 0; i < SeguentsObjectes.Count; i++)//GameObject objecte in SeguentsObjectes)
            {
                SeguentObj = SeguentsObjectes[i].GetComponent<IObjectes>();
                if (SeguentObj.estaDisponible(this.gameObject)) {
                    return i;
                }
            }
        }

        else if (enrutament == politiquesEnrutament.RANDOM){
            for (int i = 0; i < SeguentsObjectes.Count; i++){
                int obj = Random.Range(0, SeguentsObjectes.Count);
                SeguentObj = SeguentsObjectes[obj].GetComponent<IObjectes>();
                if (SeguentObj.estaDisponible(this.gameObject)) {
                    return obj;
                }
            }
        }
        return -1;
    }

    // Retorna fals si no pot enviar cap entitat al que ha avisat que esta disponible
    public bool notificacioDisponible(GameObject objecteLlibreria)
    {
        if (estat == states.BUIT) return false;
        else if (estat == states.NOBUIT){
            if (objecteLlibreria.GetComponent<IObjectes>().estaDisponible(this.gameObject)){
                GameObject entitatEnviar = cuaObjecte.Dequeue();
                float tempsCua = (float)transform.parent.GetComponent<MotorSimuladorScript>().ObteTempsActual() - (float)tempsObjecteCua[entitatEnviar];
                tempsObjecteCua.Add(entitatEnviar, tempsCua);
                objecteLlibreria.GetComponent<IObjectes>().repEntitat(entitatEnviar, this.gameObject);

                // Avisem als objectes rebutjats que hi ha un lloc nou disponible a la cua, quan es trova un es para de avisar.
                while (objectesRebutjats.Count != 0 && !AvisaDisponibilitat());
                if (cuaObjecte.Count == 0) estat = states.BUIT;
                else estat = states.NOBUIT;

                return true;
            }
        }
        return false;
    }

    // Retorna cert si l'objecte a qui s'avisa pot enviar-li una nova entitat
    private bool AvisaDisponibilitat(){
        GameObject objecteNou = objectesRebutjats.Dequeue();
        return objecteNou.GetComponent<IObjectes>().notificacioDisponible(this.gameObject);

    }


    public int getState(){
        return (int)estat;
    }

    public int ObteTipusObjecte()
    {
        return 1;
    }

    //////////////////////////////////////////////////////////////////////
    //                                                                  //
    //                                                                  //
    //                           FUNCIONS UI                            //
    //                                                                  //
    //                                                                  //
    //////////////////////////////////////////////////////////////////////
    public void OnMouseDown()
    {
        MotorSimuladorScript motorScript = gameObject.transform.parent.GetComponent<MotorSimuladorScript>();
        if (motorScript.AlgunDetallsObert())
        {
            motorScript.TancaDetallsObert();
        }
        if (UIScript.Instancia.obteBotoSeleccionat() == 6) motorScript.eliminarObjecteLlista(this.gameObject);
        else if (UIScript.Instancia.obteBotoSeleccionat() == 7)motorScript.ObreDetallsFill(transform.GetSiblingIndex());
    }

    public void ObreDetalls(){
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }   

    public void TancaDetalls(){
        gameObject.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<UICuaScript>().CancelaCanvis();
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public bool RatoliSobreDetalls(){
        var image = transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
        if (RectTransformUtility.RectangleContainsScreenPoint(image.rectTransform, Input.mousePosition))
        {
            return true;
        }
        return false;
    }

    public void ActualitzaPropietats(politiquesEnrutament nouEnrutament, int capacitatMax){
        enrutament = nouEnrutament;
        capacitatMaxima = capacitatMax;
    }
}
