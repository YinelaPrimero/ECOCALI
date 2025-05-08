using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.IO.Ports;

public class ArduinoVideoController : MonoBehaviour
{
    SerialPort puerto;
    public VideoPlayer videoPlayer;
    public VideoClip[] clips;
    public RawImage pantalla;

    void Start()
    {
        pantalla.gameObject.SetActive(false); //  Desactivamos la pantalla al inicio

        try
        {
            puerto = new SerialPort("COM10", 9600); //  COM10
            puerto.Open();
            puerto.ReadTimeout = 100;
            Debug.Log("Puerto COM10 abierto correctamente.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al abrir el puerto: " + e.Message);
        }

        videoPlayer.loopPointReached += OnVideoEnd; //  Escuchar cuando el video termina (opcional)
    }

    void Update()
    {
        if (puerto != null && puerto.IsOpen)
        {
            try
            {
                string mensaje = puerto.ReadLine().Trim();
                Debug.Log("Arduino dice: " + mensaje);

                switch (mensaje)
                {
                    case "VIDEO_1":
                        ReproducirVideo(0);
                        break;
                    case "VIDEO_2":
                        ReproducirVideo(1);
                        break;
                    case "VIDEO_3":
                        ReproducirVideo(2);
                        break;
                    case "VIDEO_4":
                        ReproducirVideo(3);
                        break;
                    case "VIDEO_5":
                        ReproducirVideo(4);
                        break;
                    default:
                        Debug.LogWarning("Mensaje desconocido: " + mensaje);
                        break;
                }
            }
            catch (System.TimeoutException)
            {
                // No pasa nada, esperamos más datos
            }
        }
    }

    void ReproducirVideo(int index)
    {
        if (index >= 0 && index < clips.Length)
        {
            videoPlayer.Stop();
            videoPlayer.clip = clips[index];
            pantalla.gameObject.SetActive(true); //  Activamos RawImage
            videoPlayer.Play();
            Debug.Log("Reproduciendo video " + (index + 1));
        }
        else
        {
            Debug.LogError("Índice de video fuera de rango: " + index);
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        pantalla.gameObject.SetActive(false); //  Opción para ocultar el RawImage al finalizar
        Debug.Log("Video terminado. Ocultando pantalla.");
    }

    void OnApplicationQuit()
    {
        if (puerto != null && puerto.IsOpen)
        {
            puerto.Close();
            Debug.Log("Puerto cerrado.");
        }
    }
}