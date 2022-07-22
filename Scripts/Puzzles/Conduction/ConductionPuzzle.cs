using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConductionPuzzle : MonoBehaviour {

    [SerializeField] private GameObject engineConnectorLeft;

    private HeatGenerator leftGenerator;
    private ConnectorStateChanger leftStateChanger;
    private ShipEngineConnectorSegment[] leftSegments;

    public bool IsLeftGeneratorHeating { get => leftGenerator.IsHeating; }
    public ShipEngineConnectorSegment.State StateChangerLeftCurrState { get => leftStateChanger.CurrState; }
    public bool AreLeftSegmentsHeated { get => System.Array.TrueForAll(leftSegments, segment => segment.IsHeated); }

    [SerializeField] private GameObject engineConnectorRight;

    private HeatGenerator rightGenerator;
    private ConnectorStateChanger rightStateChanger;
    private ShipEngineConnectorSegment[] rightSegments;

    public bool IsRightGeneratorHeating { get => rightGenerator.IsHeating; }
    public ShipEngineConnectorSegment.State StateChangerRightCurrState { get => rightStateChanger.CurrState; }
    public bool AreRightSegmentsHeated { get => System.Array.TrueForAll(rightSegments, segment => segment.IsHeated); }

    private static ConductionPuzzle _instance;
    public static ConductionPuzzle Instance { get => _instance; }

    void Awake() {
        // singleton
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
    }

    void Start() {
        leftGenerator = engineConnectorLeft.GetComponentInChildren<HeatGenerator>();
        leftStateChanger = engineConnectorLeft.GetComponentInChildren<ConnectorStateChanger>();
        leftSegments = engineConnectorLeft.GetComponentsInChildren<ShipEngineConnectorSegment>();

        rightGenerator = engineConnectorRight.GetComponentInChildren<HeatGenerator>();
        rightStateChanger = engineConnectorRight.GetComponentInChildren<ConnectorStateChanger>();
        rightSegments = engineConnectorRight.GetComponentsInChildren<ShipEngineConnectorSegment>();
    }

    public bool WillShipLaunchAttemptSucced() {
        print($"{IsLeftGeneratorHeating}; {StateChangerLeftCurrState}; {IsRightGeneratorHeating}; {StateChangerRightCurrState}");
        return IsLeftGeneratorHeating
            && StateChangerLeftCurrState == ShipEngineConnectorSegment.State.METAL
            && IsRightGeneratorHeating
            && StateChangerRightCurrState == ShipEngineConnectorSegment.State.METAL;
    }
}
