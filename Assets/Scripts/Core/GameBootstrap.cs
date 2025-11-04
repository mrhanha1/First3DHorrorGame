using UnityEngine;

public class GameBootstrap : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private bool useNewInputSystem = true;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private KeyCode cancelKey = KeyCode.Escape;

    [Header("Camera Settings")]
    [SerializeField] private bool useCinemachine = true;

    [Header("Debug")]
    [SerializeField] private bool enableDebugMode = true;

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

        RegisterInputSevice();
        RegisterCameraService();
        RegisterAudioService();
        RegisterHighlighterService();
        RegisterUIService();
        RegisterCameraService();

        if (enableDebugMode) PrintStatus();
    }

    private void RegisterInputSevice()
    {
        IInputService inputService = null;
        if (useNewInputSystem)
        {
            inputService = new InputSystemService();
            if (enableDebugMode) Debug.Log("Input System Service Registered");
        }
        if (inputService == null)
        {
            inputService = new InputService(interactKey, cancelKey);
            if (enableDebugMode) Debug.Log(" old Input Service Registered");
        }
        ServiceLocator.Register<IInputService>(inputService);
    }

    private void RegisterCameraProvider()
    {
        ICameraProvider cameraProvider = null;

        if (useCinemachine)
        {
            var brain = FindObjectOfType<Cinemachine.CinemachineBrain>();
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