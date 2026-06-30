using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class NitrousEffectController : MonoBehaviour
{
    [SerializeField] private PostProcessVolume volume;
    private Vignette vignette;

    [Header("加速特效设置")]
    [SerializeField] private float targetVignetteIntensity = 0.6f; // 加速时的压暗程度
    [SerializeField] private float effectSpeed = 5f;

    void Start()
    {
        // 获取配置文件中的 Vignette 模块
        volume.profile.TryGetSettings(out vignette);
    }

    public void UpdateEffect(bool isBoosting)
    {
        if (vignette == null) return;

        // 根据是否加速，平滑改变边缘压暗程度
        float target = isBoosting ? targetVignetteIntensity : 0.2f;
        vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, target, Time.deltaTime * effectSpeed);
    }
}