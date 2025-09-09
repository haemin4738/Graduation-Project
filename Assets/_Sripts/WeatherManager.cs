using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI ���� �۾��� ���� �ʿ�

public class WeatherManager : MonoBehaviour
{
    public enum Weather { SUNNY, RAIN };
    public Weather currentWeather;
    public ParticleSystem rain;
    public float weather_time = 10f; // ���� �ٲ�� ����
    public int next_weather; //�����ϰ� ���� ���� ����
    public int current_weather;
    public AudioSource audioSource;

    public Image screenOverlay;
    public float fadeSpeed = 1f;


    void Start()
    {
        currentWeather = Weather.RAIN; //������ ����� ����
        next_weather = Random.Range(0, 2); // ���� ����
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
        //�Ű������� ���� ������ ���� ������ ���� �ʴٸ� �Ű������� ���� ������ �������ش�. 
    }

    void Update()
    {
        this.weather_time -= Time.deltaTime; //10�ʵ����� �� ���� ����
        if (next_weather == 1) //���� ������ '��'�̰�
        {
            if (this.weather_time <= 0) //���� ������ ���ѽð��� ������
            {
                next_weather = Random.Range(0, 2); //���� ���� ���(0 - ����, 1 - ��)
                ChangeWeather(Weather.RAIN); //������ �ٲ���
                weather_time = 10f;
                current_weather = 1;

            }
        }
        if (next_weather == 0) //���� ������ '����'�̰�
        {
            if (this.weather_time <= 0) //���� ������ ���ѽð��� ������
            {
                next_weather = Random.Range(0, 2); //���� ���� ���(0 - ����, 1 - ��)
                ChangeWeather(Weather.SUNNY); //�������� �ٲ���
                weather_time = 10f;
                current_weather = 0;

            }
        }
        ControlAudioBasedOnWeather();
        ControlScreenBrightness();

    }
    void ControlAudioBasedOnWeather()
    {
        // current_weather�� 1�̸� ����� ���
        if (current_weather == 1 && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
        // current_weather�� 0�̸� ����� ����
        else if (current_weather == 0 && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
    void ControlScreenBrightness()
    {
        // current_weather ���� ���� ȭ�� ��� ����
        if (current_weather == 1)
        {
            // ȭ���� ���� ��ο����� ��
            screenOverlay.color = Color.Lerp(screenOverlay.color, new Color(0, 0, 0, 0.5f), fadeSpeed * Time.deltaTime);
        }
        else if (current_weather == 0)
        {
            // ȭ���� ���� ������� ��
            screenOverlay.color = Color.Lerp(screenOverlay.color, new Color(0, 0, 0, 0), fadeSpeed * Time.deltaTime);
        }

    }
}