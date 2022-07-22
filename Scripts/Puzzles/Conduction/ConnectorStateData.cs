using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Connector State Data", menuName = "Scriptable Objects/Ship/Connector State Data")]
public class ConnectorStateData : ScriptableObject {

    [SerializeField] private ShipEngineConnectorSegment.State state;
    public ShipEngineConnectorSegment.State State { get => state; }

    [SerializeField] private Material segmentMaterial;
    public Material SegmentMaterial { get => segmentMaterial; }

    [SerializeField] private Material glowMaterial;
    public Material GlowMaterial { get => glowMaterial; }

    [SerializeField] private Color glowColor;
    public Color GlowColor { get => glowColor; }

    [SerializeField] private AudioClip changeSound;
    public AudioClip ChangeSound { get => changeSound; }
}
