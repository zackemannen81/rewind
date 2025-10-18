using UnityEngine;
using Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Chapter1
{
[ExecuteAlways]
public class Chapter1Bootstrap : MonoBehaviour
    {
        [Header("Blockout")]
        [SerializeField]
        private bool autoGenerateBlockout = true;
        [SerializeField]
        private Vector3 apartmentSize = new(8f, 3f, 8f);
        [SerializeField]
        private Vector3 courtyardSize = new(12f, 0.5f, 14f);
        [SerializeField]
        private Vector3 streetSize = new(20f, 0.5f, 12f);
        [SerializeField]
        private Vector3 transitHubSize = new(12f, 0.5f, 10f);
        [SerializeField]
        private float areaSpacing = 12f;
        [Header("Audio")]
        [SerializeField]
        private AudioClip apartmentAmbient;
        [SerializeField]
        private AudioClip radioGarbleClip;
        [SerializeField]
        private AudioClip radioClueClip;
        [SerializeField]
        private AudioClip turnstileLockedClip;
        [SerializeField]
        private AudioClip turnstileOpenClip;
        [SerializeField]
        private AudioClip machineHumClip;
        [SerializeField]
        private AudioClip gateUnlockClip;

        [Header("Models")]
        [SerializeField]
        private GameObject fusePanelModel;
        [SerializeField]
        private GameObject radioConsoleModel;
        [SerializeField]
        private GameObject generatorModel;
        [SerializeField]
        private GameObject courtyardGateModel;
        [SerializeField]
        private GameObject turnstileModel;
        [SerializeField]
        private GameObject streetlightModel;
        [SerializeField]
        private GameObject kioskModel;
        [SerializeField]
        private GameObject benchModel;
        [SerializeField]
        private GameObject noticeBoardModel;
        [SerializeField]
        private GameObject tallBuildingModel;
        [SerializeField]
        private GameObject lowBuildingModel;
        [SerializeField]
        private GameObject apartmentBedModel;
        [SerializeField]
        private GameObject apartmentTableModel;
        [SerializeField]
        private GameObject apartmentChairModel;
        [SerializeField]
        private GameObject apartmentPlantModel;
        [SerializeField]
        private Material hologramMaterial;
        [SerializeField]
        private GameObject guardModel;
        [SerializeField]
        private GameObject droneModel;

        [Header("Palette")]
        [SerializeField]
        private Color apartmentFloorColor = new(0.06f, 0.08f, 0.12f);
        [SerializeField]
        private Color courtyardFloorColor = new(0.07f, 0.12f, 0.16f);
        [SerializeField]
        private Color streetFloorColor = new(0.07f, 0.09f, 0.13f);
        [SerializeField]
        private Color transitFloorColor = new(0.05f, 0.08f, 0.11f);
        [SerializeField]
        private Color connectorColor = new(0.1f, 0.12f, 0.18f);
        [SerializeField]
        private Color wallColor = new(0.1f, 0.12f, 0.18f);
        [SerializeField]
        private Color accentHighlightColor = new(0.78f, 0.19f, 0.84f, 0.35f);
        [SerializeField]
        private Color gatePanelColor = new(0.14f, 0.16f, 0.23f);
        [SerializeField]
        private Color interactZoneColor = new(0.36f, 0.17f, 0.72f, 0.18f);
        [SerializeField]
        private Color echoGhostColor = new(0.82f, 0.26f, 0.9f, 0.45f);

        private bool _hasBuilt;

        private void OnEnable()
        {
            if (!autoGenerateBlockout)
            {
                return;
            }

            if (_hasBuilt && transform.Find("Environment") != null)
            {
                return;
            }

#if UNITY_EDITOR
            AssignDefaultsIfNeeded();
#endif

            BuildBlockout();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this == null)
                {
                    return;
                }

                UnityEditor.EditorUtility.SetDirty(gameObject);
            };
        }
