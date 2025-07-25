using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.Events;

public class AuthManager : MonoBehaviour
{
    public static FirebaseAuth auth;
    public static FirebaseUser user;

    public void Login()
    {
        if(user == null)
            InitializeFirebase();
    }

    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                SignInAnonymously();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");
            }
        });
    }

    public UnityEvent onLogin;

    void SignInAnonymously()
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("Anonymous sign-in was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("Anonymous sign-in encountered an error: " + task.Exception);
                return;
            }

            user = task.Result.User;
            Debug.LogFormat("User signed in anonymously with UserId: {0}", user.UserId);
            onLogin.Invoke();
        });
    }
}
