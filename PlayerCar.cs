using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PlayerCar : MonoBehaviourPunCallbacks, IPunObservable
{
    [Tooltip("Local player isntance, to check it's exists int the current game/scene")]
    public static GameObject LocalPlayerInstance;

    [Tooltip("The speed of the car")]
    public float forwardSpeed = 1500f;

    [Tooltip("The tires we want to rotate")]
    public Transform[] tiresToRotate;

    [Tooltip("The sensitivity of our turning")]
    public Vector3 TurningAngle = new Vector3(0, 150, 0);

    //[HideInInspector]
    [Tooltip("The actual Health of our player")]
    public float health = 100f;

    [Tooltip("The max/initlalized Health of our player")]
    public float maxHealth = 100f;

    [Tooltip("The caused bonus damage of the car")]
    public float damage = 1f;

    [Tooltip("The Player's FireButton")]
    [SerializeField]
    public GameObject FireButton;

    [Tooltip("Drag the missile prefab here")]
    public GameObject missilePrefab;

    [Tooltip("This amount of time need to pass to shoot a missile again")]
    public float timeToFireAgain = 0.35f;

    [SerializeField]
    private float baseBoostSeed = 1000f;

    [Tooltip("Shows the player health and name to others in game")]
    [SerializeField]
    private GameObject PlayerInformations;

    [SerializeField]
    private GameObject MoneySpawner;

    [SerializeField]
    private GameObject CarExplosion;

    [Tooltip("The amount of score the player collects in the actual gameplay")]
    private int moneyAmount = 0;

    private float carDirection;
    private bool isGrounded = false;

    private Rigidbody rigidBody;
    private PhotonView thisPhotonView;

    // variables to deal with time and shoot again
    private float nextTimeToFire = 0f;
    private float elapsedTime = 0f;

    private GameObject fireButton;

    private PlayerCar lastTouchedBy;
    private PhotonView destroyedByThisPhotonView;

    private string destroyedByName;
    private int lastHitViewID;

    // The minimum collisison force need to send a daamge RPC to the touched car.
    private int minCollissionVelocity = 2;

    // with this we avoid to healthup getting called multiple times on one collision
    private bool healthGatePassed = false;
    private bool damageZonePassed = false;

    private bool OnDestroyedIsCalled = false;

    private Vector3 zeroVector = new Vector3(0f, 0f, 0f);

    // Doesnt affect the turning just the tires behaviour, cosmetic
    private Vector3 TireRotatingAngle = new Vector3(0, 30, 0);

    public void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = this.gameObject;
        }
        // to survive scene synchronization
        DontDestroyOnLoad(this.gameObject);
    }
    public void Start()
    {
        // cache singleton reference
        GameManager GameManagerInstance = GameManager.GetInstance();

        rigidBody = this.gameObject.GetComponent<Rigidbody>();

        thisPhotonView = this.gameObject.GetComponent<PhotonView>();

        GameManager.GetInstance().localPlayer = this;

        cameraController cameracontroller = this.gameObject.GetComponent<cameraController>();
        if (cameracontroller != null)
        {
            if (photonView.IsMine)
            {
                cameracontroller.FollowPlayer();
            }
        }
        else
        {
            Debug.Log("No cameraController attached to playerCar script on car prefab!");
        }

        if (PlayerInformations != null)
        {
            GameObject playerInfomarionsUI = Instantiate(PlayerInformations);
            playerInfomarionsUI.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.Log("No playerInformations GameObject attached to PlayerCar.");
        }

        if (!photonView.IsMine)
        {
            return;
        }
        if (FireButton != null)
        {
            fireButton = Instantiate(FireButton);
            fireButton.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.Log("FireButton instantiate not working");
        }

        Input.multiTouchEnabled = true;

        StartCoroutine("CarRecover");

        SceneManager.sceneLoaded += OnSceneLoaded;

        health = maxHealth;
    }

    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        if (Application.isMobilePlatform)
        {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Vector2 touchposistion = Input.touches[i].position;
                    if (!EventSystem.current.IsPointerOverGameObject(Input.touches[i].fingerId))
                    {
                        if (Input.touches[i].phase == TouchPhase.Began)
                        {
                            if (touchposistion.x > Screen.width * 0.5f && !EventSystem.current.IsPointerOverGameObject(Input.touches[i].fingerId))
                            {
                                carDirection = 1;
                            }
                            // rotate to the left
                            else if (touchposistion.x < Screen.width * 0.5f && !EventSystem.current.IsPointerOverGameObject(Input.touches[i].fingerId))
                            {
                                carDirection = -1;
                            }
                            else
                            {
                                carDirection = 0;
                            }
                        }
                    }
                }
            }
            // We didn't press anything, stop rotating
            else
            {
                carDirection = 0;
            }
        }
        // Keyboard controls, testing purposes only
        else
        {
            carDirection = Input.GetAxis("Horizontal");
        }
        Turn(carDirection);

        elapsedTime += Time.deltaTime;
    }

    public void FixedUpdate()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        // the car only can speedup, when it touches the ground
        if (isGrounded)
        {
            rigidBody.AddRelativeForce(Vector3.forward * forwardSpeed * Time.deltaTime);
        }

        if (this.health <= 0f && !OnDestroyedIsCalled)
        {
            // we only want that this coorotuine runs once
            OnDestroyedIsCalled = true;
            OnDestroyed();
        }
    }

    public void Turn(float carDirection)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        if (carDirection > 0)
        {
            // turn right
            Quaternion deltaRotation = Quaternion.Euler(TurningAngle * Time.deltaTime);
            rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);

            tiresToRotate[0].localRotation = Quaternion.Euler(TireRotatingAngle);
            tiresToRotate[1].localRotation = Quaternion.Euler(TireRotatingAngle);
        }
        else if (carDirection < 0)
        {
            // rotating/turn left
            Quaternion deltaRotation = Quaternion.Euler(-TurningAngle * Time.deltaTime);
            rigidBody.MoveRotation(rigidBody.rotation * deltaRotation);

            tiresToRotate[0].localRotation = Quaternion.Euler(-TireRotatingAngle);
            tiresToRotate[1].localRotation = Quaternion.Euler(-TireRotatingAngle);
        }
        else
        {
            tiresToRotate[0].localRotation = Quaternion.Euler(zeroVector);
            tiresToRotate[1].localRotation = Quaternion.Euler(zeroVector);
        }
    }

    public void Shoot()
    {

        if (elapsedTime >= nextTimeToFire)
        {
            nextTimeToFire = elapsedTime + timeToFireAgain;
            ShootMissile();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (other.gameObject.tag == "RampBoost")
        {
            Boost(0.60f, false);
        }

        if (other.gameObject.tag == "Health" && !healthGatePassed)
        {
            healthGatePassed = true;
            SetHealth(GameManager.GetInstance().GetHealthRepair());
        }

        if (other.gameObject.tag == "DamageAndRespawn" && !damageZonePassed)
        {
            damageZonePassed = true;
            SetHealth(GameManager.GetInstance().GetZoneDamage());
            CallRespawn();
        }

        if (other.gameObject.tag == "Teleport")
        {
            this.transform.position = GameManager.GetInstance().GetRandomSpawnPoint();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "DamageArea")
        {
            health -= 10f * Time.deltaTime;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Health")
        {
            healthGatePassed = false;
        }
        if (other.gameObject.tag == "DamageAndRespawn")
        {
            damageZonePassed = false;
        }
    }
    private void OnCollisionEnter(Collision collisionInfo)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (collisionInfo.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }

        if (collisionInfo.gameObject.tag == "Car")
        {
            var collisionVelocity = collisionInfo.relativeVelocity.magnitude - this.rigidBody.velocity.magnitude;

            if (minCollissionVelocity < collisionVelocity)
            {
                collisionInfo.collider.GetComponent<PhotonView>().RPC("DamageCar", RpcTarget.All, collisionVelocity + damage, this.photonView.ViewID);
            }
        }
    }

    private void OnCollisionExit(Collision collisionInfo)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (collisionInfo.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }

    public PhotonView GetThisPohotonView()
    {
        return thisPhotonView;
    }

    [PunRPC]
    public void DamageCar(float damageAmount, int colliderCarID)
    {
        this.health -= damageAmount;
        lastHitViewID = colliderCarID;
    }

    //Taking damage from others missiles
    [PunRPC]
    public void TakeDamageMissile(float missileDamage, int attackerCarID)
    {
        this.health -= missileDamage;
        lastHitViewID = attackerCarID;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // send data to other players
            stream.SendNext(health);
        }
        else
        {
            // receive data from other players
            this.health = (float)stream.ReceiveNext();
        }
    }
    private void ShootMissile()
    {
        Vector3 spawnOffset = transform.forward * 3f + transform.up * 0.6f;
        GameObject Missile = PhotonNetwork.Instantiate(missilePrefab.name, transform.position + spawnOffset, transform.localRotation);
        Missile.GetComponent<Missile>().photonView.RPC("SetOwnerRPC", RpcTarget.All, thisPhotonView.ViewID);
    }
    public void Boost(float multiplier, bool upwards)
    {
        if (upwards)
        {
            rigidBody.AddRelativeForce(Vector3.up * 200);
        }
        rigidBody.AddRelativeForce(Vector3.forward * baseBoostSeed * multiplier);
    }
    private void OnDestroyed()
    {
        photonView.RPC("SpawnCarExplosionLocally", RpcTarget.All, transform.position);

        if (lastHitViewID != 0 && PhotonView.Find(lastHitViewID) != null)
        {
            destroyedByThisPhotonView = PhotonView.Find(lastHitViewID);
            destroyedByName = destroyedByThisPhotonView.Owner.NickName;
            PlayerCar otherCar = destroyedByThisPhotonView.GetComponent<PlayerCar>();
            SetLastTouchedCar(otherCar);
            destroyedByThisPhotonView.GetComponent<PhotonView>().RPC("AddKill", RpcTarget.All);
        }
        else
        {
            destroyedByName = "UNKNOWN";
        }
        GameManager.GetInstance().ShowDeathUI(destroyedByName);

        GameManager.GetInstance().IncreaseDeathAmount();

        Vector3 moneySpawnPosition = new Vector3(this.transform.position.x, 0, this.transform.position.z);
        int moneyAmountToSpawn = GameManager.GetInstance().GetScore() / 30;
        if (moneyAmountToSpawn < 1)
        {
            moneyAmountToSpawn = 1;
        }
        MoneySpawner.GetComponent<MoneySpawner>().SpawnMoney(moneySpawnPosition, moneyAmountToSpawn);


        GameObject[] FireButtonsToDestroy = GameObject.FindGameObjectsWithTag("FireButton");
        for (int i = 0; i < FireButtonsToDestroy.Length; i++)
        {
            Destroy(FireButtonsToDestroy[i]);
        }

        //photonView.RPC("DecreaseActiveCarNumberCO", RpcTarget.Others);
        this.gameObject.SetActive(false);
        PhotonNetwork.Destroy(gameObject);
    }

    private void SetLastTouchedCar(PlayerCar other)
    {
        lastTouchedBy = other;
    }

    private PlayerCar GetLastTouchedCar()
    {
        return lastTouchedBy;
    }

    private void IncreaseKillCountToKiller(Player othercar)
    {
        photonView.RPC("AddKill", othercar);
    }

    //called by killed player
    [PunRPC]
    public void AddKill()
    {
        if (photonView.IsMine)
        {
            GameManager.GetInstance().IncreaseKillAmount();
            GameManager.GetInstance().AddScore(50);
        }
    }

    [PunRPC]
    public void SpawnCarExplosionLocally(Vector3 position)
    {
        Instantiate(CarExplosion, position, Quaternion.identity);
    }

    private float GetHealth()
    {
        return this.health;
    }

    private void SetHealth(float difference)
    {
        this.health += difference;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void SetMoney(int moneyAmount)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        this.moneyAmount += moneyAmount;
        GameManager.GetInstance().AddScore(moneyAmount);
    }

    private void CallRespawn()
    {
        this.rigidBody.velocity = zeroVector;
        this.transform.position = GameManager.GetInstance().GetRandomSpawnPoint();
    }

    private IEnumerator CarRecover()
    {
        float minHeight = -5f;
        float maxHeight = 30f;
        while (true)
        {
            yield return new WaitForSeconds(3f);
            // prevent out of track situations
            if (this.transform.position.y < minHeight || maxHeight < this.transform.position.y)
            {
                this.rigidBody.velocity = zeroVector;
                this.transform.position = GameManager.GetInstance().GetRandomSpawnPoint();
            }
        }
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if (!photonView.IsMine)
        {
            return;
        }
        GameObject UIGameObject = Instantiate(this.PlayerInformations);
        UIGameObject.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        GameObject fireButton = Instantiate(this.FireButton);
        fireButton.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
    }

    public override void OnDisable()
    {
        // call the base to remove unnecsary callbacks
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
