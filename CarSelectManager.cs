using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarSelectManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> Cars = new List<GameObject>();
    private int SelectedCarIndex = 0;
    private float stepSize = 3f;
    private int actualCarIndex = 0;
    private Vector3 motionVector = new Vector3(3f, 0f, 0f);
    private Camera MainCamera;

    [SerializeField]
    private int MenuSceneIndex = 0;
    private Text moneyText;
    public MoneyManager moneyManager;

    public CarSelectUIManager UIManager;

    private void Awake()
    {
        MainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }
    void Start()
    {
        actualCarIndex = PlayerPrefsManager.GetSelectedCar();

        // positioning the camera
        MainCamera.transform.position += motionVector * actualCarIndex;

        RefreshStat();
    }

    public void PreviousCar()
    {
        if (actualCarIndex <= 0)
        {
            return;
        }
        actualCarIndex--;
        MainCamera.transform.position -= motionVector;
        RefreshStat();
    }
    public void NextCar()
    {
        if (actualCarIndex == Cars.Count - 1)
        {
            return;
        }
        actualCarIndex++;
        MainCamera.transform.position += motionVector;
        RefreshStat();
    }

    public void SelectCar()
    {
        PlayerPrefs.SetInt("SelectedCar", actualCarIndex);
        BackToMainMenu();
    }

    public void RefreshStat()
    {
        PlayerCar actualCar = Cars[actualCarIndex].GetComponent<PlayerCar>();
        string speed = actualCar.forwardSpeed.ToString().Remove(3);
        string health = actualCar.maxHealth.ToString();
        string damage = actualCar.damage.ToString();
        string turning = actualCar.TurningAngle.y.ToString();

        UIManager.ShowStats(speed, health, damage, turning);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(MenuSceneIndex);
    }
}
