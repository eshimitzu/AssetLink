using UnityEditor;
using UnityEngine;

public class DefaultPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        base.OnGUI(position, property, label);
        EditorGUI.PropertyField(position, property, label);

        // new Reflex("EditorGUI").Invoke("DefaultPropertyField", position, property, label);
        // UnityEngine.Debug.Log($"{property.propertyPath}");
    }
}
