using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPSService : MonoBehaviour
{
    public static GPSService Instance { get; private set; } // 싱글톤 인스턴스
    public Text textMsg;

    public InputField Latitude;
    public InputField Longitude;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Start()
    {
        if(!Input.location.isEnabledByUser)
        {
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while(Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if(maxWait < 1)
        {
            textMsg.text = "시간초과 : " + maxWait;
            yield break;
        }

        if(Input.location.status == LocationServiceStatus.Failed)
        {
            textMsg.text = "장치 위치를 확인할 수 없습니다.";
        }
        else
        {
            while(true)
            {
                textMsg.text = "Current Location: "
                    + "latitude: " + Input.location.lastData.latitude
                    + "longitude: " + Input.location.lastData.longitude
                    + "horizontal: " + Input.location.lastData.horizontalAccuracy;

                Latitude.text = Input.location.lastData.latitude.ToString();
                Longitude.text = Input.location.lastData.longitude.ToString();

                yield return new WaitForSeconds(1);

                // Input.location.Stop();
            }
        }
    }
}
