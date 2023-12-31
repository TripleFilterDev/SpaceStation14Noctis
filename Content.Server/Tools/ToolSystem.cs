using Content.Server.Atmos.EntitySystems;
using Content.Server.Chemistry.EntitySystems;
using Content.Server.Popups;
using Content.Server.Tools.Components;
using Content.Shared.Maps;
using Content.Shared.Tools;
using Robust.Server.GameObjects;
using Robust.Shared.Map;

namespace Content.Server.Tools
{
    // TODO move tool system to shared, and make it a friend of Tool Component.
    public sealed partial class ToolSystem : SharedToolSystem
    {
        [Dependency] private readonly ITileDefinitionManager _tileDefinitionManager = default!;
        [Dependency] private readonly IMapManager _mapManager = default!;
        [Dependency] private readonly SolutionContainerSystem _solutionContainerSystem = default!;
        [Dependency] private readonly AtmosphereSystem _atmosphereSystem = default!;
        [Dependency] private readonly PopupSystem _popupSystem = default!;
        [Dependency] private readonly TransformSystem _transformSystem = default!;
        [Dependency] private readonly TurfSystem _turf = default!;

        public override void Initialize()
        {
            base.Initialize();

            InitializeTilePrying();
            InitializeLatticeCutting();
            InitializeEarthDigging();
            InitializeWelders();
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            UpdateWelders(frameTime);
        }

        protected override bool IsWelder(EntityUid uid)
        {
            return HasComp<WelderComponent>(uid);
        }
    }
}
