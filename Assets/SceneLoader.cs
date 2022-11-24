using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneLoader : MonoBehaviour
{   
    public AudioSource select;

    public void load_scene(string name){
        SceneManager.LoadScene(name);
        select.Play();
    }

}