#endif
    }

        private void BuildBlockout()
        {
            _hasBuilt = true;

            var environmentRoot = EnsureChild("Environment");
            var interactablesRoot = EnsureChild("Interactables");
            var aiRoot = EnsureChild("AI");
            var systemsRoot = EnsureChild("Systems");

            CreateFloorArea(environmentRoot.transform, "Apartment4C", Vector3.zero, apartmentSize, apartmentFloorColor);
            CreateFloorArea(environmentRoot.transform, "Courtyard", Vector3.forward * (areaSpacing - 0.1f), courtyardSize, courtyardFloorColor);
            CreateFloorArea(environmentRoot.transform, "Street", Vector3.forward * (areaSpacing * 2f - 0.2f), streetSize, streetFloorColor);
            CreateFloorArea(environmentRoot.transform, "TransitHubD4", Vector3.forward * (areaSpacing * 3f - 0.3f), transitHubSize, transitFloorColor);

            var radioObject = EnsureChild(interactablesRoot.transform, "RadioChannels");
            var fuseBoxObject = EnsureChild(interactablesRoot.transform, "FuseBox");
            var generatorObject = EnsureChild(interactablesRoot.transform, "Generator");
            var gateObject = EnsureChild(interactablesRoot.transform, "CourtyardGate");
            var turnstileObject = EnsureChild(interactablesRoot.transform, "TransitTurnstile");

            EnsureChild(aiRoot.transform, "GuardPath");
            EnsureChild(aiRoot.transform, "DronePath");

            var loopSystemsObject = EnsureChild(systemsRoot.transform, "LoopEntryPoints");
            EnsureChild(systemsRoot.transform, "AnchorTriggers");
            EnsureChild(systemsRoot.transform, "KnowledgeMarkers");

            var radioController = GetOrAddComponent<Chapter1RadioController>(radioObject);
            var radioSource = GetOrAddComponent<AudioSource>(radioObject);
            radioSource.playOnAwake = false;
            radioController.SetAudioSource(radioSource);
            var fuseBox = GetOrAddComponent<Chapter1FuseBox>(fuseBoxObject);
            var generator = GetOrAddComponent<Chapter1Generator>(generatorObject);
            generator.SetFuseBox(fuseBox);
            var gate = GetOrAddComponent<Chapter1CourtyardGate>(gateObject);
            var turnstile = GetOrAddComponent<Chapter1TransitTurnstile>(turnstileObject);

            generator.OnGeneratorActivated -= gate.Open;
            generator.OnGeneratorActivated += gate.Open;

            BuildFuseBox(fuseBoxObject.transform, fuseBox);
            BuildGenerator(generatorObject.transform, generator);
            BuildGate(gateObject.transform, gate);
            BuildRadio(radioObject.transform, radioController);
            BuildTurnstile(turnstileObject.transform, turnstile, gate);
            BuildConnectors(environmentRoot.transform);
            BuildPerimeterWalls(environmentRoot.transform);
            BuildAmbientNode(environmentRoot.transform);
            DecorateApartment(environmentRoot.transform.Find("Apartment4C"));
            PopulateCourtyard(environmentRoot.transform.Find("Courtyard"));
            PopulateStreet(environmentRoot.transform.Find("Street"));
            DeployAI(aiRoot.transform);
            EnsureEchoSystem(systemsRoot.transform);
            ConfigureLighting();

            var orchestrator = GetOrAddComponent<Chapter1LoopOrchestrator>(loopSystemsObject);
            orchestrator.Configure(radioController, fuseBox, generator, gate, turnstile);
        }

        private GameObject EnsureChild(string childName)
        {
            var existing = transform.Find(childName);
            if (existing != null)
            {
                return existing.gameObject;
            }

            var child = new GameObject(childName);
            child.transform.SetParent(transform, false);
            return child;
        }

        private GameObject EnsureChild(Transform parent, string childName)
        {
            var existing = parent.Find(childName);
            if (existing != null)
            {
                return existing.gameObject;
            }

            var child = new GameObject(childName);
            child.transform.SetParent(parent, false);
            return child;
        }

        private void CreateFloorArea(Transform parent, string areaName, Vector3 localOffset, Vector3 size, Color color)
        {
            var area = EnsureChild(parent, areaName);
            area.transform.localPosition = localOffset;
            area.transform.localRotation = Quaternion.identity;

            var blockoutTransform = area.transform.Find("Blockout");
            if (blockoutTransform == null)
            {
                var blockout = CreatePrimitive(area.transform, color);
                blockoutTransform = blockout.transform;
            }

            blockoutTransform.localPosition = Vector3.zero;
            blockoutTransform.localRotation = Quaternion.identity;
            blockoutTransform.localScale = size;
        }

        private GameObject CreatePrimitive(Transform parent, Color color)
        {
            var primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
            primitive.name = "Blockout";
            primitive.transform.SetParent(parent, false);

            var renderer = primitive.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("HDRP/Lit") ?? Shader.Find("Standard");
                if (shader != null)
                {
                    var material = new Material(shader)
                    {
                        color = color
                    };
                    renderer.sharedMaterial = material;
                }
            }

            primitive.layer = ResolveLayer("Ground");
            var collider = primitive.GetComponent<Collider>();
            if (collider != null)
            {
                collider.gameObject.layer = primitive.layer;
            }

            return primitive;
        }

        private void BuildFuseBox(Transform host, Chapter1FuseBox fuseBox)
        {
            var visual = EnsureChild(host, "Visual");
            PositionInteractObject(visual, new Vector3(-3f, 1.2f, 3.4f), new Vector3(0.45f, 0.6f, 0.12f), gatePanelColor);
            EnsureModelInstance(visual.transform, fusePanelModel, FusePanelDefaultPath, Vector3.zero, Vector3.zero, Vector3.one);

            var interactNode = EnsureChild(host, "InteractZone");
            PositionInteractObject(interactNode, new Vector3(-3f, 1.2f, 3.4f), new Vector3(0.5f, 1.2f, 0.4f), interactZoneColor, true);

            var fuseInteract = GetOrAddComponent<Chapter1FuseInteract>(interactNode)
                ?? interactNode.gameObject.AddComponent<Chapter1FuseInteract>();
            fuseInteract.SetFuseBox(fuseBox);
        }

        private void BuildGenerator(Transform host, Chapter1Generator generator)
        {
            var basePosition = new Vector3(-1.5f, 0.5f, areaSpacing + 1.5f);
            var visual = EnsureChild(host, "Visual");
            PositionInteractObject(visual, basePosition, new Vector3(1.2f, 1f, 0.8f), gatePanelColor);
            EnsureModelInstance(visual.transform, generatorModel, GeneratorDefaultPath, Vector3.zero, new Vector3(0f, 180f, 0f), Vector3.one * 0.95f);

            var interactNode = EnsureChild(host, "InteractZone");
            PositionInteractObject(interactNode, basePosition, new Vector3(1.4f, 1.6f, 1.0f), interactZoneColor, true);

            var generatorInteract = GetOrAddComponent<Chapter1GeneratorInteract>(interactNode)
                ?? interactNode.gameObject.AddComponent<Chapter1GeneratorInteract>();
            generatorInteract.SetGenerator(generator);

            var audioSource = GetOrAddComponent<AudioSource>(visual);
            if (audioSource != null && machineHumClip != null)
            {
                audioSource.clip = machineHumClip;
                audioSource.loop = true;
                audioSource.playOnAwake = false;
                generator.SetAudioSource(audioSource);
            }
        }

        private void BuildGate(Transform host, Chapter1CourtyardGate gate)
        {
            var gateVisual = EnsureChild(host, "Visual");
            var gatePositionZ = 7.5f;
            PositionInteractObject(gateVisual, new Vector3(0f, 1.5f, gatePositionZ), new Vector3(4f, 3f, 0.5f), gatePanelColor);
            EnsureModelInstance(gateVisual.transform, courtyardGateModel, CourtyardGateDefaultPath, new Vector3(0f, -1.5f, 0f), Vector3.zero, new Vector3(0.012f, 0.012f, 0.012f));

            var collider = GetOrAddComponent<BoxCollider>(gateVisual);
            collider.isTrigger = false;
            collider.center = new Vector3(0f, 0f, 0f);
            collider.size = new Vector3(4f, 3f, 0.5f);
            gate.AssignCollider(collider);

            // Block flanking paths so the player must pass through the gate.
            CreateGateBarrier(host, "Barrier_Left", new Vector3(-2.5f, 1.5f, gatePositionZ - 0.2f));
            CreateGateBarrier(host, "Barrier_Right", new Vector3(2.5f, 1.5f, gatePositionZ - 0.2f));

            var audioSource = GetOrAddComponent<AudioSource>(gateVisual);
            if (audioSource != null && gateUnlockClip != null)
            {
                audioSource.clip = gateUnlockClip;
                audioSource.playOnAwake = false;
                gate.SetAudioSource(audioSource);
            }
        }

        private void CreateGateBarrier(Transform root, string name, Vector3 localPosition)
        {
            var barrier = EnsureChild(root, name);
            barrier.transform.localPosition = localPosition;
            barrier.transform.localRotation = Quaternion.identity;
            barrier.transform.localScale = new Vector3(1f, 3f, 1f);

            var meshRenderer = barrier.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                var primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
                primitive.name = "Block";
                primitive.transform.SetParent(barrier.transform, false);
                primitive.transform.localPosition = Vector3.zero;
                primitive.transform.localScale = Vector3.one;
                meshRenderer = primitive.GetComponent<MeshRenderer>();
                meshRenderer.sharedMaterial = CreateBlockMaterial(wallColor);
                var primitiveCollider = primitive.GetComponent<BoxCollider>();
                primitiveCollider.enabled = false;
            }

            var collider = barrier.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = barrier.AddComponent<BoxCollider>();
            }
            collider.isTrigger = false;
        }

        private void BuildRadio(Transform host, Chapter1RadioController radioController)
        {
            var visual = EnsureChild(host, "Console");
            PositionInteractObject(visual, new Vector3(-2.2f, 1.1f, 2.6f), new Vector3(0.7f, 0.8f, 0.4f), gatePanelColor);
            EnsureModelInstance(visual.transform, radioConsoleModel, RadioConsoleDefaultPath, Vector3.zero, new Vector3(0f, 180f, 0f), Vector3.one * 0.009f);

            var interactNode = EnsureChild(host, "InteractZone");
            PositionInteractObject(interactNode, new Vector3(-2.2f, 1.1f, 2.6f), new Vector3(0.9f, 1.4f, 0.8f), interactZoneColor, true);

            var radioInteract = GetOrAddComponent<Chapter1RadioInteract>(interactNode)
                ?? interactNode.gameObject.AddComponent<Chapter1RadioInteract>();
            radioInteract.enabled = true;
            radioInteract.Initialize(radioController, radioGarbleClip, radioClueClip);
            radioController.SetAudioClips(radioGarbleClip, radioClueClip);
            var radioAudio = host.GetComponent<AudioSource>();
            if (radioAudio != null && radioGarbleClip != null)
            {
                radioAudio.clip = radioGarbleClip;
            }
        }

        private void BuildTurnstile(Transform host, Chapter1TransitTurnstile turnstile, Chapter1CourtyardGate gate)
        {
            var basePosition = new Vector3(0f, 1.0f, areaSpacing * 3f - 1f);
            var frame = EnsureChild(host, "Frame");
            PositionInteractObject(frame, basePosition, new Vector3(4.5f, 2.2f, 0.6f), gatePanelColor);
            EnsureModelInstance(frame.transform, turnstileModel, TurnstileDefaultPath, new Vector3(0f, -1.0f, 0f), Vector3.zero, new Vector3(0.01f, 0.01f, 0.01f));

            var interactNode = EnsureChild(host, "InteractZone");
            PositionInteractObject(interactNode, basePosition + new Vector3(0f, -0.5f, 0.65f), new Vector3(2.5f, 2f, 2f), interactZoneColor, true);

            var audioSource = GetOrAddComponent<AudioSource>(interactNode);
            audioSource.playOnAwake = false;

            var turnstileInteract = GetOrAddComponent<Chapter1TurnstileInteract>(interactNode)
                ?? interactNode.gameObject.AddComponent<Chapter1TurnstileInteract>();
            turnstileInteract.enabled = true;
            turnstileInteract.Initialize(turnstile, gate, turnstileLockedClip, turnstileOpenClip);
        }

        private void BuildConnectors(Transform environmentRoot)
        {
            CreateConnector(environmentRoot, "Apartment_Courtyard_Bridge", new Vector3(0f, 0.05f, areaSpacing * 0.6f), new Vector3(4.5f, 0.2f, areaSpacing * 0.7f));
            CreateConnector(environmentRoot, "Courtyard_Street_Bridge", new Vector3(0f, 0.05f, areaSpacing * 1.5f), new Vector3(4.5f, 0.2f, areaSpacing * 0.7f));
            CreateConnector(environmentRoot, "Street_Hub_Bridge", new Vector3(0f, 0.05f, areaSpacing * 2.4f), new Vector3(4.5f, 0.2f, areaSpacing * 0.7f));
        }

        private void CreateConnector(Transform parent, string name, Vector3 localPosition, Vector3 size)
        {
            var connector = EnsureChild(parent, name);
            connector.transform.localPosition = localPosition;
            connector.transform.localRotation = Quaternion.identity;
            connector.transform.localScale = size;

            var renderer = connector.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                var primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
                primitive.name = "Block";
                primitive.transform.SetParent(connector.transform, false);
                primitive.transform.localPosition = Vector3.zero;
                primitive.transform.localRotation = Quaternion.identity;
                primitive.transform.localScale = Vector3.one;

                renderer = primitive.GetComponent<MeshRenderer>();
                renderer.sharedMaterial = CreateBlockMaterial(connectorColor);
                var collider = primitive.GetComponent<BoxCollider>();
                collider.enabled = false;
            }

            var colliderComponent = connector.GetComponent<BoxCollider>();
            if (colliderComponent == null)
            {
                colliderComponent = connector.AddComponent<BoxCollider>();
            }
            colliderComponent.isTrigger = false;
        }

        private void BuildPerimeterWalls(Transform environmentRoot)
        {
            BuildWall(environmentRoot, "Courtyard_Wall_North", new Vector3(3.12f, 1.5f, 12.5f), new Vector3(courtyardSize.x + 4f, 3f, 0.4f), new Vector3(0f, 90f, 0f));
            BuildWall(environmentRoot, "Courtyard_Wall_South", new Vector3(-3.25f, 1.5f, 12.5f), new Vector3(courtyardSize.x + 4f, 3f, 0.4f), new Vector3(0f, 90f, 0f));
            BuildWall(environmentRoot, "Street_Wall_East", new Vector3(6f, 2f, areaSpacing * 2f), new Vector3(0.4f, 4f, streetSize.z + 8f));
            BuildWall(environmentRoot, "Street_Wall_West", new Vector3(-6f, 2f, areaSpacing * 2f), new Vector3(0.4f, 4f, streetSize.z + 8f));
        }

        private void BuildWall(Transform parent, string name, Vector3 position, Vector3 scale, Vector3? rotationEuler = null)
        {
            var wall = EnsureChild(parent, name);
            wall.transform.localPosition = position;
            wall.transform.localRotation = rotationEuler.HasValue ? Quaternion.Euler(rotationEuler.Value) : Quaternion.identity;
            wall.transform.localScale = scale;

            var renderer = wall.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                var primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
                primitive.name = "Wall";
                primitive.transform.SetParent(wall.transform, false);
                primitive.transform.localPosition = Vector3.zero;
                primitive.transform.localRotation = Quaternion.identity;
                primitive.transform.localScale = Vector3.one;
                renderer = primitive.GetComponent<MeshRenderer>();
                renderer.sharedMaterial = CreateBlockMaterial(wallColor);
                var collider = primitive.GetComponent<BoxCollider>();
                collider.enabled = false;
            }

            var box = wall.GetComponent<BoxCollider>();
            if (box == null)
            {
                box = wall.AddComponent<BoxCollider>();
            }
            box.isTrigger = false;
        }

        private void BuildAmbientNode(Transform parent)
        {
            var ambient = EnsureChild(parent, "AmbientAudio");
            ambient.transform.localPosition = new Vector3(0f, 1.8f, areaSpacing);

            var audioSource = GetOrAddComponent<AudioSource>(ambient);
            audioSource.spatialBlend = 0.6f;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
            audioSource.maxDistance = 30f;

            var ambientController = GetOrAddComponent<Chapter1AmbientController>(ambient)
                ?? ambient.AddComponent<Chapter1AmbientController>();
            ambientController.SetClip(apartmentAmbient);
        }

        private void DecorateApartment(Transform apartmentRoot)
        {
            if (apartmentRoot == null)
            {
                return;
            }

            var furnitureRoot = EnsureChild(apartmentRoot, "Furniture").transform;
            EnsureModelInstance(furnitureRoot, apartmentBedModel, BedDefaultPath, new Vector3(2.5f, 0.05f, -2.2f), new Vector3(0f, -90f, 0f), Vector3.one * 0.01f);
            EnsureModelInstance(furnitureRoot, apartmentTableModel, TableDefaultPath, new Vector3(-1.6f, 0.05f, 0.9f), new Vector3(0f, 45f, 0f), Vector3.one * 0.012f);
            EnsureModelInstance(furnitureRoot, apartmentChairModel, ChairDefaultPath, new Vector3(-2.4f, 0.05f, 1.8f), new Vector3(0f, 140f, 0f), Vector3.one * 0.011f);
            EnsureModelInstance(furnitureRoot, apartmentPlantModel, PlantDefaultPath, new Vector3(3.1f, 0.05f, 1.8f), Vector3.zero, Vector3.one * 0.01f);

            BuildNoticeBoard(apartmentRoot);
        }

        private void PopulateCourtyard(Transform courtyardRoot)
        {
            if (courtyardRoot == null)
            {
                return;
            }

            var props = EnsureChild(courtyardRoot, "Props").transform;
            EnsureModelInstance(props, benchModel, BenchDefaultPath, new Vector3(3.5f, 0f, areaSpacing + 2.6f), new Vector3(0f, -90f, 0f), Vector3.one * 0.01f);
            EnsureModelInstance(props, noticeBoardModel, NoticeBoardDefaultPath, new Vector3(-4.2f, 0f, areaSpacing + 2.8f), new Vector3(0f, 90f, 0f), Vector3.one * 0.012f);
            EnsureStreetlight(props, new Vector3(-5f, 0f, areaSpacing + 4f));
            EnsureStreetlight(props, new Vector3(5f, 0f, areaSpacing + 3f));
        }

        private void PopulateStreet(Transform streetRoot)
        {
            if (streetRoot == null)
            {
                return;
            }

            var props = EnsureChild(streetRoot, "Props").transform;
            EnsureModelInstance(props, kioskModel, KioskDefaultPath, new Vector3(-3.4f, 0f, areaSpacing * 2f - 2.4f), new Vector3(0f, 90f, 0f), Vector3.one * 0.012f);
            EnsureStreetlight(props, new Vector3(4.5f, 0f, areaSpacing * 2f - 3f));
            EnsureStreetlight(props, new Vector3(-4.5f, 0f, areaSpacing * 2f + 1f));

            var skyline = EnsureChild(streetRoot, "Skyline").transform;
            EnsureModelInstance(skyline, tallBuildingModel, TallBuildingDefaultPath, new Vector3(-12f, 0f, areaSpacing * 2f + 10f), Vector3.zero, Vector3.one);
            EnsureModelInstance(skyline, lowBuildingModel, LowBuildingDefaultPath, new Vector3(12f, 0f, areaSpacing * 2f + 8f), new Vector3(0f, 180f, 0f), Vector3.one);
        }

        private void DeployAI(Transform aiRoot)
        {
            if (aiRoot == null)
            {
                return;
            }

            var guardRoot = EnsureChild(aiRoot, "GuardPath").transform;
            var guardWaypoints = EnsureWaypointSet(guardRoot, "Point", new Vector3[]
            {
                new(-2.8f, 0f, areaSpacing + 2.0f),
                new(2.6f, 0f, areaSpacing + 2.2f),
                new(2.4f, 0f, areaSpacing - 1.2f),
                new(-2.6f, 0f, areaSpacing - 1.4f)
            });
            SpawnGuardAgent(guardRoot, guardWaypoints);

            var droneRoot = EnsureChild(aiRoot, "DronePath").transform;
            var droneWaypoints = EnsureWaypointSet(droneRoot, "Point", new Vector3[]
            {
                new(-3f, 4.5f, areaSpacing * 2f - 2f),
                new(3f, 4.8f, areaSpacing * 2f - 1f),
                new(2.5f, 5.2f, areaSpacing * 2f + 3f),
                new(-2.5f, 4.6f, areaSpacing * 2f + 2.5f)
            });
            SpawnDroneAgent(droneRoot, droneWaypoints);
        }

        private Transform[] EnsureWaypointSet(Transform root, string prefix, Vector3[] positions)
        {
            var waypoints = new Transform[positions.Length];
            for (int i = 0; i < positions.Length; i++)
            {
                var name = $"{prefix}_{i:00}";
                var wp = root.Find(name);
                if (wp == null)
                {
                    var go = new GameObject(name);
                    go.transform.SetParent(root, false);
                    wp = go.transform;
                }

                wp.localPosition = positions[i];
                wp.localRotation = Quaternion.identity;
                waypoints[i] = wp;
            }

            return waypoints;
        }

        private void SpawnGuardAgent(Transform guardRoot, Transform[] waypoints)
        {
            var agent = EnsureAgent(guardRoot, "GuardAgent", guardModel, GuardDefaultPath, new Vector3(0f, 0f, 0f), new Vector3(0.01f, 0.01f, 0.01f));
            var patrol = agent.GetComponent<Chapter1GuardPatrol>() ?? agent.AddComponent<Chapter1GuardPatrol>();
            patrol.SetWaypoints(waypoints);
            patrol.SetColors(accentHighlightColor, connectorColor);
            if (waypoints.Length > 0)
            {
                agent.transform.position = waypoints[0].position;
            }
        }

        private void SpawnDroneAgent(Transform droneRoot, Transform[] waypoints)
        {
            var agent = EnsureAgent(droneRoot, "DroneAgent", droneModel, DroneDefaultPath, Vector3.zero, new Vector3(0.01f, 0.01f, 0.01f));
            var patrol = agent.GetComponent<Chapter1DronePatrol>() ?? agent.AddComponent<Chapter1DronePatrol>();
            patrol.SetWaypoints(waypoints);
            patrol.SetEmissiveColor(accentHighlightColor);
            if (waypoints.Length > 0)
            {
                agent.transform.position = waypoints[0].position;
            }
        }

        private GameObject EnsureAgent(Transform parent, string name, GameObject prefab, string fallbackPath, Vector3 offset, Vector3 scale)
        {
            var node = parent.Find(name);
            if (node == null)
            {
                var go = new GameObject(name);
                go.transform.SetParent(parent, false);
                node = go.transform;
            }

            if (node.childCount == 0)
            {
                var model = prefab;
#if UNITY_EDITOR
                if (model == null && !string.IsNullOrEmpty(fallbackPath))
                {
                    model = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(fallbackPath);
                }
#endif
                if (model != null)
                {
                    var instance = Instantiate(model, node);
                    instance.transform.localPosition = offset;
                    instance.transform.localRotation = Quaternion.identity;
                    instance.transform.localScale = scale;
                }
            }

            node.localPosition = Vector3.zero;
            node.localRotation = Quaternion.identity;
            return node.gameObject;
        }

        private void EnsureEchoSystem(Transform systemsRoot)
        {
            if (systemsRoot == null)
            {
                return;
            }

            var echoRoot = EnsureChild(systemsRoot, "EchoSystem");
            var recorder = echoRoot.GetComponent<EchoRecorder>() ?? echoRoot.AddComponent<EchoRecorder>();
            var playback = echoRoot.GetComponent<EchoPlayback>();
            if (playback == null)
            {
                playback = echoRoot.AddComponent<EchoPlayback>();
            }

            var visualizer = echoRoot.GetComponent<Chapter1EchoVisualizer>() ?? echoRoot.AddComponent<Chapter1EchoVisualizer>();
            visualizer.SetGhostTint(echoGhostColor);
        }

        private void ConfigureLighting()
        {
            if (RenderSettings.sun != null)
            {
                var sun = RenderSettings.sun;
                sun.color = new Color(0.45f, 0.62f, 1.0f);
                sun.intensity = 1.35f;
                sun.shadowStrength = 0.85f;
                sun.transform.rotation = Quaternion.Euler(38f, -35f, 0f);
            }

            RenderSettings.ambientLight = new Color(0.045f, 0.058f, 0.085f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.025f, 0.03f, 0.05f);
            RenderSettings.fogDensity = 0.01f;
        }

        private void BuildNoticeBoard(Transform apartmentRoot)
        {
            var board = EnsureChild(apartmentRoot, "NoticeBoard");
            PositionInteractObject(board, new Vector3(-2.8f, 1.3f, -2.8f), new Vector3(1.2f, 1f, 0.1f), accentHighlightColor);
            EnsureModelInstance(board.transform, noticeBoardModel, NoticeBoardDefaultPath, Vector3.zero, Vector3.zero, Vector3.one * 0.01f, allowMultiple: false, applyMaterial: hologramMaterial);
        }

        private void PositionInteractObject(GameObject node, Vector3 localPosition, Vector3 localScale, Color color, bool trigger = false)
        {
            node.transform.localPosition = localPosition;
            node.transform.localRotation = Quaternion.identity;
            node.transform.localScale = localScale;

            var mesh = node.GetComponent<MeshFilter>();
            if (mesh == null)
            {
                var primitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
                primitive.transform.SetParent(node.transform, false);
                primitive.transform.localPosition = Vector3.zero;
                primitive.transform.localRotation = Quaternion.identity;
                primitive.transform.localScale = Vector3.one;

                var renderer = primitive.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.sharedMaterial = CreateBlockMaterial(color);
                }

                var primitiveCollider = primitive.GetComponent<Collider>();
                if (primitiveCollider != null)
                {
                    primitiveCollider.enabled = false;
                }
            }

            var nodeCollider = node.GetComponent<BoxCollider>();
            if (nodeCollider == null)
            {
                nodeCollider = node.AddComponent<BoxCollider>();
            }
            nodeCollider.isTrigger = trigger;
        }

        private Material CreateBlockMaterial(Color baseColor)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("HDRP/Lit") ?? Shader.Find("Standard");
            var material = new Material(shader)
            {
                color = baseColor
            };
            material.SetColor("_EmissionColor", baseColor * 0.45f);
            material.EnableKeyword("_EMISSION");
            return material;
        }

        private int ResolveLayer(string layerName)
        {
            var layer = LayerMask.NameToLayer(layerName);
            return layer >= 0 ? layer : gameObject.layer;
        }

        private T GetOrAddComponent<T>(GameObject host) where T : Component
        {
            var component = host.GetComponent<T>();
            return component != null ? component : host.AddComponent<T>();
        }

        private void EnsureModelInstance(Transform parent, GameObject reference, string defaultPath, Vector3 localPosition, Vector3 localRotationEuler, Vector3 localScale, bool allowMultiple = false, Material applyMaterial = null)
        {
            var target = parent.Find("Model");
            if (target != null && !allowMultiple)
            {
                return;
            }

            var modelPrefab = reference;

#if UNITY_EDITOR
            if (modelPrefab == null && !string.IsNullOrEmpty(defaultPath))
            {
                modelPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(defaultPath);
            }
#endif

            if (modelPrefab == null)
            {
                return;
            }

            foreach (Transform child in parent)
            {
                if (child.name is "Block" or "Blockout")
                {
                    child.gameObject.SetActive(false);
                }
            }

            var instance = Instantiate(modelPrefab, parent);
            instance.name = allowMultiple ? modelPrefab.name : "Model";
            instance.transform.localPosition = localPosition;
            instance.transform.localRotation = Quaternion.Euler(localRotationEuler);
            instance.transform.localScale = localScale;

            if (applyMaterial != null)
            {
                var renderers = instance.GetComponentsInChildren<MeshRenderer>(true);
                foreach (var renderer in renderers)
                {
                    renderer.sharedMaterial = applyMaterial;
                }
            }
        }

        private void EnsureStreetlight(Transform parent, Vector3 localPosition)
        {
            var key = $"Streetlight_{Mathf.RoundToInt(localPosition.x * 10f)}_{Mathf.RoundToInt(localPosition.z * 10f)}";
            var lightRootTransform = parent.Find(key);
            if (lightRootTransform == null)
            {
                var lightRoot = new GameObject(key);
                lightRoot.transform.SetParent(parent, false);
                lightRoot.transform.localPosition = localPosition;
                EnsureModelInstance(lightRoot.transform, streetlightModel, StreetlightDefaultPath, Vector3.zero, Vector3.zero, Vector3.one * 0.012f, allowMultiple: true);

                var spot = lightRoot.AddComponent<Light>();
                spot.type = LightType.Spot;
                spot.range = 14f;
                spot.intensity = 4f;
                spot.spotAngle = 70f;
                spot.color = new Color(0.7f, 0.25f, 0.95f);
                spot.transform.localPosition = new Vector3(0f, 6f, 0f);
                spot.transform.localRotation = Quaternion.Euler(75f, 0f, 0f);
            }
        }

