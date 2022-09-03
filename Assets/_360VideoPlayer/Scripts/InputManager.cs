using UnityEngine;

public class InputManager : MonoBehaviour
{
    public VideoManager videoManager = null;

    public GameObject DomeOfEvents;

    public enum GameState {CommentMode,ViewMode};

    public GameState currentGameState;

    private void Start()
    {
        currentGameState = GameState.CommentMode;
    }
    private void Update()
    {
        
        if (!videoManager.IsVideoReady)
            return;
        
        OculusInput();
        KeyboardInput();
    }

    private void OculusInput()
    {
        if(OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
        {
            videoManager.PauseToggle();
        }
        //DESHABILITADAS LAS FUNCIONES DE ADELANTAR, RETROCEDER, Y CAMBIAR VID
        /*
        if(OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.LTouch))
        {
            videoManager.PreviousVideo();
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.LTouch))
        {
            videoManager.NextVideo();
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickDown, OVRInput.Controller.LTouch))
        {
            videoManager.SeekBack();
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickUp, OVRInput.Controller.LTouch))
        {
            videoManager.SeekForward();
        }
        */
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
        {
            ChangeMode();
        }
    }

    public void ChangeMode()
    {
        if(currentGameState == GameState.CommentMode)
        {
            currentGameState = GameState.ViewMode;
            DomeOfEvents.SendMessage("PrepViewMode");
        }
        else if(currentGameState == GameState.ViewMode)
        {
            currentGameState = GameState.CommentMode;
            DomeOfEvents.SendMessage("PrepCommentMode");
        }
    }



    private void KeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            videoManager.PauseToggle();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            videoManager.PreviousVideo();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            videoManager.NextVideo();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            videoManager.SeekBack();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            videoManager.SeekForward();
        }
    }
}
