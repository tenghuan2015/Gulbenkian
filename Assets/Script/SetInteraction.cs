using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetInteraction : MonoBehaviour
{
    [SerializeField]RenderTexture rt;
    [SerializeField]Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Shader.SetGlobalTexture("_RenderTexture", rt);
        Shader.SetGlobalFloat("_OrthographicSize",GetComponent<Camera>().orthographicSize);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.position.x,transform.position.y,target.position.z);
        Shader.SetGlobalVector("_Position",transform.position);
    }
}
