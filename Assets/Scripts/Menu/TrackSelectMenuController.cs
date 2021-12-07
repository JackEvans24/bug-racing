using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TrackSelectMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas menuCanvas;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private MainMenuController menu;
    [SerializeField] private Track[] tracks;

    [Header("UI Elements")]
    [SerializeField] private GameObject nextButton;
    [SerializeField] private Image trackImage;
    [SerializeField] private TMP_Text trackText;

    private int selectedTrackIndex;
    private Track selectedTrack;

    protected void Start()
    {
        this.UpdateSelectedTrack();
    }

    public void Show()
    {
        this.menuCanvas.enabled = true;

        this.eventSystem.gameObject.SetActive(true);
        this.eventSystem.SetSelectedGameObject(null);
        this.eventSystem.SetSelectedGameObject(this.nextButton);
    }

    public void NextTrack()
    {
        this.selectedTrackIndex++;
        if (this.selectedTrackIndex >= this.tracks.Length)
            this.selectedTrackIndex = 0;

        this.UpdateSelectedTrack();
    }

    public void PreviousTrack()
    {
        this.selectedTrackIndex--;
        if (this.selectedTrackIndex < 0)
            this.selectedTrackIndex = this.tracks.Length - 1;

        this.UpdateSelectedTrack();
    }

    private void UpdateSelectedTrack()
    {
        this.selectedTrack = this.tracks[this.selectedTrackIndex];

        this.trackImage.sprite = this.selectedTrack.Image;
        this.trackText.text = this.selectedTrack.Name;
    }

    public void StartRace() => this.menu.StartRace(this.selectedTrack.Scene);

    public void Back()
    {
        this.menuCanvas.enabled = false;
        this.eventSystem.gameObject.SetActive(false);

        this.menu.BackToSelectionMenu();
    }
}
