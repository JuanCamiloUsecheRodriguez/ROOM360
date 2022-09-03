using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;

public class CommentManagerObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Transform player;
    public GameObject UICommentIcon;
    public GameObject UIListOfComments;
    public ScrollRect scrollRect;
    private bool isCommentListFocused;
    public static string newCommentinputText = "";
    public Text newCommentTextToShown;
    public LineRenderer lineRenderer;
    public GameObject DemoComment;
    public Text testText;
    public DomePointerEvents domeOfPointerInteractions;
    public VideoManager videoManager = null;


    public Transform content;
    public GameObject commentInstancePrefab;

    private bool pointerDown = false;
    private bool checkNewMessage = false;

    private List<GameObject> commentList;
    public GameObject container;
    private string path;

    // Start is called before the first frame update
    void Awake()
    {
        transform.LookAt(player);
        domeOfPointerInteractions = GameObject.FindGameObjectWithTag("Dome").GetComponent<DomePointerEvents>();
        videoManager = GameObject.FindGameObjectWithTag("VideoManager").GetComponent<VideoManager>();
        newCommentTextToShown = domeOfPointerInteractions.newCommentTextToShown;
        lineRenderer = domeOfPointerInteractions.lineRenderer;
        DemoComment = domeOfPointerInteractions.DemoComment;
        testText = domeOfPointerInteractions.testText;
        commentList = new List<GameObject>();
        path = domeOfPointerInteractions.path;

    }

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isCommentListFocused == true)
        {
            if((OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickUp, OVRInput.Controller.All) || Input.GetKey(KeyCode.W)) && scrollRect.verticalNormalizedPosition <= 1 )
            {
                Debug.Log("SCROLL RECT:" + scrollRect.verticalNormalizedPosition);
                scrollRect.verticalNormalizedPosition += 0.2f;
            }
            else if ((OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickDown, OVRInput.Controller.All) || Input.GetKey(KeyCode.S)) && scrollRect.verticalNormalizedPosition >= 0)
            {
                Debug.Log("SCROLL RECT:" + scrollRect.verticalNormalizedPosition);
                scrollRect.verticalNormalizedPosition -= 0.2f;
            }
        }

        if (checkNewMessage == true)
        {
            if (domeOfPointerInteractions.overlayKeyboard != null)
            {
                newCommentinputText = domeOfPointerInteractions.overlayKeyboard.text;
                newCommentTextToShown.text = newCommentinputText;
                lineRenderer.gameObject.SetActive(false);
                DemoComment.SetActive(true);
                testText.text = "";
                DemoComment.SendMessage("UpdateTimerText", domeOfPointerInteractions.capturedTime.ToString("F0"));
                if (domeOfPointerInteractions.overlayKeyboard.status == TouchScreenKeyboard.Status.Done)
                {
                    lineRenderer.gameObject.SetActive(true);
                    DemoComment.SetActive(false);
                    testText.text = "CommentMode";
                    domeOfPointerInteractions.overlayKeyboard = null;
                    Destroy(domeOfPointerInteractions.temporalMarker);
                    domeOfPointerInteractions.videoManager.PauseToggle();
                    checkNewMessage = false;

                    if(newCommentinputText != "")
                    {
                        var newComment = Instantiate(commentInstancePrefab);
                        newComment.GetComponentInChildren<Text>().text = newCommentinputText;
                        newComment.GetComponent<CommentInstance>().tiempoInicial = domeOfPointerInteractions.tiempoInicialComment;
                        newComment.GetComponent<CommentInstance>().duración = domeOfPointerInteractions.capturedTime;
                        AddNewComment(newComment, true);
                    }
                }
            }
        }

        bool AnyCommentActive = false;
        for(int i = 0; i < commentList.Count && AnyCommentActive == false; i++)
        {
            GameObject currentComment = commentList[i];
            if (currentComment.activeSelf == true)
            {
                AnyCommentActive = true;
            }
        }
        if(AnyCommentActive == true)
        {
            container.SetActive(true);
        }
        else
        {
            container.SetActive(false); 
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UICommentIcon.SetActive(false);
        UIListOfComments.SetActive(true);
        isCommentListFocused = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    { 
        UICommentIcon.SetActive(true);
        UIListOfComments.SetActive(false);
        isCommentListFocused = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (domeOfPointerInteractions.inputManager.currentGameState == InputManager.GameState.CommentMode)
        {
            if (pointerDown == false)
            {
                pointerDown = true;
                domeOfPointerInteractions.SendMessage("SetStartTimeCaptureTime", (float)videoManager.videoPlayer.time);
                domeOfPointerInteractions.SendMessage("TakeTimeAndMarkPosition", eventData);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (domeOfPointerInteractions.inputManager.currentGameState == InputManager.GameState.CommentMode)
        {
            if (pointerDown == true)
            {
                pointerDown = false;

                domeOfPointerInteractions.SendMessage("StopTimerandOpenKeyboard", false);
                checkNewMessage = true;
            }
        }
    }

    public void AddNewComment(GameObject newComment, bool seGuarda)
    {
        Transform contentTransform = content;
        newComment.transform.SetParent(contentTransform);
        newComment.transform.localScale = Vector2.one;
        newComment.transform.localRotation = Quaternion.identity;
        newComment.transform.localPosition = Vector3.zero;
        commentList.Add(newComment);
        StartCoroutine("StopCommentPreview", newComment);
        SaveCommentToLog(newComment, seGuarda);
    }


    public void ResetTimerForComments()
    {
        foreach (GameObject comment in commentList)
        {
            //comment.GetComponent<CommentInstance>().SetActivationTimerForComment();
            StartCoroutine("StartTimer", comment);
        }
    }

    private IEnumerator StartTimer(GameObject comment)
    {
        yield return new WaitForSeconds(comment.GetComponent<CommentInstance>().tiempoInicial);
        comment.SetActive(true);
        yield return new WaitForSeconds(comment.GetComponent<CommentInstance>().duración);
        comment.SetActive(false);
    }

    public void HideAllComments()
    {
        StopAllCoroutines();
        foreach (GameObject comment in commentList)
        {
            comment.SetActive(false);
        }
    }

    private IEnumerator StopCommentPreview(GameObject comment)
    {
        yield return new WaitForSeconds(6f);
        comment.SetActive(false);
    }

    private void SaveCommentToLog(GameObject comment, bool seguarda)
    {
        if (seguarda)
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, "");
            }
            else if (!(new FileInfo(path).Length == 0))
            {
                string space = "\n";
                File.AppendAllText(path, space);
            }

            string content = comment.GetComponent<CommentInstance>().tiempoInicial + "," + comment.GetComponent<CommentInstance>().duración + "," + comment.transform.Find("Text").GetComponent<Text>().text;
            File.AppendAllText(path, content);
        }
    }
}
