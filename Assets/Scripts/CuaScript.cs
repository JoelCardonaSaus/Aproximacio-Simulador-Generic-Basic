using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuaScript : MonoBehaviour, IRebreObjecte
{
    public int capacitatMaxima = -1; // -1 == No hi ha capacitat màxima, >0 capacitat màxima de la cua
    public List<GameObject> SeguentsObjectes;

    public enum politiquesEnrutament { PRIMERDISPONIBLE, RANDOM };
    public politiquesEnrutament enrutament;
    private Queue<GameObject> cuaObjecte = new Queue<GameObject>();
    private Dictionary<GameObject, double> tempsObjecteCua = new Dictionary<GameObject, double>();

    private enum states { IDLE, BUSY };
    private float timeIdle = 0;
    private float timeBusy = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cuaObjecte.Count > 0){
            IEnumerator<GameObject> enumerator = cuaObjecte.GetEnumerator();
            while (enumerator.MoveNext()) {
                tempsObjecteCua[enumerator.Current] += Time.deltaTime;
            }
            sendObject();
            timeBusy += Time.deltaTime;
        }
        else {
            timeIdle += Time.deltaTime;
        }
        
    }

    public bool isAvailable()
    {
        Debug.Log("Pregutnen si estic disponible");
        if (capacitatMaxima == -1 || cuaObjecte.Count < capacitatMaxima) return true;
        else return false;
    }

    public void recieveObject(GameObject entity)
    {
        cuaObjecte.Enqueue(entity);
        tempsObjecteCua.Add(entity, 0);
        Debug.Log(cuaObjecte.Count);

    }

    public bool sendObject(){
        IRebreObjecte NextObjecte;

        // Comprovem que almenys hi ha un objecte disponible
        if (nDisponibles() >= 1){
            if (enrutament == politiquesEnrutament.PRIMERDISPONIBLE){
                foreach (GameObject objecte in SeguentsObjectes)
                {
                    NextObjecte = objecte.GetComponent<IRebreObjecte>();
                    if (NextObjecte.isAvailable()) {
                        NextObjecte.recieveObject(cuaObjecte.Dequeue());
                        return true;
                    }
                }
                return false;
            }

            else if (enrutament == politiquesEnrutament.RANDOM){
                bool[] attemts = new bool[SeguentsObjectes.Count]; // Teoricament, al crearse el valor per defecte es false
                int numIntents = 0;
                while (numIntents < attemts.Length){
                    int intent = Random.Range(0, attemts.Length - 1);
                    if (!attemts[intent]) {
                        ++numIntents;
                        attemts[intent] = true;
                        NextObjecte = SeguentsObjectes[intent].GetComponent<IRebreObjecte>();
                        if (NextObjecte.isAvailable()) {
                            NextObjecte.recieveObject(cuaObjecte.Dequeue());
                            return true;
                        }
                    }
                }
                
            }
        }
        return false;
    }

    private int nDisponibles(){
        int n = 0;
        foreach (GameObject seguent in SeguentsObjectes){
            if (seguent.GetComponent<IRebreObjecte>().isAvailable()) ++n;
        }
        return n;
    }
}