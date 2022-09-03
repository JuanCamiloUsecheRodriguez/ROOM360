using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;


public class DomePointerEvents : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public TouchScreenKeyboard overlayKeyboard;

    public static string newCommentinputText = "";
    public GameObject DemoComment;
    public Text newCommentTextToShown;
    private Vector3 newCommentManagerPosition;
    public GameObject temporalMarker;


    public LineRenderer lineRenderer;
    public CommentManagerObject commentManagerObject;
    public VideoManager videoManager = null;

    public float capturedTime = 0f;
    public float tiempoInicialComment = 0f;
    public Text capturedTimeText;

    //TESTING
    private float testNum = 0f;
    public Text testText;

    private bool pointerDown = false;

    public GameObject commentInstancePrefab;
    public bool NewInputFromThisClass = true;
    private List<GameObject> commentManagerList;

    public InputManager inputManager;

    public string path;

    private void Awake()
    {
        string pdp = Application.persistentDataPath;
        bool pathFound = false;
        int count = 0;
        while (!pathFound)
        {
            path = pdp + "/DB" + count + ".txt";
            if (!File.Exists(path))
            {
                pathFound = true;
            }
            else
            {
                count++;
            }
        }

        Debug.Log("PATH:" + path);
        commentManagerList = new List<GameObject>();
        testText.text = "CommentMode";
        CreateDemoComments();
    }

    public void Update()
    {
        CheckKeyboardStatusAndRetriveInput();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (inputManager.currentGameState == InputManager.GameState.CommentMode)
        { 
            if (pointerDown == false)
            {
                pointerDown = true;
                tiempoInicialComment = (float)videoManager.videoPlayer.time;
                TakeTimeAndMarkPosition(eventData);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (inputManager.currentGameState == InputManager.GameState.CommentMode)
        {
            if (pointerDown == true)
            {
                pointerDown = false;
                StopTimerandOpenKeyboard(true);
            }
        }
    }

    public void TakeTimeAndMarkPosition(PointerEventData eventData)
    {
        capturedTimeText.gameObject.SetActive(true);
        StartCoroutine("CaptureTime");

        newCommentManagerPosition = eventData.pointerCurrentRaycast.worldPosition;

        temporalMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        temporalMarker.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        temporalMarker.transform.position = eventData.pointerCurrentRaycast.worldPosition;
    }

    public void StopTimerandOpenKeyboard(bool fromThisClass)
    {
        if(fromThisClass == true)
        {
            NewInputFromThisClass = true;
        }
        else
        {
            NewInputFromThisClass = false;
        }
        capturedTimeText.gameObject.SetActive(false);
        StopCoroutine("CaptureTime");
        if(capturedTime < 3f)
        {
            capturedTime = 3f;
        }
        videoManager.PauseToggle();

        if (overlayKeyboard != null)
        {

            return;
        }
        overlayKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }


    private IEnumerator CaptureTime()
    {
        float initialTime = Time.time;
        capturedTime = 0f;
        while (capturedTime < 20f)
        {
            capturedTime = Time.time - initialTime;
            yield return null;
        }
    }

    public void PrepViewMode()
    {
        testText.text = "ViewMode";
        videoManager.ResetVideo();
        foreach (GameObject commentManager in commentManagerList)
        {
            commentManager.GetComponent<CommentManagerObject>().HideAllComments();
            commentManager.GetComponent<CommentManagerObject>().ResetTimerForComments();
        }
    }

    public void PrepCommentMode()
    {
        testText.text = "CommentMode";
        videoManager.ResetVideo();
        foreach (GameObject commentManager in commentManagerList)
        {
            commentManager.GetComponent<CommentManagerObject>().HideAllComments();
        }
    }


    public void SetStartTimeCaptureTime(float pTime)
    {
        tiempoInicialComment = pTime;
    }


    public void CheckKeyboardStatusAndRetriveInput()
    {
        capturedTimeText.text = capturedTime.ToString("F0");
        if (NewInputFromThisClass == true)
        { 
            if (overlayKeyboard != null)
            {
                newCommentinputText = overlayKeyboard.text;
                newCommentTextToShown.text = newCommentinputText;
                lineRenderer.gameObject.SetActive(false);
                DemoComment.SetActive(true);
                testText.text = "";
                DemoComment.SendMessage("UpdateTimerText", capturedTime.ToString("F0"));


                if (overlayKeyboard.status == TouchScreenKeyboard.Status.Done)
                {
                    lineRenderer.gameObject.SetActive(true);
                    DemoComment.SetActive(false);
                    testText.text = "CommentMode";
                    overlayKeyboard = null;
                    Destroy(temporalMarker);
                    videoManager.PauseToggle();

                    if (newCommentinputText != "")
                    {
                        CommentManagerObject newCommentManager = Instantiate(commentManagerObject, newCommentManagerPosition, Quaternion.identity);
                        var newComment = Instantiate(commentInstancePrefab);
                        newComment.GetComponentInChildren<Text>().text = newCommentinputText;
                        newComment.GetComponent<CommentInstance>().tiempoInicial = tiempoInicialComment;
                        newComment.GetComponent<CommentInstance>().duración = capturedTime;
                        commentManagerList.Add(newCommentManager.gameObject);
                        newCommentManager.AddNewComment(newComment, true);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                CommentManagerObject newCommentManager = Instantiate(commentManagerObject, new Vector3(0, 0, 0), Quaternion.identity);            
                var newComment = Instantiate(commentInstancePrefab);
                newComment.GetComponentInChildren<Text>().text = "nuevocomment";
                newComment.GetComponent<CommentInstance>().tiempoInicial = 5f;
                newComment.GetComponent<CommentInstance>().duración = 3f;
                commentManagerList.Add(newCommentManager.gameObject);
                newCommentManager.AddNewComment(newComment, true);
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                PrepViewMode();
            }
        }
    }

    public void CreateDemoComments()
    {
        CommentManagerObject newCommentManager1 = Instantiate(commentManagerObject, new Vector3(1.979f, 1.504f, 0.03f), Quaternion.identity);
        CommentManagerObject newCommentManager2 = Instantiate(commentManagerObject, new Vector3(0.014f, 1.504f, 2.014f), Quaternion.identity);
        CommentManagerObject newCommentManager3 = Instantiate(commentManagerObject, new Vector3(-1.985f, 1.504f, 0.022f), Quaternion.identity);
        CommentManagerObject newCommentManager4 = Instantiate(commentManagerObject, new Vector3(0.07f, 1.504f, -1.983f), Quaternion.identity);
        commentManagerList.Add(newCommentManager1.gameObject);
        commentManagerList.Add(newCommentManager2.gameObject);
        commentManagerList.Add(newCommentManager3.gameObject);
        commentManagerList.Add(newCommentManager4.gameObject);


        //VIDEO1

        var newComment1 = Instantiate(commentInstancePrefab);
        newComment1.GetComponentInChildren<Text>().text = "Se observa el acceso del joven a agua potable";
        newComment1.GetComponent<CommentInstance>().tiempoInicial = 4f;
        newComment1.GetComponent<CommentInstance>().duración = 8f;
        newCommentManager3.AddNewComment(newComment1, false);

        //VIDEO 2

        var newComment2 = Instantiate(commentInstancePrefab);
        newComment2.GetComponentInChildren<Text>().text = "El desgaste en la fachada de los edificios es señal de infraestructura descuidada";
        newComment2.GetComponent<CommentInstance>().tiempoInicial = 14f;
        newComment2.GetComponent<CommentInstance>().duración = 4f;
        newCommentManager4.AddNewComment(newComment2, false);

        //VIDEO 3
        var newComment3 = Instantiate(commentInstancePrefab);
        newComment3.GetComponentInChildren<Text>().text = "La rehabilitación o la recuperación de edificios abandonados puede ayudar en la sostenibilidad del crecimiento urbano";
        newComment3.GetComponent<CommentInstance>().tiempoInicial = 18f;
        newComment3.GetComponent<CommentInstance>().duración = 12f;
        newCommentManager2.AddNewComment(newComment3, false);

        var newComment5 = Instantiate(commentInstancePrefab);
        newComment5.GetComponentInChildren<Text>().text = "Los edicios en deterioro pueden asociarse a la pobreza de la región";
        newComment5.GetComponent<CommentInstance>().tiempoInicial = 18f;
        newComment5.GetComponent<CommentInstance>().duración = 12f;
        newCommentManager3.AddNewComment(newComment5, false);

        //VIDEO4
        var newComment6 = Instantiate(commentInstancePrefab);
        newComment6.GetComponentInChildren<Text>().text = "Gente charlando alegremente con una mujer que trabaja en la policia es una muestra de no discriminación e igualdad de genero";
        newComment6.GetComponent<CommentInstance>().tiempoInicial = 31f;
        newComment6.GetComponent<CommentInstance>().duración = 10f;
        newCommentManager2.AddNewComment(newComment6, false);

        //VIDEO5
        var newComment7 = Instantiate(commentInstancePrefab);
        newComment7.GetComponentInChildren<Text>().text = "El pobre estado de la construcción es muestra de la falta de recursos para una urbanización sostenible";
        newComment7.GetComponent<CommentInstance>().tiempoInicial = 41f;
        newComment7.GetComponent<CommentInstance>().duración = 11f;
        newCommentManager2.AddNewComment(newComment7, false);

        var newComment8 = Instantiate(commentInstancePrefab);
        newComment8.GetComponentInChildren<Text>().text = "Las calles se ven desaseadas y con inperfecciones en el asfalto, muestra de una pobre infrastrictura vial";
        newComment8.GetComponent<CommentInstance>().tiempoInicial = 41f;
        newComment8.GetComponent<CommentInstance>().duración = 11f;
        newCommentManager1.AddNewComment(newComment8, false);

        //VIDEO6
        var newComment9 = Instantiate(commentInstancePrefab);
        newComment9.GetComponentInChildren<Text>().text = "La basura en el matorral es una muestra de los problemas de contaminación ambiental de la región";
        newComment9.GetComponent<CommentInstance>().tiempoInicial = 51f;
        newComment9.GetComponent<CommentInstance>().duración = 11f;
        newCommentManager2.AddNewComment(newComment9, false);

        //VIDEO7
        var newComment10 = Instantiate(commentInstancePrefab);
        newComment10.GetComponentInChildren<Text>().text = "Toda esta basura afecta los ecosistemas de la región";
        newComment10.GetComponent<CommentInstance>().tiempoInicial = 61f;
        newComment10.GetComponent<CommentInstance>().duración = 8f;
        newCommentManager1.AddNewComment(newComment10, false);

        var newComment11 = Instantiate(commentInstancePrefab);
        newComment11.GetComponentInChildren<Text>().text = "La basura que entre al rio puede afectar los ecosistemas que lo habitan";
        newComment11.GetComponent<CommentInstance>().tiempoInicial = 61f;
        newComment11.GetComponent<CommentInstance>().duración = 8f;
        newCommentManager2.AddNewComment(newComment11, false);

        //VIDEO10
        var newComment12 = Instantiate(commentInstancePrefab);
        newComment12.GetComponentInChildren<Text>().text = "Las calles sucias y los edificios de pobre construcción son muestra de problemas en construir comunidades sostenibles";
        newComment12.GetComponent<CommentInstance>().tiempoInicial = 90f;
        newComment12.GetComponent<CommentInstance>().duración = 10f;
        newCommentManager2.AddNewComment(newComment12, false);

        var newComment13 = Instantiate(commentInstancePrefab);
        newComment13.GetComponentInChildren<Text>().text = "La pobre estado de la construcción es muestra de la falta de recursos para una urbanización sostenible";
        newComment13.GetComponent<CommentInstance>().tiempoInicial = 90f;
        newComment13.GetComponent<CommentInstance>().duración = 10f;
        newCommentManager1.AddNewComment(newComment13, false);

        //VIDEO12
        var newComment14 = Instantiate(commentInstancePrefab);
        newComment14.GetComponentInChildren<Text>().text = "La basura afecta los ecosistemas de la región";
        newComment14.GetComponent<CommentInstance>().tiempoInicial = 110f;
        newComment14.GetComponent<CommentInstance>().duración = 10f;
        newCommentManager1.AddNewComment(newComment14, false);

        //VIDEO13
        var newComment15 = Instantiate(commentInstancePrefab);
        newComment15.GetComponentInChildren<Text>().text = "Los peces muertos muestran un ecosistema maltratado por la contaminación y la sequia";
        newComment15.GetComponent<CommentInstance>().tiempoInicial = 120f;
        newComment15.GetComponent<CommentInstance>().duración = 10f;
        newCommentManager1.AddNewComment(newComment15, false);

        //VIDEO14
        var newComment16 = Instantiate(commentInstancePrefab);
        newComment16.GetComponentInChildren<Text>().text = "Las calles sucias y los edificios de pobre construcción son muestra de problemas en construir comunidades sostenibles";
        newComment16.GetComponent<CommentInstance>().tiempoInicial = 129f;
        newComment16.GetComponent<CommentInstance>().duración = 10f;
        newCommentManager1.AddNewComment(newComment16, false);

        var newComment17 = Instantiate(commentInstancePrefab); 
        newComment17.GetComponentInChildren<Text>().text = "Se observa pobreza en las calles de la ciudad";
        newComment17.GetComponent<CommentInstance>().tiempoInicial = 129f;
        newComment17.GetComponent<CommentInstance>().duración = 10f;
        newCommentManager3.AddNewComment(newComment17, false);

        //VIDEO 15

        var newComment18 = Instantiate(commentInstancePrefab);
        newComment18.GetComponentInChildren<Text>().text = "La sequia presente en la región afecta los ecosistemas y a las personas que habitan la región";
        newComment18.GetComponent<CommentInstance>().tiempoInicial = 140f;
        newComment18.GetComponent<CommentInstance>().duración = 10f;
        newCommentManager3.AddNewComment(newComment18, false);

        var newComment19= Instantiate(commentInstancePrefab);
        newComment19.GetComponentInChildren<Text>().text = "La sequia de la guajira afecta los personas y a los ecosistemas que habitan la región";
        newComment19.GetComponent<CommentInstance>().tiempoInicial = 140f;
        newComment19.GetComponent<CommentInstance>().duración = 10f;
        newCommentManager2.AddNewComment(newComment19, false);

        //VIDEO 16

        var newComment20 = Instantiate(commentInstancePrefab);
        newComment20.GetComponentInChildren<Text>().text = "El hoyo en la carretera cubierto por el cono muestra el pobre estado de la infraestructura vial";
        newComment20.GetComponent<CommentInstance>().tiempoInicial = 150f;
        newComment20.GetComponent<CommentInstance>().duración = 10f;
        newCommentManager3.AddNewComment(newComment20, false);

        var newComment21 = Instantiate(commentInstancePrefab);
        newComment21.GetComponentInChildren<Text>().text = "Un rio que podria verse afectado por la contaminación ambiental en la región";
        newComment21.GetComponent<CommentInstance>().tiempoInicial = 150f;
        newComment21.GetComponent<CommentInstance>().duración = 10f;
        newCommentManager1.AddNewComment(newComment21, false);

        //VIDEO 17

        var newComment22 = Instantiate(commentInstancePrefab);
        newComment22.GetComponentInChildren<Text>().text = "La lancha no esta en uso, solo es contaminación ambiental";
        newComment22.GetComponent<CommentInstance>().tiempoInicial = 160f;
        newComment22.GetComponent<CommentInstance>().duración = 10f;
        newCommentManager1.AddNewComment(newComment22, false);

        //VIDEO 18

        var newComment23 = Instantiate(commentInstancePrefab);
        newComment23.GetComponentInChildren<Text>().text = "Un ecosistema que podria verse afectado por la contaminación en la región";
        newComment23.GetComponent<CommentInstance>().tiempoInicial = 170f;
        newComment23.GetComponent<CommentInstance>().duración = 30f;
        newCommentManager3.AddNewComment(newComment23, false);


        var newComment24 = Instantiate(commentInstancePrefab);
        newComment24.GetComponentInChildren<Text>().text = "La botella en la lancha deberia ir al reciclaje antes de que termine por contaminar";
        newComment24.GetComponent<CommentInstance>().tiempoInicial = 170f;
        newComment24.GetComponent<CommentInstance>().duración = 30f;
        newCommentManager1.AddNewComment(newComment24, false);


        var newComment25 = Instantiate(commentInstancePrefab);
        newComment25.GetComponentInChildren<Text>().text = "Los habitantes de la región son victimas tambien de los problemas de sequia";
        newComment25.GetComponent<CommentInstance>().tiempoInicial = 170f;
        newComment25.GetComponent<CommentInstance>().duración = 30f;
        newCommentManager4.AddNewComment(newComment25, false);

        //VIDEO 20
        var newComment26 = Instantiate(commentInstancePrefab);
        newComment26.GetComponentInChildren<Text>().text = "Hay señales de sequia y contaminación presentes en el ecosistema";
        newComment26.GetComponent<CommentInstance>().tiempoInicial = 200f;
        newComment26.GetComponent<CommentInstance>().duración = 12f;
        newCommentManager3.AddNewComment(newComment26, false);

        //VIDEO 21
        var newComment27 = Instantiate(commentInstancePrefab);
        newComment27.GetComponentInChildren<Text>().text = "Hay contaminación en el pasto que afecta el medio ambiente";
        newComment27.GetComponent<CommentInstance>().tiempoInicial = 212f;
        newComment27.GetComponent<CommentInstance>().duración = 8f;
        newCommentManager4.AddNewComment(newComment27, false);

        //VIDEO 22
        var newComment28 = Instantiate(commentInstancePrefab);
        newComment28.GetComponentInChildren<Text>().text = "Contaminación y sequia abundan en el ecosistema";
        newComment28.GetComponent<CommentInstance>().tiempoInicial = 220f;
        newComment28.GetComponent<CommentInstance>().duración = 17f;
        newCommentManager2.AddNewComment(newComment28, false);

        var newComment29 = Instantiate(commentInstancePrefab);
        newComment29.GetComponentInChildren<Text>().text = "La infraestructura vial consiste de vias no pavimentadas, los niños dependen de ellas para acceder a su educación";
        newComment29.GetComponent<CommentInstance>().tiempoInicial = 220f;
        newComment29.GetComponent<CommentInstance>().duración = 17f;
        newCommentManager1.AddNewComment(newComment29, false);

        var newComment30 = Instantiate(commentInstancePrefab);
        newComment30.GetComponentInChildren<Text>().text = "El comercio local es clave en el crecimiento economico, se obseva variedad y abundancia de productos";
        newComment30.GetComponent<CommentInstance>().tiempoInicial = 270f;
        newComment30.GetComponent<CommentInstance>().duración = 35f;
        newCommentManager1.AddNewComment(newComment30, false);


        var newComment31 = Instantiate(commentInstancePrefab);
        newComment31.GetComponentInChildren<Text>().text = "Los edificios que soportan la operacion no estan en su mejor condición, lo que demuestra falta de recursos ";
        newComment31.GetComponent<CommentInstance>().tiempoInicial = 270f;
        newComment31.GetComponent<CommentInstance>().duración = 35f;
        newCommentManager1.AddNewComment(newComment31, false);



        PrepCommentMode();
 
    }

}