using UnityEngine;

namespace Chapter1
{
    public class Chapter1Bootstrap : MonoBehaviour
    {
        [Header("Blockout")]
        [SerializeField]
        private bool autoGenerateBlockout = true;
        [SerializeField]
        private Vector3 apartmentSize = new(8f, 3f, 8f);
        [SerializeField]
        private Vector3 courtyardSize = new(12f, 0.5f, 12f);
        [SerializeField]
        private Vector3 streetSize = new(20f, 0.25f, 8f);
        [SerializeField]
        private Vector3 transitHubSize = new(10f, 0.25f, 6f);
        [SerializeField]
        private float areaSpacing = 12f;

        private bool _hasBuilt;

        private void Awake()
        {
            if (!autoGenerateBlockout || _hasBuilt)
            {
                return;
            }

            BuildBlockout();
        }

        private void BuildBlockout()
        {
            _hasBuilt = true;

            var environmentRoot = EnsureChild("Environment");
            var interactablesRoot = EnsureChild("Interactables");
            var aiRoot = EnsureChild("AI");
            var systemsRoot = EnsureChild("Systems");

            CreateFloorArea(environmentRoot.transform, "Apartment4C", Vector3.zero, apartmentSize, new Color(0.55f, 0.55f, 0.6f));
            CreateFloorArea(environmentRoot.transform, "Courtyard", Vector3.forward * areaSpacing, courtyardSize, new Color(0.25f, 0.3f, 0.35f));
            CreateFloorArea(environmentRoot.transform, "Street", Vector3.forward * areaSpacing * 2f, streetSize, new Color(0.2f, 0.2f, 0.2f));
            CreateFloorArea(environmentRoot.transform, "TransitHubD4", Vector3.forward * areaSpacing * 3f, transitHubSize, new Color(0.18f, 0.18f, 0.22f));

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
    }
}
