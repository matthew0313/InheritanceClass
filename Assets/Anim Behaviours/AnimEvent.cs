using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.UIElements;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class AnimEvent : StateMachineBehaviour
{
    [SerializeField] int eventIndex;
    [SerializeField] AnimEventSettings settings;
    AnimEventChannel channel;
    bool invoked = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (channel == null) channel = animator.GetComponent<AnimEventChannel>();
        invoked = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (channel == null || invoked) return;
        if (settings.timeMode == AnimEventTimeMode.RelativeTime)
        {
            if (stateInfo.normalizedTime >= settings.relativeTime)
            {
                channel.CallEvent(eventIndex);
                invoked = true;
            }
        }
        if (settings.timeMode == AnimEventTimeMode.Seconds)
        {
            if (stateInfo.normalizedTime * stateInfo.length >= settings.seconds)
            {
                channel.CallEvent(eventIndex);
                invoked = true;
            }
        }
        if (settings.timeMode == AnimEventTimeMode.Frames)
        {
            if (stateInfo.normalizedTime * stateInfo.length >= settings.frames / 60.0f)
            {
                channel.CallEvent(eventIndex);
                invoked = true;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!invoked)
        {
            channel.CallEvent(eventIndex);
            invoked = true;
        }
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
[System.Serializable]
public struct AnimEventSettings
{
    public AnimEventTimeMode timeMode;
    public float relativeTime;
    public float seconds;
    public float frames;
}
[System.Serializable]
public enum AnimEventTimeMode
{
    RelativeTime = 0,
    Seconds = 1,
    Frames = 2
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(AnimEventSettings))]
public class AnimEventSettings_Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("timeMode"));
        EditorGUI.PropertyField(position, property.FindPropertyRelative("timeMode"));
        position.y += position.height + 2;

        int enumIndex = property.FindPropertyRelative("timeMode").enumValueIndex;
        if(enumIndex == 0)
        {
            EditorGUI.PropertyField(position, property.FindPropertyRelative("relativeTime"));
        }
        if(enumIndex == 1)
        {
            position.height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Float, new GUIContent() { text = "Seconds" });
            float tmp = EditorGUI.FloatField(position, new GUIContent() { text = "Seconds" }, property.FindPropertyRelative("seconds").floatValue);
            property.FindPropertyRelative("seconds").floatValue = tmp;
            property.FindPropertyRelative("frames").floatValue = tmp * 60.0f;
        }
        if(enumIndex == 2)
        {
            position.height = EditorGUI.GetPropertyHeight(SerializedPropertyType.Float, new GUIContent() { text = "Frames" });
            float tmp = EditorGUI.FloatField(position, new GUIContent() { text = "Frames" }, property.FindPropertyRelative("frames").floatValue);
            property.FindPropertyRelative("frames").floatValue = tmp;
            property.FindPropertyRelative("seconds").floatValue = tmp / 60.0f;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = 0.0f;
        height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("timeMode")) + 2;
        switch (property.FindPropertyRelative("timeMode").enumValueIndex)
        {
            case 0: height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("relativeTime")) + 2; break;
            case 1: height += EditorGUI.GetPropertyHeight(SerializedPropertyType.Float, new() { text = "Seconds" }); break;
            case 2: height += EditorGUI.GetPropertyHeight(SerializedPropertyType.Float, new() { text = "Frames" }); break;
        }
        return height;
    }
}
#endif