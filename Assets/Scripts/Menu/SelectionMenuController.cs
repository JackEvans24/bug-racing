using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class SelectionMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerInputManager inputManager;
    [SerializeField] private PlayerInput input;
    [SerializeField] private EventSystem eventSystem;

    [Header("Canvas References")]
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject readyCanvas;
    [SerializeField] private GameObject readyButton;
    [SerializeField] private GameObject unreadyButton;
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject buttonPadding;
    public SkinnedMeshRenderer Mesh;
    private MainMenuController mainMenu;

    [Header("Variables")]
    [SerializeField] private Color[] colors;
    [SerializeField] private PlayerBodyModel[] bodies;

    private int currentPrimaryMaterialIndex = 0;
    private int currentSecondaryMaterialIndex = 1;
    private int currentBodyIndex = 0;

    private void Awake()
    {
        this.mainMenu = FindObjectOfType<MainMenuController>();
    }

    private void Start()
    {
        this.currentSecondaryMaterialIndex = this.colors.Length - 1;
        this.SetVariables();
    }

    public void SetBackButtonEnabled(bool enabled)
    {
        this.backButton.SetActive(enabled);
        this.buttonPadding.SetActive(enabled);
    }

    private void OnEnable()
    {
        this.SetVariables();
    }

    public void NextMaterial()
    {
        this.currentPrimaryMaterialIndex++;
        if (this.currentPrimaryMaterialIndex >= this.colors.Length)
            this.currentPrimaryMaterialIndex = 0;

        this.SetVariables();
    }

    public void PreviousMaterial()
    {
        this.currentPrimaryMaterialIndex--;
        if (this.currentPrimaryMaterialIndex < 0)
            this.currentPrimaryMaterialIndex = this.colors.Length - 1;

        this.SetVariables();
    }

    public void NextSecondaryMaterial()
    {
        this.currentSecondaryMaterialIndex++;
        if (this.currentSecondaryMaterialIndex >= this.colors.Length)
            this.currentSecondaryMaterialIndex = 0;

        this.SetVariables();
    }

    public void PreviousSecondaryMaterial()
    {
        this.currentSecondaryMaterialIndex--;
        if (this.currentSecondaryMaterialIndex < 0)
            this.currentSecondaryMaterialIndex = this.colors.Length - 1;

        this.SetVariables();
    }

    public void NextBody()
    {
        this.currentBodyIndex++;
        if (this.currentBodyIndex >= this.bodies.Length)
            this.currentBodyIndex = 0;

        this.SetVariables();
    }

    public void PreviousBody()
    {
        this.currentBodyIndex--;
        if (this.currentBodyIndex < 0)
            this.currentBodyIndex = this.bodies.Length - 1;

        this.SetVariables();
    }

    private void SetVariables()
    {
        if (this.Mesh == null)
            return;

        var playerBody = this.bodies[this.currentBodyIndex];
        this.Mesh.sharedMesh = playerBody.Body;

        this.Mesh.materials[0].color = this.colors[this.currentPrimaryMaterialIndex];

        if (playerBody.HasAccent)
            this.Mesh.materials[1].color = this.colors[this.currentSecondaryMaterialIndex];
    }

    public void Ready()
    {
        this.mainMenu.Ready(GetPlayer());

        this.menuCanvas.SetActive(false);
        this.readyCanvas.SetActive(true);

        this.eventSystem.SetSelectedGameObject(null);
        this.eventSystem.SetSelectedGameObject(this.unreadyButton);
    }

    public void UnReady()
    {
        this.mainMenu.Unready(this.input.devices[0]);

        this.readyCanvas.SetActive(false);
        this.menuCanvas.SetActive(true);

        this.eventSystem.SetSelectedGameObject(null);
        this.eventSystem.SetSelectedGameObject(this.readyButton);
    }

    private PlayerSelection GetPlayer() => new PlayerSelection()
        {
            BodyModel = this.bodies[this.currentBodyIndex].Body,
            MainColor = this.colors[this.currentPrimaryMaterialIndex],
            AccentColor = this.colors[this.currentSecondaryMaterialIndex],
            Device = this.input.devices[0]
        };

    public void BackToMenu()
    {
        this.mainMenu.Back();
    }
}
