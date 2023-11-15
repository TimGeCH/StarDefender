using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScorall : MonoBehaviour
{
    [SerializeField] Vector2 scrollVelocity;
    Material material;
    
    void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    IEnumerator  Start()
    {
        while (GameManager.GameState != GameState.GameOver)
        {
            material.mainTextureOffset += scrollVelocity * Time.deltaTime;
            yield return null;
        }
        
    }
}
