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

    [Header("References")]
    [SerializeField] private GameObject Camera;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject AI;
    [SerializeField] private Transform startLine;

    [Header("Race Start")]
    [SerializeField] private int totalRacers = 8;
    [SerializeField] private int raceStartTime = 5;
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

        var aiRacerCount = this.totalRacerCount - playerCount;
        for (int i = 0; i < this.totalRacerCount; i++)
        {
            var position = this.GetPosition(i);

            var isPlayer = i >= aiRacerCount;
            var obj = isPlayer ? this.Player : this.AI;

            var racer = Instantiate(obj, position, this.startLine.rotation);

            if (isPlayer)
            {
                var cameraFollow = Instantiate(this.Camera).GetComponent<CameraFollow>();
                cameraFollow.target = racer.transform;
            }

            this.racers.Add(racer.GetComponent<CarMovement>());
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
        yield return new WaitForSeconds(this.raceStartTime);

        foreach (var racer in this.racers)
            racer.CanMove = true;
    }

    private IEnumerator EndRace()
    {
        yield return new WaitForEndOfFrame();
    }

    private void UpdatePositions()
    {
        var orderedRacers = racers
            .OrderByDescending(r => r.LapsCompleted)
            .ThenByDescending(r => r.NextCheckpoint.Index)
            .ThenBy(r => this.GetDistanceToCheckpoint(r, r.NextCheckpoint))
            .ToList();

        this.racePositions.Clear();

        for (int i = 0; i < orderedRacers.Count; i++)
            this.racePositions[orderedRacers[i]] = i + 1;
    }

    public static void LogFinish(CarMovement car) => _instance.LogFinishLocal(car);

    public void LogFinishLocal(CarMovement car)
    {
        this.finishedRacers.Add(car);

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
