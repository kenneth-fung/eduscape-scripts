using UnityEngine;

/// <summary>
/// Attach this component to objects that should react when hit by an element projectile.
/// </summary>
public class ElementTarget : MonoBehaviour {

    [SerializeField] private ElementEvent hitByElementProjectile;

    public void GetHitByElementProjectile(Element element) {
        hitByElementProjectile.Invoke(element);
    }
}
