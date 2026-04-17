using UnityEngine;
using UnityEditor;
using System;

namespace NaughtyAttributes.Editor
{
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerPropertyDrawer : PropertyDrawerBase
    {
        private const string TypeWarningMessage = "{0} must be an int or a string";
        private const string FlagsTypeWarningMessage = "{0} must be an int when useFlags is true";

        protected override float GetPropertyHeight_Internal(SerializedProperty property, GUIContent label)
        {
            bool useFlags = ((LayerAttribute)attribute).UseFlags;
            bool validPropertyType = useFlags
                ? property.propertyType == SerializedPropertyType.Integer
                : property.propertyType == SerializedPropertyType.String || property.propertyType == SerializedPropertyType.Integer;

            return validPropertyType
                ? GetPropertyHeight(property)
                : GetPropertyHeight(property) + GetHelpBoxHeight();
        }

        protected override void OnGUI_Internal(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);

            bool useFlags = ((LayerAttribute)attribute).UseFlags;
            string[] layers = GetLayers();

            if (useFlags)
            {
                if (property.propertyType == SerializedPropertyType.Integer)
                {
                    DrawPropertyForIntFlags(rect, property, label, layers);
                }
                else
                {
                    string message = string.Format(FlagsTypeWarningMessage, property.name);
                    DrawDefaultPropertyAndHelpBox(rect, property, message, MessageType.Warning);
                }
            }
            else
            {
                switch (property.propertyType)
                {
                    case SerializedPropertyType.String:
                        DrawPropertyForString(rect, property, label, layers);
                        break;
                    case SerializedPropertyType.Integer:
                        DrawPropertyForInt(rect, property, label, layers);
                        break;
                    default:
                        string message = string.Format(TypeWarningMessage, property.name);
                        DrawDefaultPropertyAndHelpBox(rect, property, message, MessageType.Warning);
                        break;
                }
            }

            EditorGUI.EndProperty();
        }

        private string[] GetLayers()
        {
            return UnityEditorInternal.InternalEditorUtility.layers;
        }

        private static void DrawPropertyForString(Rect rect, SerializedProperty property, GUIContent label, string[] layers)
        {
            int index = IndexOf(layers, property.stringValue);
            int newIndex = EditorGUI.Popup(rect, label.text, index, layers);
            string newLayer = layers[newIndex];

            if (!property.stringValue.Equals(newLayer, StringComparison.Ordinal))
            {
                property.stringValue = layers[newIndex];
            }
        }

        private static void DrawPropertyForInt(Rect rect, SerializedProperty property, GUIContent label, string[] layers)
        {
            int index = 0;
            string layerName = LayerMask.LayerToName(property.intValue);
            for (int i = 0; i < layers.Length; i++)
            {
                if (layerName.Equals(layers[i], StringComparison.Ordinal))
                {
                    index = i;
                    break;
                }
            }

            int newIndex = EditorGUI.Popup(rect, label.text, index, layers);
            string newLayerName = layers[newIndex];
            int newLayerNumber = LayerMask.NameToLayer(newLayerName);

            if (property.intValue != newLayerNumber)
            {
                property.intValue = newLayerNumber;
            }
        }

        private static void DrawPropertyForIntFlags(Rect rect, SerializedProperty property, GUIContent label, string[] layers)
        {
            int maskFieldValue = LayerMaskToMaskFieldMask(property.intValue, layers);
            int newMaskFieldValue = EditorGUI.MaskField(rect, label.text, maskFieldValue, layers);

            if (maskFieldValue != newMaskFieldValue)
            {
                property.intValue = MaskFieldMaskToLayerMask(newMaskFieldValue, layers);
            }
        }

        // MaskField uses bit indices matching the layers array, but LayerMask bits map to actual layer indices (0-31).
        private static int LayerMaskToMaskFieldMask(int layerMask, string[] layers)
        {
            if (layerMask == 0) return 0;
            if (layerMask == -1) return -1;

            int maskFieldMask = 0;
            for (int i = 0; i < layers.Length; i++)
            {
                int layerIndex = LayerMask.NameToLayer(layers[i]);
                if ((layerMask & (1 << layerIndex)) != 0)
                {
                    maskFieldMask |= (1 << i);
                }
            }
            return maskFieldMask;
        }

        private static int MaskFieldMaskToLayerMask(int maskFieldMask, string[] layers)
        {
            if (maskFieldMask == 0) return 0;
            if (maskFieldMask == -1) return -1;

            int layerMask = 0;
            for (int i = 0; i < layers.Length; i++)
            {
                if ((maskFieldMask & (1 << i)) != 0)
                {
                    int layerIndex = LayerMask.NameToLayer(layers[i]);
                    layerMask |= (1 << layerIndex);
                }
            }
            return layerMask;
        }

        private static int IndexOf(string[] layers, string layer)
        {
            var index = Array.IndexOf(layers, layer);
            return Mathf.Clamp(index, 0, layers.Length - 1);
        }
    }
}
