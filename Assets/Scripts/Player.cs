using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GoogleMobileAds.Api;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    static public bool GameOver;
    static public bool Respawned;
    static public bool Restarted;
    static public bool Invincible;
    static public bool CallInvincibility;
    static public bool Sensor = false;
    static public bool Vibration = true;
    static public bool Music = true;

    static public int Score = 0;
    static public int Life = 100;
    static public int HighScore = 0;
    static public int HighLevel = 0;

    static public int LanguageNumber;

    [Header("SISTEMAS")]
    public GameObject PlayerCamera;
    public List<int> TilesOld = new List<int>();
    public int TileLast;

    public float SensorSpeed = 2.5f;
    public float MaxDistance = 25f;

    public int[] ResolutionsWidth;
    public int[] ResolutionsHeight;
    public Dropdown ResolutionDropdown;

    [Header("ANÚNCIOS")]
    public bool Ads;
    public GameObject AdsController;

    [Header("JOGADOR")]
    public int Damage = 5;
    public int Level = 1;
    public List<int> LevelOld = new List<int>();
    public Material BlueMaterial, RedMaterial, BlackMaterial, YellowMaterial;
    public string BlueColor = "00AFFFFF", RedColor = "C30000FF", BlackColor = "000000FF", YellowColor = "AF6405FF";

    [Header("FÍSICA")]
    // Força padrão de Subida e Descida.
    public float DefaultForce = 15f;
    public float DistanceForce = 25f;

    public float RedForce;
    public float BlueForce;

    public float RedDistance;
    public float BlueDistance;

    // Força de Perigo e de Curva.
    public float DangerForce = 5f;
    public float TurnForce = 25f;
    public float GroundTurnForce = 50f;

    public float RayDistance = 250f;

    private float BackupTurnForce;

    // Rigidbody do Jogador.
    private Rigidbody RigidbodyComponent;

    [Header("INTERFACE")]
    // Texto de Níveis.
    public Text[] LevelText;
    // Texto de Pontos.
    public Text[] ScoreText;
    // Texto de Recordes.
    public Text[] HighScoreText;
    // Botão de Inversão.
    public Button InvertButton;
    // Botão da Esquerda.
    public Button LeftButton;
    // Botão da Direita.
    public Button RightButton;
    // Barra de Vida.
    public Image LifeBar;
    // Interface de Morte.
    public GameObject DeathUI;
    // Interface de Pause.
    public GameObject PauseUI;
    // Aviso de Respawn.
    public GameObject RespawnNotice;

    public AudioSource MusicAudio, ZapAudio, PopAudio;
    public AudioClip[] PopsAudio;

    public Toggle[] ToggleComponent;

    [Header("ONDAS")]
    // Objetos das Ondas.
    public GameObject RedWave;
    public GameObject RedBottomWave;
    public GameObject BlueWave;
    public GameObject BlueBottomWave;
    // Condições das Ondas.
    public bool RedOne, BlueOne, RedBottomOne, BlueBottomOne;
    public bool RedTwo, BlueTwo, RedBottomTwo, BlueBottomTwo;

    [Header("AMBIENTE")]
    public GameObject Sky;

    public float SkyLerpSpeed = 25f;

    public Color SkyColorTop, SkyColorBottom;

    private List<Color> LevelSkyColorTopOld = new List<Color>();
    private List<Color> LevelSkyColorBottomOld = new List<Color>();

    private Material SkyMaterial;
    private Renderer SkyRenderer;

    [Header("IDIOMAS")]
    static public string StaticPopUpPointText;

    public string[] PopUpPointText;
    public string[] PopUpLifeText;

    private void Awake()
    {

        // PlayerPrefs.Init();

#if UNITY_EDITOR
        // PlayerPrefs.DeleteAll();
#endif

#if UNITY_STANDALONE
        ResolutionDropdown.gameObject.SetActive(true);
#endif

        BackupTurnForce = TurnForce;

        if (PlayerPrefs.GetString("Sensor") == "True")
        {
            Sensor = true;
        }
        else if (PlayerPrefs.GetString("Sensor") == "False")
        {
            Sensor = false;
        }

        ToggleComponent[0].isOn = Sensor;

        if (PlayerPrefs.GetString("Vibration") == "True")
        {
            Vibration = true;
        }
        else if (PlayerPrefs.GetString("Vibration") == "False")
        {
            Vibration = false;
        }

        ToggleComponent[1].isOn = Vibration;

        if (PlayerPrefs.GetString("Music") == "True")
        {
            Music = true;
        }
        else if (PlayerPrefs.GetString("Music") == "False")
        {
            Music = false;
        }

        ToggleComponent[2].isOn = Music;
        MusicAudio.mute = !Music;

        switch (Application.systemLanguage)
        {
            case SystemLanguage.English:
                {
                    LanguageNumber = 1;
                    break;
                }

            case SystemLanguage.Portuguese:
                {
                    LanguageNumber = 0;
                    break;
                }

            default:
                {
                    LanguageNumber = 1;
                    break;
                }
        }

        BlueMaterial.color = HexToColor(BlueColor);
        RedMaterial.color = HexToColor(RedColor);
        BlackMaterial.color = HexToColor(BlackColor);
        YellowMaterial.color = HexToColor(YellowColor);
    }

    // Use this for initialization.
    private void Start()
    {
        StaticPopUpPointText = PopUpPointText[LanguageNumber];

        RigidbodyComponent = GetComponent<Rigidbody>();

        SkyRenderer = Sky.GetComponent<Renderer>();
        SkyMaterial = SkyRenderer.material;

        GameOver = true;

        SpawnTile(0, 0, "TilesDefault");
        SpawnTile(0, 5, "TilesDefault");
        SpawnTile(0, 10, "TilesDefault");
        SpawnTile(0, 15, "TilesDefault");

        LevelOld.Add(Level);
    }

    // Update is called once per frame.
    private void Update()
    {
        if (transform.position.y < -6.75f)
        {
            transform.position = new Vector3(transform.position.x, -6.75f, 0);
        }

        if (transform.position.x < -5.5f)
        {
            transform.position = new Vector3(-5.5f, transform.position.y, 0);
        }

        if (transform.position.x > 5.5f)
        {
            transform.position = new Vector3(5.5f, transform.position.y, 0);
        }

        if (!GameOver)
        {
            // Controle pelo accelerometer.
            if (Input.acceleration.x != 0 && Sensor)
            {
                if (Input.acceleration.x > 0)
                {
                    TurnRight((Input.acceleration.x) * SensorSpeed);
                }

                if (Input.acceleration.x < 0)
                {
                    TurnLeft((Input.acceleration.x * -1f) * SensorSpeed);
                }
            }

            // Controle pelo mouse.
            if (Input.GetButtonDown("Jump") || Input.GetButtonDown("Fire2"))
            {
                InvertRotation();
            }

            // Controle pelo keyboard ou joystick.
            if (Input.GetAxis("Horizontal") > 0)
            {
                TurnRight(Input.GetAxis("Horizontal"));
            }
            if (Input.GetAxis("Horizontal") < 0)
            {
                TurnLeft(Input.GetAxis("Horizontal") * -1f);
            }

            // Controle pelo touch.
            if (LeftButton.GetComponent<PressingButton>().Pressing)
            {
                TurnLeft(LeftButton.GetComponent<PressingButton>().InputPressing);
            }
            if (RightButton.GetComponent<PressingButton>().Pressing)
            {
                TurnRight(RightButton.GetComponent<PressingButton>().InputPressing);
            }
        }

        // Receber valor do Recorde salvo.
        HighScore = PlayerPrefs.GetInt("HighScore", 0);
        // Atualizar Recorde se o total do Pontos for maior que o mesmo.
        if (Score > HighScore)
        {
            PlayerPrefs.SetInt("HighScore", Score);
        }

        // Receber valor do Nível salvo.
        HighLevel = PlayerPrefs.GetInt("HighLevel", 0);
        // Atualizar Nível se o total do Níveis for maior que o mesmo.
        if (Level > HighLevel)
        {
            PlayerPrefs.SetInt("HighLevel", Level);
        }

        // Atualizar texto de Nível.
        for (int i = 0; i < LevelText.Length; i++)
        {
            LevelText[i].text = Level.ToString();
        }

        // Atualizar texto de Pontos.
        for (int i = 0; i < ScoreText.Length; i++)
        {
            ScoreText[i].text = Score.ToString();
        }

        // Atualizar texto de Recorde.
        for (int i = 0; i < HighScoreText.Length; i++)
        {
            HighScoreText[i].text = PlayerPrefs.GetInt("HighScore", 0).ToString();
        }

        // Atualizar Nível de acordo com a quantidade de pontos.
        if (Score % (10 * Level) == 0 && Score > 0)
        {
            Level += 1;

            if (!LevelOld.Contains(Level))
            {
                int AddLife = Random.Range(50, 100);

                Life += AddLife;

                if (Life > 100)
                {
                    Life = 100;
                }

                SpawnPopUp("+" + AddLife.ToString() + " " + PopUpLifeText[LanguageNumber]);

                LevelOld.Add(Level);
            }
        }

        // Modificar comprimento da barra de acordo com a quantidade de Vida.
        LifeBar.fillAmount = Amount(Life, 0, 100, 0, 1);

        // Reduzir Nível de acordo com a quantidade de pontos.
        if (Score < ((10 * Level) - 10) && Level > 1)
        {
            Level -= 1;
        }

        // Morte do Jogador.
        if (Life <= 0)
        {
            GameOver = true;
            PlayerCamera.GetComponent<MoveCamera>().Shaking = false;
            PlayerCamera.transform.position = new Vector3(0, PlayerCamera.transform.position.y, PlayerCamera.transform.position.z);
            DeathUI.SetActive(true);
        }

        if (CallInvincibility)
        {
            StartCoroutine(Invincibility());
            CallInvincibility = false;
        }

        if (Ads)
        {
            AdsController.SetActive(true);
        }
        else
        {
            AdsController.SetActive(false);
        }

        if (PlayerCamera.transform.position.y <= 0f && Restarted)
        {
            RedWave.SetActive(false);
            BlueWave.SetActive(false);
            RedBottomWave.SetActive(false);
            BlueBottomWave.SetActive(false);

            GameObject[] Tiles = GameObject.FindGameObjectsWithTag("Tile");

            foreach (GameObject Go in Tiles)
            {
                Destroy(Go);
            }

            TilesOld.Clear();
            LevelOld.Clear();

            SpawnTile(0, 0, "TilesDefault");
            SpawnTile(0, 5, "TilesDefault");
            SpawnTile(0, 10, "TilesDefault");
            SpawnTile(0, 15, "TilesDefault");

            LevelSkyColorTopOld.Clear();
            LevelSkyColorBottomOld.Clear();
            RandomSkyColor();

            GameOver = false;
            Restarted = false;
        }

        GameObject[] Tile = GameObject.FindGameObjectsWithTag("Tile");

        foreach (GameObject Go in Tile)
        {
            if (Vector3.Distance(transform.position, Go.transform.position) > MaxDistance && !Player.Restarted)
            {
                foreach (Transform GoChild in Go.transform)
                {
                    GoChild.gameObject.SetActive(false);
                }
            }
            else
            {
                foreach (Transform GoChild in Go.transform)
                {
                    GoChild.gameObject.SetActive(true);
                }
            }
        }

        // Desativar outros tipos de sons, de acordo com a Música.
        ZapAudio.gameObject.SetActive(Music);
        PopAudio.gameObject.SetActive(Music);
#if UNITY_STANDALONE
        Screen.SetResolution(ResolutionsWidth[ResolutionDropdown.value], ResolutionsHeight[ResolutionDropdown.value], false);
#endif
    }

    private void FixedUpdate()
    {
        RandomSkyColor();

        if (!GameOver)
        {
            // Desativar Kinematic do Jogador.
            RigidbodyComponent.isKinematic = false;

            // Aumentar força de acordo com a Distância.
            if (RedDistance < BlueDistance)
            {
                RedForce = DistanceForce;
                BlueForce = DefaultForce;
            }
            else if (BlueDistance < RedDistance)
            {
                RedForce = DefaultForce;
                BlueForce = DistanceForce;
            }

            RaycastHit Hit;
            int LayerMask = 1 << 8;

            // Lado positivo (Azul).
            if (Physics.SphereCast(transform.position, 0.5f, transform.TransformDirection(Vector3.forward), out Hit, RayDistance, LayerMask))
            {
                Blue(Hit, transform.position);
            }

            // Lado negativo (Vermelho).
            if (Physics.SphereCast(transform.position, 0.5f, transform.TransformDirection(Vector3.back), out Hit, RayDistance, LayerMask))
            {
                Red(Hit, transform.position);
            }

            if (Physics.SphereCast(transform.position, 0.5f, transform.TransformDirection(Vector3.left), out Hit, 0.5f, LayerMask) && Physics.SphereCast(transform.position, 0.5f, transform.TransformDirection(Vector3.right), out Hit, 0.5f, LayerMask))
            {
                ToUp(35f);
                Debug.Log("Ficou Preso!");
            }
            else if ((Physics.SphereCast(transform.position, 0.5f, transform.TransformDirection(Vector3.left), out Hit, 0.5f, LayerMask) || Physics.SphereCast(transform.position, 0.5f, transform.TransformDirection(Vector3.right), out Hit, 0.5f, LayerMask)) && transform.position.x < -5f)
            {
                ToUp(35f);
                Debug.Log("Ficou Preso!");
            }
            else if ((Physics.SphereCast(transform.position, 0.5f, transform.TransformDirection(Vector3.left), out Hit, 0.5f, LayerMask) || Physics.SphereCast(transform.position, 0.5f, transform.TransformDirection(Vector3.right), out Hit, 0.5f, LayerMask)) && transform.position.x > 5f)
            {
                ToUp(35f);
                Debug.Log("Ficou Preso!");
            }

            // Ativar e desativar ondas de acordo com as condições.
            if (RedOne || RedTwo)
            {
                RedWave.SetActive(true);
            }
            else if (!RedOne || !RedTwo)
            {
                RedWave.SetActive(false);
            }

            if (BlueOne || BlueTwo)
            {
                BlueWave.SetActive(true);
            }
            else if (!BlueOne && !BlueTwo)
            {
                BlueWave.SetActive(false);
            }

            if (RedBottomOne || RedBottomTwo)
            {
                RedBottomWave.SetActive(true);
            }
            else if (!RedBottomOne && !RedBottomTwo)
            {
                RedBottomWave.SetActive(false);
            }

            if (BlueBottomOne || BlueBottomTwo)
            {
                BlueBottomWave.SetActive(true);
            }
            else if (!BlueBottomOne && !BlueBottomTwo)
            {
                BlueBottomWave.SetActive(false);
            }

        }
        else
        {
            // Ativar Kinematic do Jogador.
            RigidbodyComponent.isKinematic = true;
        }
    }

    private void Blue(RaycastHit Hit, Vector3 StartPosition)
    {
        if (Hit.collider.tag == "Red")
        {
            ToDown(BlueForce);
            RedWave.transform.position = Hit.point;
            RedOne = true;
            BlueDistance = Hit.distance;
        }
        else if (Hit.collider.tag != "Red")
        {
            RedOne = false;
        }

        if (Hit.collider.tag == "RedBottom")
        {
            ToUp(BlueForce);
            RedBottomWave.transform.position = Hit.point;
            RedBottomOne = true;
            BlueDistance = Hit.distance;
        }
        else if (Hit.collider.tag != "RedBottom")
        {
            RedBottomOne = false;
        }

        if (Hit.collider.tag == "Blue")
        {
            ToUp(BlueForce);
            BlueWave.transform.position = Hit.point;
            BlueOne = true;
            BlueDistance = Hit.distance;
        }
        else if (Hit.collider.tag != "Blue")
        {
            BlueOne = false;
        }

        if (Hit.collider.tag == "BlueBottom")
        {
            ToDown(BlueForce);
            BlueBottomWave.transform.position = Hit.point;
            BlueBottomOne = true;
            BlueDistance = Hit.distance;
        }
        else if (Hit.collider.tag != "BlueBottom")
        {
            BlueBottomOne = false;
        }

        if (Hit.collider.tag == "Black")
        {
            ToDanger();
        }

        Vector3 EndPosition = Hit.point;
        Debug.DrawLine(StartPosition, EndPosition, Color.green);
        // Debug.Log("Blue = " + Hit.collider.tag);
    }

    private void Red(RaycastHit Hit, Vector3 StartPosition)
    {
        if (Hit.collider.tag == "Red")
        {
            ToUp(RedForce);
            RedWave.transform.position = Hit.point;
            RedTwo = true;
            RedDistance = Hit.distance;
        }
        else if (Hit.collider.tag != "Red")
        {
            RedTwo = false;
        }

        if (Hit.collider.tag == "RedBottom")
        {
            ToDown(RedForce);
            RedBottomWave.transform.position = Hit.point;
            RedBottomTwo = true;
            RedDistance = Hit.distance;
        }
        else if (Hit.collider.tag != "RedBottom")
        {
            RedBottomTwo = false;
        }

        if (Hit.collider.tag == "Blue")
        {
            ToDown(RedForce);
            BlueWave.transform.position = Hit.point;
            BlueTwo = true;
            RedDistance = Hit.distance;
        }
        else if (Hit.collider.tag != "Blue")
        {
            BlueTwo = false;
        }

        if (Hit.collider.tag == "BlueBottom")
        {
            ToUp(RedForce);
            BlueBottomWave.transform.position = Hit.point;
            BlueBottomTwo = true;
            RedDistance = Hit.distance;
        }
        else if (Hit.collider.tag != "BlueBottom")
        {
            BlueBottomTwo = false;
        }

        if (Hit.collider.tag == "Black")
        {
            ToDanger();
        }

        Vector3 EndPosition = Hit.point;
        Debug.DrawLine(StartPosition, EndPosition, Color.green);
        // Debug.Log("Red = " + Hit.collider.tag);
    }

    private void GetDamage(Collision Other)
    {
        if (Other.collider.tag == "Black" && !GameOver && !Invincible)
        {
            Life -= Damage;

            SpawnPopUp("-" + Damage.ToString() + " " + PopUpLifeText[LanguageNumber]);

            StartCoroutine(PlayerCamera.GetComponent<MoveCamera>().Shake(0.25f, 3.5f));
            if (Vibration)
            {
#if UNITY_ANDROID
                Handheld.Vibrate();
#endif
            }
            ZapAudio.mute = false;
        }
    }

    private void OnCollisionEnter(Collision Other)
    {
        GetDamage(Other);

        if (Other.collider.tag != "Black")
        {
            PopAudio.PlayOneShot(PopsAudio[Random.Range(0, PopsAudio.Length)]);
        }
    }

    private void OnCollisionStay(Collision Other)
    {
        GetDamage(Other);

        TurnForce = GroundTurnForce;

        if (Other.collider.tag == "Blue" || Other.collider.tag == "BlueBottom" || Other.collider.tag == "Red" || Other.collider.tag == "RedBottom" || Other.collider.tag == "Untagged")
        {
            transform.parent = Other.transform;
        }
    }

    private void OnCollisionExit(Collision Other)
    {
        if (Other.collider.tag == "Black")
        {
            ZapAudio.mute = true;
            StopCoroutine(PlayerCamera.GetComponent<MoveCamera>().Shake(0.25f, 3.5f));
            PlayerCamera.GetComponent<MoveCamera>().Shaking = false;
            Vector3 OriginalPositionX = new Vector3(0, PlayerCamera.transform.position.y, PlayerCamera.transform.position.z);
            PlayerCamera.transform.position = Vector3.Lerp(PlayerCamera.transform.position, OriginalPositionX, PlayerCamera.GetComponent<MoveCamera>().SmoothSpeed);
        }

        // Em teste.
        if (Other.collider)
        {
            TurnForce = BackupTurnForce;
        }

        if (Other.collider.tag == "Blue" || Other.collider.tag == "BlueBottom" || Other.collider.tag == "Red" || Other.collider.tag == "RedBottom" || Other.collider.tag == "Untagged")
        {
            transform.parent = null;
        }
    }

    private float Amount(float Value, float InMin, float InMax, float OutMin, float OutMax)
    {
        return (Value - InMin) * (OutMax - OutMin) / (InMax - InMin) + OutMin;
    }

    private Color HexToColor(string Hex)
    {
        byte r = byte.Parse(Hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(Hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(Hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255);
    }

    public void GameOverChange(bool Bool)
    {
        GameOver = Bool;
    }

    public void UseSensor(bool Bool)
    {
        Sensor = Bool;
        PlayerPrefs.SetString("Sensor", Bool.ToString());
    }

    public void UseVibration(bool Bool)
    {
        Vibration = Bool;
        PlayerPrefs.SetString("Vibration", Bool.ToString());
    }

    public void UseMusic(bool Bool)
    {
        Music = Bool;
        MusicAudio.mute = !Music;
        PlayerPrefs.SetString("Music", Bool.ToString());
    }

    public void Restart()
    {
        transform.parent = null;
        transform.rotation = new Quaternion(-0.7071068f, 0, 0, 0.7071068f);
        transform.position = new Vector3(0, -6.75f, 0);
        RespawnNotice.SetActive(false);
        DeathUI.SetActive(false);
        PauseUI.SetActive(false);
        Respawned = false;
        Life = 100;
        Score = 0;
        Level = 1;

        Restarted = true;
    }

    public void Continue()
    {
        GameOverChange(false);

        if (!Ads && !Respawned)
        {
            DeathUI.SetActive(false);
            PauseUI.SetActive(false);

            Life = 100;

            CallInvincibility = true;
            GameOver = false;
            Respawned = true;
        }
        else if (Ads && !Respawned)
        {
#if UNITY_ANDROID
            Admob.RewardBasedVideo.Show();
            Respawned = true;
#endif
#if UNITY_STANDALONE
            DeathUI.SetActive(false);
            PauseUI.SetActive(false);

            Life = 100;

            CallInvincibility = true;
            GameOver = false;
            Respawned = true;
#endif
        }
        else if (Respawned)
        {
            RespawnNotice.SetActive(true);
            GameOverChange(true);
        }
    }

    public void InvertRotation()
    {
        // Inverter bola.
        transform.Rotate(Vector3.left * 180);
        // Inverter botão de Inversão.
        InvertButton.transform.Rotate(Vector3.forward * 180);
    }

    public void ToUp(float UpForce)
    {
        RigidbodyComponent.AddForce(Vector3.up * (UpForce * 100) * Time.deltaTime);
    }

    public void ToDown(float DownForce)
    {
        RigidbodyComponent.AddForce(Vector3.down * (DownForce * 100) * Time.deltaTime);
    }

    public void ToDanger()
    {
        // RigidbodyComponent.AddForce(Vector3.up * (DangerForce * 100) * Time.deltaTime);
    }

    // Funções para mover a bola para esquerda ou para direita, mas pensando bem... acho que eu posso utilizar apenas uma função e mudar apenas o número para negativo ou não. Bom, depois eu testo isso...
    public void TurnLeft(float Input)
    {
        RigidbodyComponent.AddForce(Vector3.left * (TurnForce * (Input * 50)) * Time.deltaTime);
    }
    public void TurnRight(float Input)
    {
        RigidbodyComponent.AddForce(Vector3.right * (TurnForce * (Input * 50)) * Time.deltaTime);
    }

    // Gerar número aleátorio e diferente dos anteriores inseridos na lista.
    public int TileNumberRandom(int Min, int Max)
    {
        if (Mathf.Abs(Max - Min) > TilesOld.Count)
        {
            while (true)
            {
                int RandomNumber = Random.Range(Min, Max);
                if (!TilesOld.Contains(RandomNumber))
                {
                    TilesOld.Add(RandomNumber);
                    return RandomNumber;
                }
            }
        }
        else
        {
            TileLast = TilesOld[TilesOld.Count - 1];

            TilesOld.Clear();

            int RandomNumber = Random.Range(Min, Max);

            if (RandomNumber == TileLast)
            {
                RandomNumber -= 1;
            }

            return RandomNumber;
        }
    }

    public void RandomSkyColor()
    {
        if (LevelSkyColorTopOld.Count < Level && LevelSkyColorBottomOld.Count < Level)
        {

            SkyColorTop = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 1f);
            SkyColorBottom = new Color(1.0f - SkyColorTop.r, 1.0f - SkyColorTop.g, 1.0f - SkyColorTop.b, 1f);

            if (LevelSkyColorTopOld.Contains(SkyColorTop) && LevelSkyColorBottomOld.Contains(SkyColorBottom))
            {
                return;
            }

            SkyMaterial.SetColor("_Color", (Color.Lerp(SkyMaterial.GetColor("_Color"), SkyColorTop, Mathf.PingPong(Time.time, SkyLerpSpeed))));
            SkyMaterial.SetColor("_Color1", (Color.Lerp(SkyMaterial.GetColor("_Color1"), SkyColorBottom, Mathf.PingPong(Time.time, SkyLerpSpeed))));

            LevelSkyColorTopOld.Add(SkyColorTop);
            LevelSkyColorBottomOld.Add(SkyColorBottom);

            // Debug.Log("Céu - Adicionado.");
        }
        else
        {
            SkyMaterial.SetColor("_Color", (Color.Lerp(SkyMaterial.GetColor("_Color"), LevelSkyColorTopOld[Level - 1], Mathf.PingPong(Time.time, SkyLerpSpeed))));
            SkyMaterial.SetColor("_Color1", (Color.Lerp(SkyMaterial.GetColor("_Color1"), LevelSkyColorBottomOld[Level - 1], Mathf.PingPong(Time.time, SkyLerpSpeed))));

            // Debug.Log("Céu - Selecionado.");
        }

        // Debug.Log("Céu - Quantidade Top: " + LevelSkyColorTopOld.Count + " Quantidade Bottom: " + LevelSkyColorBottomOld.Count + " Nível: " + Level + ".");
    }

    public static void SpawnPopUp(string Text)
    {
        GameObject PopUp = Instantiate(Resources.Load("PopUp") as GameObject);

        Vector2 ScreenPosition = Camera.main.WorldToScreenPoint(new Vector2(PopUp.transform.localPosition.x + Random.Range(-2.75f, 2.75f), PopUp.transform.localPosition.y + Random.Range(-0.25f, 1.25f)));

        PopUp.transform.name = "PopUp";
        PopUp.transform.parent = GameObject.Find("PopUps").transform;
        PopUp.transform.position = ScreenPosition;

        PopUp.GetComponent<Text>().text = Text;

        Destroy(PopUp, 1.5f);
    }

    // Sistema um pouco procedural, mas que estou com preguiça de comentar. :)
    public void SpawnTile(float StartPosition, float NewPosition, string Parent)
    {
        int TilesTotal = Resources.LoadAll("Tiles/").Length;
        int TileSpawn = TileNumberRandom(1, TilesTotal);

        string TilesName = "Tile (" + TileSpawn + ")";

        Transform TilesParent = GameObject.Find(Parent).transform;

        GameObject Tile = Resources.Load("Tiles/" + TilesName) as GameObject;
        GameObject TileInstance = Instantiate(Tile, TilesParent);

        float RandomX = Random.Range(-1.5f, 1.5f);

        Vector3 TilesPosition = new Vector3(RandomX, StartPosition + NewPosition, 0f);
        TileInstance.transform.position = TilesPosition;
        TileInstance.transform.name = TilesName;

        GameObject Spark = Resources.Load("Spark") as GameObject;

        foreach (Transform Go in TileInstance.transform)
        {
            if (Go.tag == "Black")
            {
                Instantiate(Spark, Go.transform);
            }
        }

        // Debug.Log("Plataformas - Total: " + TilesTotal + " Name: " + TilesName + " Parent: " + TilesParent.name + ".");

        // Ferrar com a vida dos jogadores. :P
        if (TileInstance.GetComponentsInChildren<MovePlatform>() != null)
        {
            MovePlatform[] Movement = TileInstance.GetComponentsInChildren<MovePlatform>();

            foreach (MovePlatform Go in Movement)
            {
                Go.Speed = Random.Range(0.5f, 2.5f);
                Go.Move = TrueFalse();
                Go.StartInvert = TrueFalse();

                int TotalMove = 0;
                int TotalInvert = 0;

                if (Go.Move)
                {
                    TotalMove++;
                }

                if (Go.StartInvert)
                {
                    TotalInvert++;
                }

                // Debug.Log("Plataforma Dinâmicas - Quantidade: " + Movement.Length + " Move: " + TotalMove + " Invert: " + TotalInvert + " Speed: " + Go.Speed + ".");
            }
        }
    }

    public bool TrueFalse()
    {
        int Result = Random.Range(0, 2);

        if (Result == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public IEnumerator Invincibility()
    {
        LifeBar.color = Color.gray;
        Invincible = true;

        // Debug.Log("Invencibilidade - Ativada.");

        yield return new WaitForSecondsRealtime(5f);

        Invincible = false;
        LifeBar.color = Color.white;

        // Debug.Log("Invencibilidade - Desativada.");
    }
}