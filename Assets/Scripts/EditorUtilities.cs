using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
public static class EditorUtilities
{
    static AudioSource testSource = null;
    static void PlayClip(AudioClip clip)
    {
        if (testSource == null)
        {
            testSource = new GameObject() { hideFlags = HideFlags.HideAndDontSave }.AddComponent<AudioSource>();
            testSource.playOnAwake = false;
        }
        testSource.clip = clip;
        testSource.Play();
    }
    [MenuItem("Assets/Audio/Test Play Clip")]
    static void TestPlayClip()
    {
        if(Selection.activeObject is AudioClip) PlayClip(Selection.activeObject as AudioClip);
    }
}
#endif