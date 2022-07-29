using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVFloor : MonoBehaviour
{
    public Material myMat;

    public float scrollSpeed;

    private float offset;

    private Vector2 offsetVector;

    void Start()
    {
        
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h > 0)
        {
            offset += (float)(scrollSpeed * Time.deltaTime * 0.2f);

            offsetVector = new Vector2(offset, 0);

            myMat.SetTextureOffset("_MainTex", offsetVector);
        }
        else if (h < 0)
        {
            offset += (float)(scrollSpeed * Time.deltaTime * 0.2f);

            offsetVector = new Vector2(-offset, 0);

            myMat.SetTextureOffset("_MainTex", offsetVector);
        }

        if (v > 0)
        {
            offset += (float)(scrollSpeed * Time.deltaTime * 0.2f);

            offsetVector = new Vector2(0, offset);

            myMat.SetTextureOffset("_MainTex", offsetVector);
        }
        else if (v < 0)
        {
            offset += (float)(scrollSpeed * Time.deltaTime * 0.2f);

            offsetVector = new Vector2(0, -offset);

            myMat.SetTextureOffset("_MainTex", offsetVector);
        }

        if (v > 0 && h > 0)
        {
            offset += (float)(scrollSpeed * Time.deltaTime * 0.05f);

            offsetVector = new Vector2(offset, offset);

            myMat.SetTextureOffset("_MainTex", offsetVector);
        }
        else if (v < 0 && h < 0)
        {
            offset += (float)(scrollSpeed * Time.deltaTime * 0.05f);

            offsetVector = new Vector2(-offset, -offset);

            myMat.SetTextureOffset("_MainTex", offsetVector);
        }
        else if (v > 0 && h < 0)
        {
            offset += (float)(scrollSpeed * Time.deltaTime * 0.05f);

            offsetVector = new Vector2(-offset, offset);

            myMat.SetTextureOffset("_MainTex", offsetVector);
        }
        else if (v < 0 && h > 0)
        {
            offset += (float)(scrollSpeed * Time.deltaTime * 0.05f);

            offsetVector = new Vector2(offset, -offset);

            myMat.SetTextureOffset("_MainTex", offsetVector);
        }
    }
}
