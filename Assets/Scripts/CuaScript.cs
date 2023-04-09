using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CuaScript : MonoBehaviour, IRebreObjecte
{
    [SerializeField]
    public int capacitatMaxima = -1; // -1 == No hi ha capacitat màxima, >0 capacitat màxima de la cua
    [SerializeField]
    public List<GameObject> SeguentsObjectes;

    public enum politiquesEnrutament { PRIMERDISPONIBLE, RANDOM };
    [SerializeField]
    public politiquesEnrutament enrutament;
    private Queue<GameObject> cuaObjecte = new Queue<GameObject>();
    private Dictionary<GameObject, double> tempsObjecteCua = new Dictionary<GameObject, double>();

    private enum states { EMPTY, NOEMPTY };

    private states state = states.EMPTY;
    private float timeEmpty = 0;
    private float timeNoEmpty = 0;
    private float timeScale = 1;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cuaObjecte.Count > 0){
            IEnumerator<GameObject> enumerator = cuaObjecte.GetEnumerator();
            while (enumerator.MoveNext()) {
                tempsObjecteCua[enumerator.Current] += (Time.deltaTime * timeScale);
            }
            sendObject();
            timeNoEmpty += (Time.deltaTime * timeScale);
        }
        else {
            timeEmpty += (Time.deltaTime * timeScale);
        }
        
    }

    public void setTimeScale(float timeScale){
        this.timeScale = timeScale;
    }

    public bool isAvailable()
    {
        if (capacitatMaxima == -1 || cuaObjecte.Count < capacitatMaxima) return true;
        else return false;
    }

    public void recieveObject(GameObject entity)
    {
        Debug.Log("La CUA rep un objecte!");
        entity.transform.position = transform.position + new Vector3(0,+1,0);
        if (state == states.EMPTY) state = states.NOEMPTY;
        cuaObjecte.Enqueue(entity);
        tempsObjecteCua.Add(entity, 0);

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
                        if (cuaObjecte.Count == 0) state = states.EMPTY;
                        return true;
                    }
                }
                return false;
            }
            else if (enrutament == politiquesEnrutament.RANDOM){
                for (int i = 0; i < SeguentsObjectes.Count; i++){
                    int obj = Random.Range(0, SeguentsObjectes.Count);
                    NextObjecte = SeguentsObjectes[obj].GetComponent<IRebreObjecte>();
                    if (NextObjecte.isAvailable()) {
                        NextObjecte.recieveObject(cuaObjecte.Dequeue());
                        if (cuaObjecte.Count == 0) state = states.EMPTY;
                        return true;
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
