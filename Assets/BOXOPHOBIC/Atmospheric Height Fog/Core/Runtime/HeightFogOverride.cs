// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using Boxophobic;
using Boxophobic.StyledGUI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[HideScriptField]
[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
[HelpURL("https://docs.google.com/document/d/1pIzIHIZ-cSh2ykODSZCbAPtScJ4Jpuu7lS3rNEHCLbc/edit#heading=h.hd5jt8lucuqq")]
public class HeightFogOverride : MonoBehaviour
{
    [StyledBanner(0.474f, 0.709f, 0.901f, "Height Fog Override", "", "https://docs.google.com/document/d/1pIzIHIZ-cSh2ykODSZCbAPtScJ4Jpuu7lS3rNEHCLbc/edit#heading=h.hd5jt8lucuqq")]
    public bool styledBanner;

    [StyledMessage("Info", "The Height Fog Global object is missing from your scene! Please add it before using the Height Fog Override component!", 5, 0)]
    public bool messageNoHeightFogGlobal = false;

    [BCategory("Settings")]
    public bool categorySettings;

    [Tooltip("Choose if the fog settings are set on game start or updated in realtime for animation purposes.")]
    public FogUpdateMode updateMode = FogUpdateMode.OnLoad;

    [BCategory("Volume")]
    public bool categoryVolume;

    //public float volumeShapeFade = 1.0f;
    //public float volumeExitFade = 10.0f;
    public float volumeDistanceFade = 3.0f;
    [Range(0f, 1f)]
    public float volumeVisibility = 0.2f;

    [BCategory("Fog")]
    public bool categoryFog;

    [Tooltip("Shareable fog preset material.")]
    public Material fogPreset = null;
    [HideInInspector]
    public Material fogPresetOld = null;

    [StyledMessage("Info", "The is not a valid Fog Preset material! Please assign the correct shader first!", 10, 0)]
    public bool messageInvalidPreset = false;

    [Space(10)]
    [Range(0f, 1f)]
    public float fogIntensity = 1.0f;
#if UNITY_2018_1_OR_NEWER
    [ColorUsage(false, true)]
#else
    [ColorUsage(false, true, 0f, 8f, 0f, 1f)]
#endif
    public Color fogColor = new Color(0.0f, 1.0f, 0.0f, 1.0f);

    public float fogDistanceStart = 0.0f;
    public float fogDistanceEnd = 30.0f;
    public float fogHeightStart = 0.0f;
    public float fogHeightEnd = 5.0f;

    [BCategory("Skybox")]
    public bool categotySkybox;

    [Range(0f, 1f)]
    public float skyboxFogHeight = 0.5f;
    [Range(0f, 1f)]
    public float skyboxFogFill = 0.0f;

    [BCategory("Directional")]
    public bool categoryDirectional;

    public FogDirectionalMode directionalMode = FogDirectionalMode.Off;
    [Range(0f, 1f)]
    public float directionalIntensity = 1.0f;
#if UNITY_2018_1_OR_NEWER
    [ColorUsage(false, true)]
#else
    [ColorUsage(false, true, 0f, 8f, 0f, 1f)]
#endif
    public Color directionalColor = new Color(1f, 0.75f, 0.5f, 1f);

    [BCategory("Noise")]
    public bool categoryNoise;

    public FogNoiseMode noiseMode = FogNoiseMode.Off;
    [Range(0f, 1f)]
    public float noiseIntensity = 1.0f;
    public float noiseDistanceEnd = 60.0f;
    public float noiseScale = 1.0f;
    public Vector3 noiseSpeed = new Vector3(0.0f, 0.0f, 0.0f);

    [StyledSpace(5)]
    public bool styledSpace0;

    [HideInInspector]
    public bool firstTime = true;
    [HideInInspector]
    public bool upgradedTo100 = false;

    Material localMaterial;
    Collider volumeCollider;
    HeightFogGlobal globalFog = null;
    Camera cam;
    bool distanceSent = false;

    void Start()
    {
        if (firstTime == true)
        {
            gameObject.name = "Height Fog Override";
            firstTime = false;
        }

        volumeCollider = GetComponent<Collider>();
        volumeCollider.isTrigger = true;

        if (GameObject.Find("Height Fog Global") != null)
        {
            GameObject globalFogGO = GameObject.Find("Height Fog Global");
            globalFog = globalFogGO.GetComponent<HeightFogGlobal>();

            // Fix override Diectional and Noise blending
            // Upgrade from v0.9.0 to v1.0.0
            if (upgradedTo100 == false)
            {
                directionalMode = globalFog.directionalMode;
                noiseMode = globalFog.noiseMode;

                upgradedTo100 = true;
            }

            messageNoHeightFogGlobal = false;
        }
        else
        {
            messageNoHeightFogGlobal = true;
        }

        localMaterial = new Material(Shader.Find("BOXOPHOBIC/Atmospherics/Height Fog Preset"));
        localMaterial.name = "Local";

        SetLocalMaterial();
    }

    void LateUpdate()
    {
        GetCamera();

        if (cam == null || globalFog == null)
        {
            return;
        }

        Vector3 camPos = cam.transform.position;
        Vector3 closestPos = volumeCollider.ClosestPoint(camPos);

        float dist = Vector3.Distance(camPos, closestPos);

        if (dist > volumeDistanceFade && distanceSent == false)
        {
            globalFog.overrideCamToVolumeDistance = Mathf.Infinity;
            distanceSent = true;
        }
        else if (dist < volumeDistanceFade)
        {
            globalFog.overrideMaterial = localMaterial;
            globalFog.overrideCamToVolumeDistance = dist;
            globalFog.overrideVolumeDistanceFade = volumeDistanceFade;
            distanceSent = false;
        }

        // Only update the fog when in SceneMode or FogUpdateMode is set to Realtime
        if (Application.isPlaying == false || updateMode == FogUpdateMode.Realtime)
        {
            SetLocalMaterial();
        }

#if UNITY_EDITOR
        if (fogPreset != null)
        {
            if (fogPreset.HasProperty("_HeightFogPreset"))
            {
                if (fogPresetOld != fogPreset)
                {
                    SetPresetToScript();
                    SetLocalMaterial();

                    fogPresetOld = fogPreset;
                }

                if (Selection.Contains(gameObject))
                {
                    fogPreset.CopyPropertiesFromMaterial(localMaterial);
                    EditorUtility.SetDirty(fogPreset);
                }
                else
                {
                    SetPresetToScript();
                    SetLocalMaterial();
                }

                messageInvalidPreset = false;
            }
            else
            {
                messageInvalidPreset = true;
            }
        }
        else
        {
            messageInvalidPreset = false;
        }
#endif

    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(fogColor.r, fogColor.g, fogColor.b, volumeVisibility);
        Gizmos.DrawCube(transform.position, new Vector3(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z));
        Gizmos.DrawCube(transform.position, new Vector3(transform.lossyScale.x + (volumeDistanceFade * 2), transform.lossyScale.y + (volumeDistanceFade * 2), transform.lossyScale.z + (volumeDistanceFade * 2)));
    }

    void GetCamera()
    {
        cam = null;

        if (Camera.current != null)
        {
            cam = Camera.current;
        }

        if (Camera.main != null)
        {
            cam = Camera.main;
        }
    }

    void SetPresetToScript()
    {
        fogIntensity = fogPreset.GetFloat("_FogIntensity");

        fogColor = fogPreset.GetColor("_FogColor");
        fogDistanceStart = fogPreset.GetFloat("_FogDistanceStart");
        fogDistanceEnd = fogPreset.GetFloat("_FogDistanceEnd");
        fogHeightStart = fogPreset.GetFloat("_FogHeightStart");
        fogHeightEnd = fogPreset.GetFloat("_FogHeightEnd");

        skyboxFogHeight = fogPreset.GetFloat("_SkyboxFogHeight");
        skyboxFogFill = fogPreset.GetFloat("_SkyboxFogFill");

        directionalColor = fogPreset.GetColor("_DirectionalColor");
        directionalIntensity = fogPreset.GetFloat("_DirectionalIntensity");

        noiseIntensity = fogPreset.GetFloat("_NoiseIntensity");
        noiseDistanceEnd = fogPreset.GetFloat("_NoiseDistanceEnd");
        noiseScale = fogPreset.GetFloat("_NoiseScale");
        noiseSpeed = fogPreset.GetVector("_NoiseSpeed");

        if (fogPreset.GetInt("_DirectionalMode") == 1)
        {
            directionalMode = FogDirectionalMode.On;
        }
        else
        {
            directionalMode = FogDirectionalMode.Off;
        }

        if (fogPreset.GetInt("_NoiseMode") == 2)
        {
            noiseMode = FogNoiseMode.Procedural3D;
        }
        else
        {
            noiseMode = FogNoiseMode.Off;
        }
    }

    void SetLocalMaterial()
    {
        localMaterial.SetFloat("_FogIntensity", fogIntensity);

        localMaterial.SetColor("_FogColor", fogColor);
        localMaterial.SetFloat("_FogDistanceStart", fogDistanceStart);
        localMaterial.SetFloat("_FogDistanceEnd", fogDistanceEnd);
        localMaterial.SetFloat("_FogHeightStart", fogHeightStart);
        localMaterial.SetFloat("_FogHeightEnd", fogHeightEnd);

        localMaterial.SetFloat("_SkyboxFogHeight", skyboxFogHeight);
        localMaterial.SetFloat("_SkyboxFogFill", skyboxFogFill);

        localMaterial.SetFloat("_DirectionalIntensity", directionalIntensity);
        localMaterial.SetColor("_DirectionalColor", directionalColor);

        localMaterial.SetFloat("_NoiseIntensity", noiseIntensity);
        localMaterial.SetFloat("_NoiseDistanceEnd", noiseDistanceEnd);
        localMaterial.SetFloat("_NoiseScale", noiseScale);
        localMaterial.SetVector("_NoiseSpeed", noiseSpeed);

        if (directionalMode == FogDirectionalMode.On)
        {
            localMaterial.SetInt("_DirectionalMode", 1);
            localMaterial.SetFloat("_DirectionalModeBlend", 1.0f);
        }
        else
        {
            localMaterial.SetInt("_DirectionalMode", 0);
            localMaterial.SetFloat("_DirectionalModeBlend", 0.0f);
        }

        if (noiseMode == FogNoiseMode.Procedural3D)
        {
            localMaterial.SetInt("_NoiseMode", 2);
            localMaterial.SetFloat("_NoiseModeBlend", 1.0f);
        }
        else
        {
            localMaterial.SetInt("_NoiseMode", 0);
            localMaterial.SetFloat("_NoiseModeBlend", 0.0f);
        }
    }
}

