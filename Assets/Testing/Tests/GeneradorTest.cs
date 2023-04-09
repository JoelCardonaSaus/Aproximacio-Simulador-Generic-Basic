using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class GeneradorTest
{
    
    [Test]
    public void GeneradorTestSimplePasses()
    {
        GeneradorScript gs = new GeneradorScript();
    }
}

class fakeGeneradorScript : IRebreObjecte
{
        public void setTimeScale(float timeScale){
            
        }

        public bool sendObject(){
            return true;
        }

        public bool isAvailable(){
            return true;
        }
        
        public void recieveObject(GameObject entity){

        }
}
