using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A UnityEvent that triggers methods that with a single parameter of type <c>Element</c>.
/// </summary>
[System.Serializable]
public class ElementEvent : UnityEvent<Element> { }

/// <summary>
/// The base abstract class from which the different types of elements inherit.
/// </summary>
public abstract class Element : MonoBehaviour {

    public enum Type { FIRE, METAL, WATER }
    [SerializeField] protected Type type;
    public Type ElementType { get => type; protected set => type = value; }

    protected virtual void Awake() {}

    protected virtual void Start() {}

    protected virtual void OnEnable() {}

    /// <summary>
    /// Checks two Elements for equality.
    /// </summary>
    /// <param name="e1">The first element.</param>
    /// <param name="e2">The second element to compare against the first.</param>
    /// <returns>True if the elemenets are of the same type.</returns>
    public static bool operator== (Element e1, Element e2) {
        bool isE1Null = e1 is null;
        bool isE2Null = e2 is null;
        if (isE1Null && isE2Null) {
            return true;
        } else if ((isE1Null && !isE2Null) || (!isE1Null && isE2Null)) {
            return false;
        }

        return e1.ElementType == e2.ElementType;
    }

    /// <summary>
    /// Checks two elements for inequality.
    /// </summary>
    /// <param name="e1">The first element.</param>
    /// <param name="e2">The second element to compare against the first.</param>
    /// <returns>True if the elemenets are not of the same type.</returns>
    public static bool operator!= (Element e1, Element e2) {
        return !(e1 == e2);
    }

    /// <summary>
    /// Checks two Elements for equality.
    /// </summary>
    /// <param name="e1">The first element.</param>
    /// <param name="e2">The second element to compare against the first.</param>
    /// <returns>True if the elemenets are of the same type.</returns>
    public override bool Equals(object other) {
        Element otherElement = other as Element;
        if (otherElement == null) {
            return false;
        }

        return this == otherElement;
    }

    public override int GetHashCode() {
        return ElementType.GetHashCode();
    }
}
