using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentInstance : MonoBehaviour
{
    public float tiempoInicial;
    public float duraci�n;
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
        duraci�n = tDuracion;
    }

    public void SetActivationTimerForComment()
    {
        StartCoroutine("StartTimer");
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(tiempoInicial);
        this.gameObject.SetActive(true);
        yield return new WaitForSeconds(duraci�n);
        this.gameObject.SetActive(false);
    }
}