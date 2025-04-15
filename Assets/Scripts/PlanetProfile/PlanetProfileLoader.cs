using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlanetProfileLoader : MonoBehaviour
{
    public static PlanetProfile activeProfile = null;
    
    public static PlanetProfileLoader Instance {get; private set;}
    
    public static bool finishedLoading {
        get{
            if(PlanetProfileLoader.Instance != null) { 
                return PlanetProfileLoader.Instance._finishedLoading;
            }
            return false;
        }
    }

    private bool _finishedLoading = false;

    private AstronomyDistanceFetcher astronomyFetcher;

    [SerializeField] PlanetProfile[] planetProfiles;
    void Awake() {
        if(Instance != null) {
            Destroy(this);
            return;
        } else {
            Instance = this;
        }
        astronomyFetcher = GetComponent<AstronomyDistanceFetcher>();
    }

    void Start() {
        loadCurrentPlanetProfile();
    }

    public void loadCurrentPlanetProfile() {
        StartCoroutine(loadCoroutine());
    }

    private IEnumerator loadCoroutine() {
        while(astronomyFetcher.planetName == null || astronomyFetcher.planetName == "") yield return null;
        for(int i = 0; i < planetProfiles.Length; i++) {
            if(planetProfiles[i].planetName.Equals(astronomyFetcher.planetName)) {
                if(activeProfile == null) {
                    planetProfiles[i].initialize(astronomyFetcher.GetPlanetDistance(planetProfiles[i].planetName));
                    activeProfile = planetProfiles[i];
                    Debug.Log($"Planet Profile {activeProfile.planetName} Loaded.");
                }
            }
        }
        _finishedLoading = true;
        yield break;
    }
    


}
