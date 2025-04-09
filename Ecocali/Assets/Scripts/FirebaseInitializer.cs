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
                // Firebase se inicializ� correctamente
                Debug.Log("Firebase inicializado con �xito.");
                OnFirebaseInitialized?.Invoke();  // Notifica que Firebase ya est� listo
            }
            else
            {
                Debug.LogError("No se pudo inicializar Firebase: " + task.Result);
            }
        });
    }
}