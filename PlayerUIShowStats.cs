using UnityEngine;
using UnityEngine.UI;

public class PlayerUIShowStats : MonoBehaviour
{
    [Tooltip("UI Text to display Player's Name")]
    [SerializeField]
    private Text playerNameText;

    [Tooltip("UI Slider to display Player's Health")]
    [SerializeField]
    private Slider playerHealthSlider;

    [Tooltip("Pixel offset from the player target")]
    [SerializeField]
    private Vector3 screenOffset = new Vector3(0f, 50f, 0f);

    float playerHeight = 0f;
    Transform targetTransform;
    Renderer targetRenderer;
    CanvasGroup canvasGroup;
    Vector3 targetPosition;
    private PlayerCar target;
    public void SetTarget(PlayerCar target)
    {
        if (target == null)
        {
            return;
        }

        this.target = target;

        targetTransform = this.target.GetComponent<Transform>();
        targetRenderer = this.target.GetComponent<Renderer>();

        if (playerNameText != null)
        {
            playerNameText.text = this.target.photonView.Owner.NickName;
        }
        if (playerHealthSlider != null)
        {
            playerHealthSlider.maxValue = target.maxHealth;
        }
    }
    void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        canvasGroup = this.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        // show the player's health
        if (playerHealthSlider != null)
        {
            playerHealthSlider.value = target.health;
        }

        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    void LateUpdate()
    {
        if (targetRenderer != null)
        {
            this.canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
        }

        if (targetTransform != null)
        {
            targetPosition = targetTransform.position;
            targetPosition.y += playerHeight;
            this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
        }
    }

}
