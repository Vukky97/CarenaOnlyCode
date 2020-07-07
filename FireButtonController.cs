using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FireButtonController : MonoBehaviour
{
    private static FireButtonController Instance;

    CanvasGroup canvasGroup;
    private PlayerCar target;
    private bool isFireButtonDown = false;

    [SerializeField]
    private Button FireButton;

    [SerializeField]
    private Button FireButtonLeft;

    void Awake()
    {
        Instance = this;
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        canvasGroup = this.GetComponent<CanvasGroup>();
        FireButton = this.GetComponent<Button>();
    }
    public static FireButtonController GetInstance()
    {
        return Instance;
    }

    private void Start()
    {
        FireButton.onClick.AddListener(ReleaseShoot);
    }
    private void ReleaseShoot()
    {
        target.Shoot();
    }
    public void SetTarget(PlayerCar target)
    {
        Debug.Log("Set target called on GO: " + this.gameObject.name);
        if (target == null)
        {
            Debug.LogWarning("Can't set target in PlayerUIInteractives");
            return;
        }
        this.target = target;
        SetAnotherButton(target);
    }

    public void SetAnotherButton(PlayerCar target)
    {
        if (FireButtonLeft != null)
        {
            FireButtonLeft.GetComponent<FireButtonController>().SetTarget(target);
        }
    }

}
