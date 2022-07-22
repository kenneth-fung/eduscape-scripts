using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShipEngineConnectorSegment : MonoBehaviour {

    /// <summary>
    /// The percentage of energy lost after heating a segment of the given state.
    /// </summary>
    private static readonly Dictionary<State, float> stateEnergyLostDict = new Dictionary<State, float>() {
        [State.PLASTIC] = 0.5f,
        [State.METAL] = 0f,
        [State.WATER] = 2f
    };

    public enum State { PLASTIC, METAL, WATER }

    private State state = State.PLASTIC;

    private MeshRenderer mesh;

    [SerializeField] private HeatedSegment heatedSegment;

    public bool IsHeated { get; private set; }

    [SerializeField] private ShipEngineConnectorSegment[] connectedSegments;
    [SerializeField] private ShipEngine engine;

    void Awake() {
        mesh = GetComponent<MeshRenderer>();
    }

    public void ChangeState(State newState, Material newMaterial) {
        state = newState;
        mesh.material = newMaterial;
    }

    public void Heat(float currEnergy, float totalEnergy, UnityAction failureCallback) {
        if (IsHeated) {
            return;
        }

        StartCoroutine(StartHeating(currEnergy, totalEnergy, failureCallback));
    }

    private IEnumerator StartHeating(float currEnergy, float totalEnergy, UnityAction failureCallback) {
        // only heat the entire segment if we have not run out of energy
        currEnergy -= stateEnergyLostDict[state] * totalEnergy;
        bool isEnergySufficient = currEnergy >= 0f;
        StartCoroutine(heatedSegment.Heat(isEnergySufficient ? 0.99f : 0.5f));

        yield return new WaitUntil(() => heatedSegment.CurrState == HeatedSegment.State.HEATED);

        IsHeated = true;

        if (!isEnergySufficient) {
            StartCoroutine(StopHeating(failureCallback));
            yield break;
        }

        // we have enough energy to continue heating the adjacent segments
        foreach (ShipEngineConnectorSegment segment in connectedSegments) {
            segment.Heat(currEnergy, totalEnergy, failureCallback);
        }

        if (engine != null) {
            engine.Heat();
        }
    }

    private IEnumerator StopHeating(UnityAction failureCallback) {
        // heating has stopped; wait a moment before starting cooling
        yield return new WaitForSeconds(2f);

        Cool();
        failureCallback();
    }

    public void Cool() {
        if (!IsHeated) {
            return;
        }

        StartCoroutine(StartCooling());
    }

    private IEnumerator StartCooling() {
        StartCoroutine(heatedSegment.Cool());

        yield return new WaitUntil(() => heatedSegment.CurrState == HeatedSegment.State.COOLED);

        IsHeated = false;

        foreach (ShipEngineConnectorSegment segment in connectedSegments) {
            segment.Cool();
        }
    }
}
