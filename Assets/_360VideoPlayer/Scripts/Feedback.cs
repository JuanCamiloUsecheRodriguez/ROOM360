using UnityEngine;

public class Feedback : MonoBehaviour
{
    public float distance = 4f; //2.5
    public VideoManager videoManager = null;

    [Header("Icons")]
    public Sprite pause = null;
    public Sprite load = null;

    private SpriteRenderer spriteRenderer = null;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetUpWithCamera();

        videoManager.onPause.AddListener(TogglePause);
        videoManager.onLoad.AddListener(ToggleLoad);
    }

    private void OnDestroy()
    {
        videoManager.onPause.RemoveListener(TogglePause);
        videoManager.onLoad.RemoveListener(ToggleLoad);
    }

    private void SetUpWithCamera()
    {
        transform.parent = Camera.main.transform;
        transform.localRotation = Quaternion.identity;
        transform.localPosition = new Vector3(0f, 0f, distance);
    }
    private void TogglePause(bool isPaused)
    {
        spriteRenderer.sprite = pause;
        spriteRenderer.enabled = isPaused;
    }
    private void ToggleLoad(bool isLoaded)
    {
        spriteRenderer.sprite = load;
        spriteRenderer.enabled = !isLoaded;
    }
}
