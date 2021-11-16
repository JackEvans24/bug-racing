using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SelectionMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject defaultButton;
    [SerializeField] private SkinnedMeshRenderer mesh;

    [Header("Variables")]
    [SerializeField] private Color[] colors;
    [SerializeField] private PlayerBodyModel[] bodies;

    private int currentPrimaryMaterialIndex = 0;
    private int currentSecondaryMaterialIndex = 1;
    private int currentBodyIndex = 0;

    private void Start()
    {
        this.SetVariables();
    }

    private void OnEnable()
    {
        StartCoroutine(this.SetSelectedButton());

        this.SetVariables();
    }

    private IEnumerator SetSelectedButton()
    {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(this.defaultButton);
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
        var playerBody = this.bodies[this.currentBodyIndex];
        this.mesh.sharedMesh = playerBody.Body;

        this.mesh.materials[0].color = this.colors[this.currentPrimaryMaterialIndex];

        if (playerBody.HasAccent)
            this.mesh.materials[1].color = this.colors[this.currentSecondaryMaterialIndex];
    }

    public void StartGame()
    {
        var player = new PlayerSelection()
        {
            BodyModel = this.bodies[this.currentBodyIndex].Body,
            MainColor = this.colors[this.currentPrimaryMaterialIndex],
            AccentColor = this.colors[this.currentSecondaryMaterialIndex]
        };

        GameController.SetPlayersAndPlay(new[] { player }, Scenes.OakHighway);
    }

    public void BackToMenu()
    {
        this.mainMenu.SetActive(true);
        this.gameObject.SetActive(false);
    }
}
