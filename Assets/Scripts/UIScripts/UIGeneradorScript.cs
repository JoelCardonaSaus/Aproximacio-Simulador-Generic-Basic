using UnityEngine;
using UnityEngine.UI;

public class UIGeneradorScript : MonoBehaviour
{
    public Dropdown enrutament; 
    public GeneradorScript generadorScript;
    public void CanviEnrutamentSeleccionat()
    {
        if (enrutament.value == 0) generadorScript.CanviaEnrutament(GeneradorScript.politiquesEnrutament.PRIMERDISPONIBLE);
        else if (enrutament.value == 1) generadorScript.CanviaEnrutament(GeneradorScript.politiquesEnrutament.RANDOM);
    }

    void Start()
    {
        generadorScript = gameObject.transform.parent.GetComponentInParent<GeneradorScript>();
    }

    void Update()
    {

    }
}
