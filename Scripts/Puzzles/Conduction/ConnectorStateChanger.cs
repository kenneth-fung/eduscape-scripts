using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConnectorStateChanger : MonoBehaviour {

    private MeshRenderer mesh;
    private DisplayScreen screen;

    public ShipEngineConnectorSegment.State CurrState { get; private set; }

    [SerializeField] private int lightIndicatorMaterialIndex;

    [Space(10)]

    [SerializeField] private ShipEngineConnectorSegment[] segments;
    [SerializeField] private SegmentChangeGlow[] segmentGlows;
    [SerializeField] private AudioSource stateAccepterAudioSource;
    [SerializeField] private float changeDuration;

    [Space(10)]

    [SerializeField] private ConnectorStateData[] stateData;
    [SerializeField] private GameObject metalIndicator;
    [SerializeField] private GameObject waterIndicator;
    [SerializeField] private AudioSource stateChangeAudioSource;

    private Dictionary<ShipEngineConnectorSegment.State, (ConnectorStateData stateDataEntry, GameObject indicator)> stateDataDict =
        new Dictionary<ShipEngineConnectorSegment.State, (ConnectorStateData stateDataEntry, GameObject indicator)>();

    public bool IsChanging { get; private set; }

    [Space(10)]

    [SerializeField] private HeatGenerator connectedGenerator;

    [Space(10)]

    [SerializeField] private UnityEvent changeStateOccursEvent;

    void Awake() {
        mesh = GetComponent<MeshRenderer>();
        screen = GetComponentInChildren<DisplayScreen>();

        stateDataDict[ShipEngineConnectorSegment.State.METAL] =
            (System.Array.Find(stateData, stateDataEntry => stateDataEntry.State == ShipEngineConnectorSegment.State.METAL),
            metalIndicator);
        stateDataDict[ShipEngineConnectorSegment.State.WATER] =
            (System.Array.Find(stateData, stateDataEntry => stateDataEntry.State == ShipEngineConnectorSegment.State.WATER),
            waterIndicator);
    }

    private void Start() {
        CurrState = ShipEngineConnectorSegment.State.PLASTIC;
        screen.SetText($"{System.Enum.GetName(typeof(ShipEngineConnectorSegment.State), CurrState)}");
        if (segments.Length != segmentGlows.Length) {
            Debug.LogWarning($"Number of segment ({segments.Length}) does not match number of segment glows ({segmentGlows.Length}).");
        }
    }

    public void HitByElement(Element element) {
        if (connectedGenerator.IsHeating) {
            return;
        }

        if (element.ElementType == Element.Type.METAL || element.ElementType == Element.Type.WATER) {
            stateAccepterAudioSource.Play();
            switch (element.ElementType) {
                case Element.Type.METAL:
                    ChangeLightIndicator(ShipEngineConnectorSegment.State.METAL);
                    ChangeSegmentStatesOverTime(ShipEngineConnectorSegment.State.METAL);
                    break;
                case Element.Type.WATER:
                    ChangeLightIndicator(ShipEngineConnectorSegment.State.WATER);
                    ChangeSegmentStatesOverTime(ShipEngineConnectorSegment.State.WATER);
                    break;
                default:
                    throw new System.ArgumentException($"No connector segment state corresponds to element of type {element.ElementType}.");
            }
        }
    }

    private void ChangeLightIndicator(ShipEngineConnectorSegment.State newState) {
        Material[] materials = mesh.materials;
        materials[lightIndicatorMaterialIndex] = stateDataDict[newState].stateDataEntry.GlowMaterial;
        mesh.materials = materials;
    }

    private void ChangeSegmentStatesOverTime(ShipEngineConnectorSegment.State newState) {
        if (IsChanging) {
            return;
        }

        IsChanging = true;

        foreach ((ConnectorStateData stateDataEntry, GameObject indicator) in stateDataDict.Values) {
            indicator.SetActive(stateDataEntry.State == newState);
        }

        bool hasChangeOccurred = false;
        for (int i = 0; i < segments.Length; i++) {
            ShipEngineConnectorSegment segment = segments[i];
            SegmentChangeGlow glow = segmentGlows[i];
            glow.GlowMaterial = stateDataDict[newState].stateDataEntry.GlowMaterial;
            glow.GlowColor = stateDataDict[newState].stateDataEntry.GlowColor;
            StartCoroutine(glow.BrieflyGlow(
                changeDuration,
                () => {
                    if (!hasChangeOccurred) {
                        hasChangeOccurred = true;
                        stateChangeAudioSource.PlayOneShot(stateDataDict[newState].stateDataEntry.ChangeSound);
                        CurrState = newState;
                        screen.SetText($"{System.Enum.GetName(typeof(ShipEngineConnectorSegment.State), CurrState)}", true, true);
                        changeStateOccursEvent.Invoke();
                    }

                    segment.ChangeState(newState, stateDataDict[newState].stateDataEntry.SegmentMaterial);
                },
                () => IsChanging = false));
        }
    }
}
