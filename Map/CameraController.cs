using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float offsetX;
    public float offsetY;
    GameObject player;
    void Start()
    {
        player = GameObject.Find("Player");
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
    }

    void LateUpdate()
    {
        var diffX = transform.position.x - player.transform.position.x;
        var diffY = transform.position.y - player.transform.position.y;

        if (diffX >= offsetX)
            transform.position = new Vector3(player.transform.position.x + offsetX, transform.position.y, -10);
        else if (diffX <= -offsetX)
            transform.position = new Vector3(player.transform.position.x - offsetX, transform.position.y, -10);

        if (diffY >= offsetY)
            transform.position = new Vector3(transform.position.x, player.transform.position.y + offsetY, -10);
        else if (diffY <= -offsetY)
            transform.position = new Vector3(transform.position.x, player.transform.position.y - offsetY, -10);

        // var camPosDiff = transform.position - player.transform.position;
        // Debug.Log("Diff" + camPosDiff);
        // if (camPosDiff.x > 2)
        //     transform.position = player.transform.position + offset + new Vector3(2, camPosDiff.y, 0);

        // else if (camPosDiff.x < -2)
        //     transform.position = player.transform.position + offset + new Vector3(-2, camPosDiff.y, 0);

        // else if (camPosDiff.y > 2)
        //     transform.position = player.transform.position + offset + new Vector3(camPosDiff.x, 2, 0);

        // else if (camPosDiff.y < -2)
        //     transform.position = player.transform.position + offset + new Vector3(camPosDiff.x, -2, 0);
    }
}
