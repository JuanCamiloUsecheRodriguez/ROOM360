using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentInstance : MonoBehaviour
{
    public float tiempoInicial;
    public float duración;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AssignActivationTimerForComment(float tInicial, float tDuracion)
    {
        tiempoInicial = tInicial;
        duración = tDuracion;
    }

    public void SetActivationTimerForComment()
    {
        StartCoroutine("StartTimer");
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(tiempoInicial);
        this.gameObject.SetActive(true);
        yield return new WaitForSeconds(duración);
        this.gameObject.SetActive(false);
    }
}