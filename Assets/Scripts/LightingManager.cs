using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingManager : MonoBehaviour
{
    public static LightingManager Instance {get; private set;}
    public void Awake() {
        if(Instance != null) {
            Destroy(this);
            return;
        } else {
            Instance = this;
        }
    }

    public void Start() {
        StartCoroutine(ApplyLightSettings());
    }
    IEnumerator ApplyLightSettings() {
        while(!PlanetProfileLoader.finishedLoading) yield return null;
        RenderSettings.fogColor = PlanetProfileLoader.activeProfile.renderSettings.fogColor;
        yield break;
    }
}
