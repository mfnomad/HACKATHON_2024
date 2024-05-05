using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivateImages : MonoBehaviour
{
    public RawImage i1;
    public RawImage i2;
    public RawImage i3;

    // Start is called before the first frame update
    void Start()
    {
        // Inicialmente, asegurarse de que las imágenes estén desactivadas
        i1.enabled = false;
        i2.enabled = false;
        i3.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Verificar si la tecla 'R' está siendo presionada
        if (Input.GetKey(KeyCode.R))
        {
            // Activar las imágenes
            i1.enabled = true;
            i2.enabled = true;
            i3.enabled = true;
        }
        else
        {
            // Desactivar las imágenes cuando la tecla 'R' no está presionada
            i1.enabled = false;
            i2.enabled = false;
            i3.enabled = false;
        }
    }
}
