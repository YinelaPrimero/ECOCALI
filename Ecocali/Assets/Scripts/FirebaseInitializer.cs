using UnityEngine;
using Firebase;
using Firebase.Extensions;
using System;

public class FirebaseInitializer : MonoBehaviour
{
    public static event Action OnFirebaseInitialized;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                // Firebase se inicializó correctamente
                Debug.Log("Firebase inicializado con éxito.");
                OnFirebaseInitialized?.Invoke();  // Notifica que Firebase ya está listo
            }
            else
            {
                Debug.LogError("No se pudo inicializar Firebase: " + task.Result);
            }
        });
    }
}