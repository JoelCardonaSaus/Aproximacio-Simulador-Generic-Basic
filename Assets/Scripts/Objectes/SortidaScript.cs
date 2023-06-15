using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SortidaScript : LlibreriaObjectes
{
    private int nEntitatsDestruides;
    private List<double> tempsEntreEntitats = new List<double>();
    public TMP_Text etiqueta;

    // Variable per poder moure els objectes
    Vector3 posicioRatoliOffset;

    void Start()
    {
        nEntitatsDestruides = 0;
        transform.name = transform.name.Replace("Clone", MotorSimuladorScript.Instancia.ObteIdSeguentObjecte().ToString());
        etiqueta.text = transform.name;
    }

    void Update()
    {
    }

    public override void IniciaSimulacio(){
        nEntitatsDestruides = 0;
        tempsEntreEntitats = new List<double>();
        etiqueta.text = transform.name + "\n";
    }

    public override void RepEntitat(GameObject entitat, GameObject objecteLlibreria)
    {
        entitat.transform.position = transform.position + new Vector3(0,+1,0);
        ++nEntitatsDestruides;
        float tActual = MotorSimuladorScript.Instancia.ObteTempsActual();
        if (tempsEntreEntitats.Count != 0) {
            tempsEntreEntitats.Add(tActual-tempsEntreEntitats[tempsEntreEntitats.Count-1]);
        } else {
            tempsEntreEntitats.Add(tActual);
        }
        Destroy(entitat);        
    }

    // Retorna fals si no pot enviar cap entitat al que ha avisat que esta disponible
    public override bool NotificacioDisponible(GameObject objecteLlibreria)
    {
        return false;
    }

    public override bool EstaDisponible(GameObject objecteLlibreria)
    {
        return true;
    }

    public override void ReiniciaSimulador()
    {
        nEntitatsDestruides = 0;
        tempsEntreEntitats = new List<double>();
        etiqueta.text = transform.name + "\n";

    }

    new public int CercaDisponible(){   
        return -1;
    }

    new public void AfegeixSeguentObjecte(GameObject objecte){
        
    }

    new public void IntentaEliminarObjecteSeguents(GameObject objecte){    }

    new public void DesajuntarSeguentObjecte(GameObject desjuntar){
        
    }

    public override int ObteTipusObjecte()
    {
        return 3;
    }
    

    public int getNEntitatsDestruides(){
        return nEntitatsDestruides;
    }


    public void InicialitzaPerFerTests(){
        tempsEntreEntitats.Add(0); // Creem el temps d'espera per la primera entitat
        nEntitatsDestruides = 0;
    }

    public override void ActualizaEstadistics(){
        string estadistics = "Output: " + nEntitatsDestruides +"\n";
        float tempsActual = (MotorSimuladorScript.Instancia.ObteTempsActual());
        
        etiqueta.text = transform.name + "\n";
        if (nEntitatsDestruides != 0) estadistics += "Temps mig entitats destruides: " + (float)Math.Round((tempsActual/(nEntitatsDestruides)),2) +"\n";

        etiqueta.text += estadistics; 
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
        posicioRatoliOffset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (MotorSimuladorScript.Instancia.AlgunDetallsObert())
        {
            MotorSimuladorScript.Instancia.TancaDetallsObert();
        }
        if (UIScript.Instancia.ObteBotoSeleccionat() == 6) MotorSimuladorScript.Instancia.EliminarObjecte(this.gameObject);
        else if (UIScript.Instancia.ObteBotoSeleccionat() == 7){
            MotorSimuladorScript.Instancia.ObreDetallsFill(transform.GetSiblingIndex());
        }
        else if (UIScript.Instancia.ObteBotoSeleccionat() == 4) UIScript.Instancia.AjuntarObjectes(this.gameObject);
        else if (UIScript.Instancia.ObteBotoSeleccionat() == 5) UIScript.Instancia.DesjuntarObjectes(this.gameObject);
    }

    private void OnMouseDrag(){
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + posicioRatoliOffset;
        for (int i = 0; i < SeguentsObjectes.Count; i++){
            LineRenderer lr = gameObject.transform.GetChild(2+i).GetComponent<LineRenderer>();
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, SeguentsObjectes[i].transform.position);
        }
        for (int i = 0; i < ObjectesPredecessors.Count; i++){
            ObjectesPredecessors[i].GetComponent<LlibreriaObjectes>().CanviaPosicioPredecessor(this.gameObject);
        }
    }
    

    public override void ObreDetalls(){
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }   

    public override void TancaDetalls(){
        gameObject.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<UISortidaScript>().CancelaCanvis();
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public override bool RatoliSobreDetalls(){
        var image = transform.GetChild(0).transform.GetChild(0).GetComponent<Image>();
        if (RectTransformUtility.RectangleContainsScreenPoint(image.rectTransform, Input.mousePosition))
        {
            return true;
        }
        return false;
    }

    public void ActualitzaPropietats(string nom){
        transform.name = nom;
        etiqueta.text = nom;
    }
}
