using UnityEditor;
using UnityEngine;

[UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyProperty))]
public class ReadOnlyDrawer : UnityEditor.PropertyDrawer
{
    public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false; // Отключаем редактирование поля
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true; // Включаем редактирование для остальных полей
    }
}
