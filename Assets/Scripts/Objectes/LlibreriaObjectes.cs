using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LlibreriaObjectes : MonoBehaviour
{
    public List<GameObject> SeguentsObjectes = new List<GameObject>(); 
    public  enum politiquesEnrutament { PRIMERDISPONIBLE, RANDOM };
    public politiquesEnrutament enrutament;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public abstract void IniciaSimulacio();
    public abstract void RepEntitat(GameObject entitat, GameObject objecteLlibreria);
    public abstract bool NotificacioDisponible(GameObject objecteLlibreria);
    public abstract bool EstaDisponible(GameObject objecteLlibreria);
    public abstract int ObteTipusObjecte();
    public abstract void GenerarPlots();
    public abstract void ReiniciaSimulador();
    
    // Funcions per la UI
    public abstract bool RatoliSobreDetalls();
    public abstract void ObreDetalls();
    public abstract void TancaDetalls();
   

    public int CercaDisponible(){   
        LlibreriaObjectes SeguentObj;

        // Comprovem que almenys hi ha un objecte disponible
        if (enrutament == politiquesEnrutament.PRIMERDISPONIBLE){
            for (int i = 0; i < SeguentsObjectes.Count; i++)//GameObject objecte in SeguentsObjectes)
            {
                SeguentObj = SeguentsObjectes[i].GetComponent<LlibreriaObjectes>();
                if (SeguentObj.EstaDisponible(this.gameObject)) {
                    return i;
                }
            }
        }

        else if (enrutament == politiquesEnrutament.RANDOM){
            for (int i = 0; i < SeguentsObjectes.Count; i++){
                int obj = Random.Range(0, SeguentsObjectes.Count);
                SeguentObj = SeguentsObjectes[obj].GetComponent<LlibreriaObjectes>();
                if (SeguentObj.EstaDisponible(this.gameObject)) {
                    return obj;
                }
            }
        }
        return -1;
    }

    public void AfegeixSeguentObjecte(GameObject objecte){
        if (!SeguentsObjectes.Contains(objecte)){
            DibuixaLinia(objecte);
        }
    }

    public void DibuixaLinia(GameObject objecte){
        GameObject objecteAmbLinia = new GameObject("L"+SeguentsObjectes.Count.ToString());
        objecteAmbLinia.transform.parent = transform;
        SeguentsObjectes.Add(objecte);
        LineRenderer lr = objecteAmbLinia.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, objecte.transform.position);
        lr.startColor = Color.white;
        lr.endColor = Color.green;
        lr.material = Resources.Load<Material>("Materials/LineRendererMaterial") as Material;
    }

    public void DesajuntarSeguentObjecte(GameObject desjuntar){
        IntentaEliminarObjecteSeguents(desjuntar);
    }

    public void IntentaEliminarObjecteSeguents(GameObject objecte){
        if (SeguentsObjectes.Contains(objecte)) {
            Destroy(transform.GetChild(SeguentsObjectes.IndexOf(objecte)+2).gameObject);
            SeguentsObjectes.Remove(objecte);  
        }
    }

}
