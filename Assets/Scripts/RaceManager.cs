using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    private static RaceManager _instance;

    private List<CarMovement> racers;
    private Dictionary<CarMovement, int> racePositions = new Dictionary<CarMovement, int>();
    private List<CarMovement> finishedRacers = new List<CarMovement>();

    private List<RaceTextController> raceTextControllers;

    [Header("References")]
    [SerializeField] private GameObject Camera;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject AI;
    [SerializeField] private Transform startLine;

    [Header("Race Start")]
    [SerializeField] private int totalRacers = 8;
    [SerializeField] private Vector3 startPositioning;

    [Header("Race position")]
    [SerializeField] private int positionUpdateFrequency = 5;

    private int totalRacerCount, currentFrameCount;
    private bool setupComplete;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        this.RaceSetup(1, this.totalRacers);
    }

    private void Update()
    {
        if (!this.setupComplete)
            return;

        this.currentFrameCount++;
        if (this.currentFrameCount >= this.positionUpdateFrequency)
        {
            this.currentFrameCount = 0;
            this.UpdatePositions();
        }
    }

    public void RaceSetup(int playerCount = 1, int totalRacerCount = 8)
    {
        this.totalRacerCount = totalRacerCount;
        this.GenerateRacers(playerCount);

        this.setupComplete = true;
        StartCoroutine(this.StartRace());
    }

    private void GenerateRacers(int playerCount)
    {
        this.racers = new List<CarMovement>();
        this.raceTextControllers = new List<RaceTextController>();

        var aiRacerCount = this.totalRacerCount - playerCount;
        for (int i = 0; i < this.totalRacerCount; i++)
        {
            var position = this.GetPosition(i);

            var isPlayer = i >= aiRacerCount;
            var obj = isPlayer ? this.Player : this.AI;

            var racer = Instantiate(obj, position, this.startLine.rotation);
            var carMovement = racer.GetComponent<CarMovement>();

            if (isPlayer)
            {
                var camera = Instantiate(this.Camera);

                var cameraFollow = camera.GetComponent<CameraFollow>();
                cameraFollow.target = racer.transform;

                var hud = camera.GetComponent<HUDUpdater>();
                hud.car = carMovement;
                hud.itemSystem = racer.GetComponent<CarItemSystem>();

                var textController = camera.GetComponentInChildren<RaceTextController>();
                carMovement.TextController = textController;
                this.raceTextControllers.Add(textController);
            }

            this.racers.Add(carMovement);
        }
    }

    private Vector3 GetPosition(int i)
    {
        var xCoefficient = i % 2f == 0f ? -1 : 1;
        var xOffset = this.startLine.right * this.startPositioning.x * xCoefficient;

        var yCoefficient = (-1 * (i / 2)) - 1;
        var yOffset = this.startLine.forward * this.startPositioning.y * yCoefficient;

        var zOffset = this.startLine.up * this.startPositioning.z;

        return this.startLine.position + xOffset + yOffset + zOffset;
    }

    private IEnumerator StartRace()
    {
        for (int i = 0; i < this.raceTextControllers.Count; i++)
        {
            var controller = this.raceTextControllers[i];
            if (i + 1 >= this.raceTextControllers.Count)
                yield return controller.ShowRaceReady();
        }

        foreach (var racer in this.racers)
            racer.CanMove = true;

        for (int i = 0; i < this.raceTextControllers.Count; i++)
        {
            var controller = this.raceTextControllers[i];
            if (i + 1 >= this.raceTextControllers.Count)
                yield return controller.ShowRaceStart();
        }
    }

    private IEnumerator EndRace()
    {
        Debug.Log($"Race complete!");
        yield return new WaitForEndOfFrame();
    }

    private void UpdatePositions()
    {
        var orderedRacers = racers
            .OrderByDescending(r => r.LapsCompleted)
            .ThenByDescending(r => r.NextCheckpoint.Index == 0)
            .ThenByDescending(r => r.NextCheckpoint.Index)
            .ThenBy(r => this.GetDistanceToCheckpoint(r, r.NextCheckpoint))
            .ToList();

        this.racePositions.Clear();

        for (int i = 0; i < orderedRacers.Count; i++)
            this.racePositions[orderedRacers[i]] = i + 1;
    }

    public static void LogFinish(CarMovement car, out int finalPosition) => _instance.LogFinishLocal(car, out finalPosition);

    public void LogFinishLocal(CarMovement car, out int finalPosition)
    {
        this.finishedRacers.Add(car);
        Debug.Log($"{car.name} finished");

        finalPosition = this.finishedRacers.IndexOf(car) + 1;

        if (this.finishedRacers.Count == this.racers.Count)
            StartCoroutine(this.EndRace());
    }

    public static bool TryGetPosition(CarMovement car, out int position) => _instance.TryGetPositionLocal(car, out position);

    private bool TryGetPositionLocal(CarMovement car, out int position) => this.racePositions.TryGetValue(car, out position);

    public static bool TryGetRacer(int position, out CarMovement car) => _instance.TryGetRacerLocal(position, out car);

    private bool TryGetRacerLocal(int position, out CarMovement car)
    {
        car = default;
        if (this.racePositions.Count < this.totalRacerCount)
            return false;

        var result = false;
        foreach (var kvp in this.racePositions)
        {
            if (kvp.Value == position)
            {
                car = kvp.Key;
                result = true;
                break;
            }
        }

        return result;
    }

    private float GetDistanceToCheckpoint(CarMovement car, Checkpoint checkpoint) =>
        Mathf.Min(
            Vector3.Distance(car.transform.position, checkpoint.MinimumInnerBoundary),
            Vector3.Distance(car.transform.position, checkpoint.MaximumInnerBoundary)
        );

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;

        for (int i = 0; i < 8; i++)
        {
            var position = this.GetPosition(i);
            Gizmos.DrawSphere(position, 2);
        }
    }
}
