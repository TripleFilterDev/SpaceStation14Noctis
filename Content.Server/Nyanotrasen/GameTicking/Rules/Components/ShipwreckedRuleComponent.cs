using Robust.Server.Player;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.Utility;
using Robust.Shared.Map.Components;
using Content.Server.Shipwrecked;
using Content.Shared.Procedural;
using Content.Shared.Roles;

namespace Content.Server.GameTicking.Rules.Components;

[RegisterComponent, Access(typeof(ShipwreckedRuleSystem))]
public sealed class ShipwreckedRuleComponent : Component
{

#region Config

    /// <summary>
    /// The shuttle that the game will start on.
    /// </summary>
    [ViewVariables]
    [DataField("shuttlePath", required: true, customTypeSerializer: typeof(ResPathSerializer))]
    public ResPath ShuttlePath = default!;

    /// <summary>
    /// The prototype that will be used to place travellers.
    /// </summary>
    [ViewVariables]
    [DataField("spawnPointTraveller", required: true, customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string SpawnPointTraveller = default!;

    /// <summary>
    /// The jobs that the travellers will be randomly assigned.
    /// </summary>
    [ViewVariables]
    [DataField("availableJobs", required: true, customTypeSerializer: typeof(PrototypeIdListSerializer<JobPrototype>))]
    public List<string> AvailableJobPrototypes = default!;

    /// <summary>
    /// The destinations for the shipwreck.
    /// </summary>
    [ViewVariables]
    [DataField("destinations", required: true, customTypeSerializer: typeof(PrototypeIdListSerializer<ShipwreckDestinationPrototype>))]
    public List<string> ShipwreckDestinationPrototypes = default!;

    /// <summary>
    /// Hecate's spawn point.
    /// </summary>
    [ViewVariables]
    [DataField("spawnPointHecate", required: true, customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string SpawnPointHecate = default!;

    /// <summary>
    /// Hecate's mob prototype.
    /// </summary>
    [ViewVariables]
    [DataField("hecatePrototype", required: true, customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string HecatePrototype = default!;

    /// <summary>
    /// The schedule of events to occur.
    /// </summary>
    [ViewVariables]
    [DataField("eventSchedule")]
    public List<(TimeSpan timeOffset, ShipwreckedEventId eventId)> EventSchedule = new();

#endregion

#region Live Data

    /// <summary>
    /// A list of all survivors and their player sessions.
    /// </summary>
    [ViewVariables]
    public List<(EntityUid entity, IPlayerSession session)> Survivors = new();

    /// <summary>
    /// Where the game starts and ends.
    /// </summary>
    [ViewVariables]
    public MapId? SpaceMapId;

    /// <summary>
    /// The shuttle's grid entity.
    /// </summary>
    [ViewVariables]
    public EntityUid? Shuttle;

    /// <summary>
    /// The chosen destination for the shipwreck.
    /// </summary>
    [ViewVariables]
    public ShipwreckDestinationPrototype? Destination;

    /// <summary>
    /// The map of the shipwreck destination.
    /// </summary>
    [ViewVariables]
    public MapId? PlanetMapId;

    /// <summary>
    /// The grid entity of the shipwreck destination.
    /// </summary>
    [ViewVariables]
    public EntityUid? PlanetMap;

    /// <summary>
    /// The MapGrid component of the PlanetMap entity.
    /// </summary>
    [ViewVariables]
    public MapGridComponent? PlanetGrid;

    /// <summary>
    /// The spawned instance of Hecate.
    /// </summary>
    [ViewVariables]
    public EntityUid? Hecate;

    /// <summary>
    /// The original number of intact and connected linear thrusters on the shuttle.
    /// </summary>
    [ViewVariables]
    public int OriginalThrusterCount;

    /// <summary>
    /// The original power demand on the shuttle's generator(s).
    /// </summary>
    [ViewVariables]
    public float OriginalPowerDemand;

    /// <summary>
    /// A dictionary of vital shuttle pieces and their eventual destinations once the shuttle decouples the engine.
    /// </summary>
    [ViewVariables]
    public Dictionary<EntityUid, (EntityCoordinates destination, Dungeon? structure)> VitalPieces = new();

    /// <summary>
    /// A dictionary of structures to coordinates of where the vital pieces landed.
    /// </summary>
    [ViewVariables]
    public Dictionary<Dungeon, List<EntityCoordinates>> VitalPieceStructureSpots = new();

    /// <summary>
    /// Keeps track of the internal event scheduler.
    /// </summary>
    [ViewVariables]
    [DataField("nextEventTick", customTypeSerializer: typeof(TimeOffsetSerializer))]
    public TimeSpan NextEventTick;

    /// <summary>
    /// The planetary structures.
    /// </summary>
    [ViewVariables]
    public List<Dungeon> Structures = new();

    /// <summary>
    /// If true, the game has been won.
    /// </summary>
    [ViewVariables]
    public bool AllObjectivesComplete;

#endregion

}

public enum ShipwreckedEventId : int
{
    AnnounceTransit,
    ShowHecate,
    IntroduceHecate,
    EncounterTurbulence,
    ShiftParallax,
    MidflightDamage,
    Alert,
    DecoupleEngine,
    SendDistressSignal,
    InterstellarBody,
    EnteringAtmosphere,
    Crash,
    AfterCrash,
    Sitrep,

    // The win event
    Launch,
}

