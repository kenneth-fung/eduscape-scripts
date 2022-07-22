using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ChangeTemperatureEvent : UnityEvent<float, float> {}

public class MoleculesContainer : MonoBehaviour {

    [SerializeField] private TemperatureDisplay tempDisplay;
    [SerializeField] private float roomTemperature = 36.0f;
    [SerializeField] private float raiseTempAmount = 5.0f;
    [SerializeField] private float maxTemperature = 65f;
    private float temperature;

    [SerializeField] private MoleculesSfx molSfx;
    [SerializeField] private Molecule[] molecules;

    [Space(10)]

    [SerializeField] private ChangeTemperatureEvent raiseTemperatureEvent;
    [SerializeField] private UnityEvent exceedMaxTemperatureEvent;
    [SerializeField] private UnityEvent moleculeEscapeEvent;

    private int lastElementHitId;

    // Start is called before the first frame update
    void Start() {
        temperature = roomTemperature;
        tempDisplay.UpdateDisplay(temperature, temperature > maxTemperature);
        // StartCoroutine(RaiseTempOverTime());
    }

    private IEnumerator RaiseTempOverTime() {
        while (true) {
            yield return new WaitForSeconds(1);
            RaiseTemperature();
        }
    }

    public void HitByElementProjectile(Element element) {
        if (element.ElementType == Element.Type.FIRE && element.gameObject.GetInstanceID() != lastElementHitId) {
            RaiseTemperature();
            lastElementHitId = element.gameObject.GetInstanceID();
        }
    }

    public void RaiseTemperature() {
        if (temperature >= maxTemperature) {
            return;
        }

        float newTemp = temperature + raiseTempAmount;

        foreach (Molecule mol in molecules) {
            mol.ChangeSpeed(temperature, newTemp);
        }

        molSfx.DecreaseDelayBetweenBounceSounds();

        raiseTemperatureEvent.Invoke(temperature, newTemp);
        temperature = newTemp;
        if (temperature > maxTemperature) {
            exceedMaxTemperatureEvent.Invoke();
        }

        tempDisplay.UpdateDisplay(temperature, temperature > maxTemperature);
    }

    public void Destroy() {
        molSfx.StopAllSources();

        foreach (Molecule mol in molecules) {
            Destroy(mol.gameObject);
        }

        Destroy(tempDisplay.gameObject);
        Destroy(gameObject);
    }

    private void OnTriggerExit(Collider other) {
        if (other.GetComponent<Molecule>()) {
            // molecule has broken free of the container
            moleculeEscapeEvent.Invoke();
        }
    }
}
