using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private bool useNewInputSystem = true;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode cancelKey = KeyCode.Escape;

    [Header("Camera Settings")]
    [SerializeField] private bool useCinemachine = true;
    [SerializeField] private Cinemachine.CinemachineBrain cinemachineBrain;

    [Header("Debug")]
    [SerializeField] private bool enableDebugMode = true;

    private IInputService inputService;

    private void Awake()
    {
        if (FindObjectsOfType<GameBootstrap>().Length >1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        InitializeServices();
    }

    private void InitializeServices()
    {
        if (enableDebugMode) Debug.Log("===Initializing Game Bootstrap===");

        RegisterInputService();
        RegisterCameraService();
        RegisterAudioService();
        RegisterHighlighterService();
        RegisterUIService();
        //RegisterCameraService();
        RegisterCameraProvider();

        if (enableDebugMode) PrintStatus();
    }

    private void RegisterInputService()
    {
        if (useNewInputSystem)
        {
            var inputSysServ = new InputSystemService();
            this.inputService = inputSysServ;
            ServiceLocator.Register<IInputService>(inputSysServ);
            if (enableDebugMode) Debug.Log("Input System Service Registered");
        }
        if (inputService == null)
        {
            inputService = new InputService(interactKey, cancelKey);
            ServiceLocator.Register<IInputService>(inputService);
            if (enableDebugMode) Debug.Log(" old Input Service Registered");
        }
    }
    private void RegisterCameraProvider()
    {
        ICameraProvider cameraProvider = null;

        if (useCinemachine)
        {
            var brain = cinemachineBrain;
            if (cinemachineBrain == null)
            {
                brain = FindObjectOfType<Cinemachine.CinemachineBrain>();
            }
            if (brain != null)
            {
                cameraProvider = new CinemachineCameraProvider(brain);
                if (enableDebugMode) Debug.Log("Cinemachine Camera Provider Registered");
            }
        }
        if (cameraProvider == null)
        {
            cameraProvider = new StandardCameraProvider(Camera.main);
            if (enableDebugMode) Debug.Log("standard Camera Provider Registered");
        }
        ServiceLocator.Register<ICameraProvider>(cameraProvider);
    }
    private void RegisterAudioService()
    {
        var audioService = new AudioService();
        ServiceLocator.Register<IAudioService>(audioService);
        if (enableDebugMode) Debug.Log("Audio Service Registered");
    }
    private void RegisterHighlighterService()
    {
        var highlighter = new EmissionHighlighter();
        ServiceLocator.Register<IHighlighter>(highlighter);
        if (enableDebugMode) Debug.Log("Highlighter Service Registered");
    }
    private void RegisterUIService()
    {
        var uiManager = FindObjectOfType<InteractionUIManager>();
        IUIService uiService = uiManager != null ? new UIServiceAdapter(uiManager) : new DummyUIService();
        ServiceLocator.Register<IUIService>(uiService);
        if (enableDebugMode) Debug.Log("UI Service Registered");
    }
    private void RegisterCameraService()
    {
        var cameraShake = FindObjectOfType<CameraShakeManager>();
        if (cameraShake == null)
        {
            var obj = new GameObject("CameraService");
            obj.transform.SetParent(transform);
            cameraShake = obj.AddComponent<CameraShakeManager>();
        }
        ServiceLocator.Register<ICameraService>(cameraShake);
        if (enableDebugMode) Debug.Log("Camera Service Registered");
    }

    private void PrintStatus()
    {
        Debug.Log("===Service Locator Status===");
        Debug.Log($"Input Service: {(useNewInputSystem? "new INput System" : "Legacy input")}");
        Debug.Log($"Camera Provider: {(useCinemachine? "Cinemachine":"Standard camera")}");
        Debug.Log("================================");
    }

    private void LateUpdate()
    {
        if (inputService is InputSystemService inputsysServ)
        {
            inputsysServ.LateUpdate();
        }
    }

    private void OnDestroy()
    {
        if (ServiceLocator.TryGet<IInputService>(out IInputService input))
        {
            if (input is InputSystemService inputSystem)
            {
                inputSystem.Dispose();
            }
        }
        ServiceLocator.Clear();
    }
}