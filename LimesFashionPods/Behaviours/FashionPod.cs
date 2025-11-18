#if NET6_0
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.World;
using MelonLoader;
#endif
using LimesFashionPods.Utilities;
using UnityEngine;

namespace LimesFashionPods.Behaviours
{
    #if NET6_0
    [RegisterTypeInIl2Cpp]
    #endif
    public class FashionPod : MonoBehaviour
    {
    #if NET6_0
        private const float ClearRad = 0.4f;
        
        //public Il2CppReferenceField<GameObject> spawnFX;
        public Il2CppReferenceField<IdentifiableType> fashionId;
        public Il2CppReferenceField<Transform> fashionItemPos;
        private GameObject? FashionPrefab => fashionId.Get()?.prefab;
        
        private Joint? _fashionJoint;
    #else
        public IdentifiableType fashionId;
        public Transform fashionItemPos;
    #endif

        private void Start()
        {
            var castedRes = gameObject;
            var renderers = castedRes.GetComponentsInChildren<MeshRenderer>();
           if (renderers.Length > 0 && castedRes.TryGetComponent<RegionMember>(out _))
            {
                MelonCoroutines.Start(AddressableShaderCache.ReloadAddressableShaders(castedRes));
            }
        }

        private void Update()
        {
    #if NET6_0
            var fashionItemPosRes = fashionItemPos.Get();
            var regionMemberScript = GetComponent<RegionMember>() ?? gameObject.AddComponent<RegionMember>();

            if (_fashionJoint != null && _fashionJoint.connectedBody == null)
            {
                Destroyer.Destroy(_fashionJoint, nameof(Update));
                _fashionJoint = null;
            }

            if (_fashionJoint != null || fashionItemPosRes == null || FashionPrefab == null ||
                Physics.CheckSphere(fashionItemPosRes.position, ClearRad)) return;
            try
            {
                var itemObj = InstantiationHelpers.InstantiateActor(FashionPrefab, regionMemberScript.SceneGroup,
                    fashionItemPosRes.position, fashionItemPosRes.rotation);
                var configurableJoint = fashionItemPosRes.GetComponent<ConfigurableJoint>() ??
                                        fashionItemPosRes.gameObject.AddComponent<ConfigurableJoint>();
                SafeJointReference.AttachSafely(itemObj, configurableJoint);
                configurableJoint.anchor = Vector3.zero;
                configurableJoint.autoConfigureConnectedAnchor = false;
                configurableJoint.connectedAnchor = Vector3.zero;
                var softJointLimitSpring = default(SoftJointLimitSpring);
                softJointLimitSpring.damper = 0.2f;
                softJointLimitSpring.spring = 1000f;
                configurableJoint.xMotion = ConfigurableJointMotion.Limited;
                configurableJoint.yMotion = ConfigurableJointMotion.Limited;
                configurableJoint.zMotion = ConfigurableJointMotion.Limited;
                configurableJoint.angularXMotion = ConfigurableJointMotion.Limited;
                configurableJoint.angularYMotion = ConfigurableJointMotion.Limited;
                configurableJoint.angularZMotion = ConfigurableJointMotion.Limited;
                configurableJoint.linearLimitSpring = softJointLimitSpring;
                configurableJoint.angularXLimitSpring = softJointLimitSpring;
                configurableJoint.angularYZLimitSpring = softJointLimitSpring;
                configurableJoint.breakForce = 20f;
                _fashionJoint = configurableJoint;
                fashionItemPosRes.localRotation = Quaternion.Euler(Vector3.zero);
            }
            catch
            {
                // ignore
            }
    #endif
        }

        private void FixedUpdate()
        {
    #if NET6_0
            var fashionItemPosRes = fashionItemPos.Get();
            if (_fashionJoint != null && fashionItemPosRes != null)
                fashionItemPosRes.Rotate(Vector3.up, 90f * Time.fixedDeltaTime);
    #endif
        }
    }
}