#if UNITY_EDITOR
        private void AssignDefaultsIfNeeded()
        {
            apartmentAmbient = EnsureClip(apartmentAmbient, AmbientDefaultPath);
            radioGarbleClip = EnsureClip(radioGarbleClip, RadioGarbleDefaultPath);
            radioClueClip = EnsureClip(radioClueClip, RadioClueDefaultPath);
            turnstileLockedClip = EnsureClip(turnstileLockedClip, TurnstileLockedDefaultPath);
            turnstileOpenClip = EnsureClip(turnstileOpenClip, TurnstileOpenDefaultPath);
            machineHumClip = EnsureClip(machineHumClip, MachineHumDefaultPath);
            gateUnlockClip = EnsureClip(gateUnlockClip, GateUnlockDefaultPath);

            fusePanelModel = EnsureModel(fusePanelModel, FusePanelDefaultPath);
            radioConsoleModel = EnsureModel(radioConsoleModel, RadioConsoleDefaultPath);
            generatorModel = EnsureModel(generatorModel, GeneratorDefaultPath);
            courtyardGateModel = EnsureModel(courtyardGateModel, CourtyardGateDefaultPath);
            turnstileModel = EnsureModel(turnstileModel, TurnstileDefaultPath);
            streetlightModel = EnsureModel(streetlightModel, StreetlightDefaultPath);
            kioskModel = EnsureModel(kioskModel, KioskDefaultPath);
            benchModel = EnsureModel(benchModel, BenchDefaultPath);
            noticeBoardModel = EnsureModel(noticeBoardModel, NoticeBoardDefaultPath);
            tallBuildingModel = EnsureModel(tallBuildingModel, TallBuildingDefaultPath);
            lowBuildingModel = EnsureModel(lowBuildingModel, LowBuildingDefaultPath);
            apartmentBedModel = EnsureModel(apartmentBedModel, BedDefaultPath);
            apartmentTableModel = EnsureModel(apartmentTableModel, TableDefaultPath);
            apartmentChairModel = EnsureModel(apartmentChairModel, ChairDefaultPath);
            apartmentPlantModel = EnsureModel(apartmentPlantModel, PlantDefaultPath);
            hologramMaterial = EnsureMaterial(hologramMaterial, HologramMaterialDefaultPath);
            guardModel = EnsureModel(guardModel, GuardDefaultPath);
            droneModel = EnsureModel(droneModel, DroneDefaultPath);
        }

        private AudioClip EnsureClip(AudioClip clip, string assetPath)
        {
            return clip != null ? clip : UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
        }

        private GameObject EnsureModel(GameObject model, string assetPath)
        {
            return model != null ? model : UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        }

        private Material EnsureMaterial(Material material, string assetPath)
        {
            if (material != null || string.IsNullOrEmpty(assetPath))
            {
                return material;
            }

            return UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(assetPath);
        }

        private const string AmbientDefaultPath = "Assets/WorkInProgressOrPlaceHolders/Chapter1_Ambient.wav";
        private const string RadioGarbleDefaultPath = "Assets/WorkInProgressOrPlaceHolders/Radio_Code_7312.wav";
        private const string RadioClueDefaultPath = "Assets/WorkInProgressOrPlaceHolders/AudioLog.wav";
        private const string TurnstileLockedDefaultPath = "Assets/WorkInProgressOrPlaceHolders/AlertBeep.wav";
        private const string TurnstileOpenDefaultPath = "Assets/WorkInProgressOrPlaceHolders/Reset.wav";
        private const string MachineHumDefaultPath = "Assets/WorkInProgressOrPlaceHolders/MachineHum.wav";
        private const string GateUnlockDefaultPath = "Assets/WorkInProgressOrPlaceHolders/StealthPulse.wav";

        private const string FusePanelDefaultPath = "Assets/WorkInProgressOrPlaceHolders/WallTerminal.obj";
        private const string RadioConsoleDefaultPath = "Assets/WorkInProgressOrPlaceHolders/DesktopTerminal.obj";
        private const string GeneratorDefaultPath = "Assets/WorkInProgressOrPlaceHolders/Generator.obj";
        private const string CourtyardGateDefaultPath = "";
        private const string TurnstileDefaultPath = "Assets/WorkInProgressOrPlaceHolders/Turnstile.obj";
        private const string StreetlightDefaultPath = "Assets/WorkInProgressOrPlaceHolders/Streetlight.obj";
        private const string KioskDefaultPath = "Assets/WorkInProgressOrPlaceHolders/Kiosk.obj";
        private const string BenchDefaultPath = "Assets/WorkInProgressOrPlaceHolders/Bench.obj";
        private const string NoticeBoardDefaultPath = "Assets/WorkInProgressOrPlaceHolders/NoticeBoard.obj";
        private const string TallBuildingDefaultPath = "Assets/WorkInProgressOrPlaceHolders/city_kit/Skyscraper/skyscraperA.obj";
        private const string LowBuildingDefaultPath = "Assets/WorkInProgressOrPlaceHolders/city_kit/Low Building/low_buildingA.obj";
        private const string BedDefaultPath = "Assets/WorkInProgressOrPlaceHolders/furniture_kit/Bed Double/bedDouble.obj";
        private const string TableDefaultPath = "Assets/WorkInProgressOrPlaceHolders/furniture_kit/Table/table.obj";
        private const string ChairDefaultPath = "Assets/WorkInProgressOrPlaceHolders/furniture_kit/Chair/chair.obj";
        private const string PlantDefaultPath = "Assets/WorkInProgressOrPlaceHolders/furniture_kit/Potted Plant/pottedPlant.obj";
        private const string HologramMaterialDefaultPath = "";
        private const string GuardDefaultPath = "Assets/WorkInProgressOrPlaceHolders/Guard.fbx";
        private const string DroneDefaultPath = "Assets/WorkInProgressOrPlaceHolders/DroneEnemy.fbx";
#endif
    }
}
