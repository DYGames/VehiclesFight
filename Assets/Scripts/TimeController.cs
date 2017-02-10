using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TimeController : NetworkBehaviour
{
    public static TimeController instance;

    public Material[] skyboxs;
    public Light directinallight;
    [SyncVar]
    public float CurrentTime;
    [SyncVar]
    public bool isNextDayForWaveMng;
    public UnityEngine.UI.Text TimeText;

    void Awake()
    {
        instance = this;
        isNextDayForWaveMng = false;
    }

    void Start()
    {
        RenderSettings.fog = true;
    }

    void Update()
    {
        string hours = ((int)CurrentTime).ToString();
        string mins = ((int)((CurrentTime - (int)CurrentTime) * 60)).ToString();
        TimeText.text = string.Format("{0} : {1}", hours, mins);

        if (CurrentTime > 8 && CurrentTime < 16)
        {
            RenderSettings.skybox = skyboxs[0];
            RenderSettings.fogDensity = 0;
            RenderSettings.ambientSkyColor = new Color(0.5f, 0.5f, 0.5f);
            directinallight.color = new Color(1, 1, 1);
        }
        else if (CurrentTime > 16 && CurrentTime < 20)
        {
            RenderSettings.skybox = skyboxs[1];
            RenderSettings.fogDensity = 0.1f * ((CurrentTime - 16.0f) / 4.0f);
            float a = ((4 - (CurrentTime - 16.0f)) / 4.0f);
            directinallight.color = new Color(a * (9 / 10), a * (9 / 10), a * (9 / 10));
            RenderSettings.ambientSkyColor = new Color(a, a, a);
        }
        else if (CurrentTime > 20 || CurrentTime < 5)
        {
            RenderSettings.skybox = skyboxs[2];
            RenderSettings.fogDensity = 0.1f;
            RenderSettings.ambientSkyColor = new Color();
            directinallight.color = new Color(0.1f, 0.1f, 0.1f);
        }
        else if (CurrentTime > 5 && CurrentTime < 8)
        {
            RenderSettings.skybox = skyboxs[3];
            RenderSettings.fogDensity = 0.1f * ((3.0f - (CurrentTime - 5.0f)) / 3.0f);
            float a = (((CurrentTime - 5.0f)) / 3.0f);
            directinallight.color = new Color(a, a, a);
            RenderSettings.ambientSkyColor = new Color(a, a, a);
        }
    }
}
