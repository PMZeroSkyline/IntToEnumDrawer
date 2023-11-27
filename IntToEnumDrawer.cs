using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class IntToEnumAttribute : PropertyAttribute
{
    public Type EnumType { get; }

    public IntToEnumAttribute(Type enumType)
    {
        EnumType = enumType;
    }
}

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class DescriptionAttribute : PropertyAttribute
{
    public string Description { get; }

    public DescriptionAttribute(string description)
    {
        Description = description;
    }
}
[CustomPropertyDrawer(typeof(IntToEnumAttribute))]
public class IntToEnumDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        System.Type enumType = fieldInfo.FieldType;
        IntToEnumAttribute enumAttribute = attribute as IntToEnumAttribute;

        if (enumAttribute != null)
        {
            enumType = enumAttribute.EnumType;
        }
        
        if (enumType == null || !enumType.IsEnum)
        {
            throw new ArgumentException("Input type must be an enum");
        }

        List<string> descriptions = new List<string>();

        FieldInfo[] fields = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);

        foreach (FieldInfo field in fields)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                descriptions.Add(attributes[0].Description);
            }
            else
            {
                descriptions.Add(field.Name); // Fallback to field name if no DescriptionAttribute is found
            }
        }

        property.intValue = EditorGUI.Popup(position, label.text, property.intValue, descriptions.ToArray());

        EditorGUI.EndProperty();
    }
}