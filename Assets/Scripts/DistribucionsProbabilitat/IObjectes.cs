using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IObjectes
{
    void IniciaSimulacio();
    void repEntitat(GameObject entitat, GameObject objecteLlibreria); 
    void intentaEliminarObjecteSeguents(GameObject objecte);
    void afegeixSeguentObjecte(GameObject objecte);
    void desajuntarSeguentObjecte(GameObject objecte);
    bool notificacioDisponible(GameObject objecteLlibreria);
    bool estaDisponible(GameObject objecteLlibreria);
    // Retorna la posició dels seguents objectes. -1 si no hi ha cap, [0..n-1] si la llista de seguents objectes no es buida.
    int cercaDisponible();
    void GenerarPlots();
    // Generador == 0; Cua == 1; Processador == 2; Sortida == 3; FakeObject == -1(per fer testing)
    int ObteTipusObjecte();
    bool RatoliSobreDetalls();
    void ObreDetalls();
    void TancaDetalls();

}
