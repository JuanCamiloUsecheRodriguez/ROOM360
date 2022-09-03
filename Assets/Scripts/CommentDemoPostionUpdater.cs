using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommentDemoPostionUpdater : MonoBehaviour
{
    public Transform player;
    public Text timerText;
    void Awake()
    {
       
    }
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player);
    }

    public void UpdatePosition(Vector3 newPosition)
    {
   
        this.transform.position = new Vector3(newPosition.x, player.position.y, newPosition.z);
    }

    public void UpdateTimerText(string pText)
    {
        timerText.text = pText;
    }
}
