using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
   public static DontDestroy instance = null;
   
    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null){
            instance = this;
        } else {
            Destroy(transform.gameObject);
        }
        DontDestroyOnLoad(transform.gameObject);

        
    }
}
