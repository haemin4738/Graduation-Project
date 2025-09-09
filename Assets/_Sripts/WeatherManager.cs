using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI 관련 작업을 위해 필요

public class WeatherManager : MonoBehaviour
{
    public enum Weather { SUNNY, RAIN };
    public Weather currentWeather;
    public ParticleSystem rain;
    public float weather_time = 10f; // 날씨 바뀌는 간격
    public int next_weather; //랜덤하게 다음 날씨 지정
    public int current_weather;
    public AudioSource audioSource;

    public Image screenOverlay;
    public float fadeSpeed = 1f;


    void Start()
    {
        currentWeather = Weather.RAIN; //시작은 비오는 날씨
        next_weather = Random.Range(0, 2); // 다음 날씨
        current_weather = 1;
        audioSource = GetComponentInChildren<AudioSource>();
    }

    public void ChangeWeather(Weather weatherType)
    {
        if (weatherType != this.currentWeather)
        {
            switch (weatherType)
            {
                case Weather.SUNNY:
                    currentWeather = Weather.SUNNY;
                    this.rain.Stop();
                    break;
                case Weather.RAIN:
                    currentWeather = Weather.RAIN;
                    this.rain.Play();
                    break;
            }
        }
        //매개변수로 받은 날씨가 현재 날씨와 같지 않다면 매개변수로 받은 날씨로 변경해준다. 
    }

    void Update()
    {
        this.weather_time -= Time.deltaTime; //10초동안은 그 날씨 유지
        if (next_weather == 1) //다음 날씨가 '비'이고
        {
            if (this.weather_time <= 0) //현재 날씨의 제한시간이 끝나면
            {
                next_weather = Random.Range(0, 2); //다음 날씨 계산(0 - 맑음, 1 - 비)
                ChangeWeather(Weather.RAIN); //비으로 바꿔줌
                weather_time = 10f;
                current_weather = 1;

            }
        }
        if (next_weather == 0) //다음 날씨가 '맑음'이고
        {
            if (this.weather_time <= 0) //현재 날씨의 제한시간이 끝나면
            {
                next_weather = Random.Range(0, 2); //다음 날씨 계산(0 - 맑음, 1 - 비)
                ChangeWeather(Weather.SUNNY); //맑음으로 바꿔줌
                weather_time = 10f;
                current_weather = 0;

            }
        }
        ControlAudioBasedOnWeather();
        ControlScreenBrightness();

    }
    void ControlAudioBasedOnWeather()
    {
        // current_weather가 1이면 오디오 재생
        if (current_weather == 1 && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        // current_weather가 0이면 오디오 중지
        else if (current_weather == 0 && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
    void ControlScreenBrightness()
    {
        // current_weather 값에 따라 화면 밝기 조절
        if (current_weather == 1)
        {
            // 화면을 점점 어두워지게 함
            screenOverlay.color = Color.Lerp(screenOverlay.color, new Color(0, 0, 0, 0.5f), fadeSpeed * Time.deltaTime);
        }
        else if (current_weather == 0)
        {
            // 화면을 점점 밝아지게 함
            screenOverlay.color = Color.Lerp(screenOverlay.color, new Color(0, 0, 0, 0), fadeSpeed * Time.deltaTime);
        }

    }
}