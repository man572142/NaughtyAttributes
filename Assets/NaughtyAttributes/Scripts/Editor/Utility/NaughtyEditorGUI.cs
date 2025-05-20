using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NaughtyAttributes.Editor
{
    public static class NaughtyEditorGUI
    {
        public struct FieldButtonScope : IDisposable
        {
            private SpecialCaseDrawerAttribute[] _attributes;
            private SerializedProperty _property;

            public FieldButtonScope(SpecialCaseDrawerAttribute[] specialCaseAttributes, SerializedProperty property)
            {
                _attributes = specialCaseAttributes ?? Array.Empty<SpecialCaseDrawerAttribute>();
                _property = property;

                bool hasHorizontalBegan = false;
                for (int i = 0; i < _attributes.Length; i++)
                {
                    if (!(_attributes[i] is ButtonAttribute button))
                    {
                        continue;
                    }

                    switch (button.DisplayOptions.GetPosition())
                    {
                        case DisplayOptions.OnTop:
                            FieldButton(button, _property);
                            break;
                        case DisplayOptions.AlongSide:
                            if(!hasHorizontalBegan)
                            {
                                EditorGUILayout.BeginHorizontal();
                                hasHorizontalBegan = true;
                            }
                            break;
                    }
                }
            }

            public void Dispose()
            {
                bool hasHorizontalEnded = false;
                for (int i = 0; i < _attributes.Length; i++)
                {
                    if (!(_attributes[i] is ButtonAttribute button))
                    {
                        continue;
                    }

                    switch (button.DisplayOptions.GetPosition())
                    {
                        case DisplayOptions.AlongSide:
                            FieldButton(button, _property);
                            if (!hasHorizontalEnded)
                            {
                                EditorGUILayout.EndHorizontal();
                                hasHorizontalEnded = true;
                            }
                            break;
                        case DisplayOptions.AtBottom:
                            FieldButton(button, _property);
                            break;
                    }
                }
            }
        }

        public struct CenterScope : IDisposable
        {
            private bool _isCenter;

            public CenterScope(bool isCenter)
            {
                _isCenter = isCenter;

                if(_isCenter)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                }
            }

            public void Dispose()
            {
                if (_isCenter)
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        public const float IndentLength = 15.0f;
        public const float HorizontalSpacing = 2.0f;

        private static GUIStyle _buttonStyle = new GUIStyle(GUI.skin.button) { richText = true };

        private delegate void PropertyFieldFunction(Rect rect, SerializedProperty property, GUIContent label, bool includeChildren);

        public static void PropertyField(Rect rect, SerializedProperty property, bool includeChildren)
        {
            PropertyField_Implementation(rect, property, includeChildren, DrawPropertyField);
        }

        public static void PropertyField_Layout(SerializedProperty property, bool includeChildren)
        {
            Rect dummyRect = new Rect();
            PropertyField_Implementation(dummyRect, property, includeChildren, DrawPropertyField_Layout);
        }

        private static void DrawPropertyField(Rect rect, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            EditorGUI.PropertyField(rect, property, label, includeChildren);
        }

        private static void DrawPropertyField_Layout(Rect rect, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            EditorGUILayout.PropertyField(property, label, includeChildren);
        }

        private static void PropertyField_Implementation(Rect rect, SerializedProperty property, bool includeChildren, PropertyFieldFunction propertyFieldFunction)
        {
            SpecialCaseDrawerAttribute[] specialCaseAttributes = PropertyUtility.GetAttributes<SpecialCaseDrawerAttribute>(property);
            if (specialCaseAttributes != null)
            {
                foreach(var specialCaseAttr in specialCaseAttributes)
                {
                    if(!(specialCaseAttr is ButtonAttribute))
                    {
                        specialCaseAttr.GetDrawer().OnGUI(rect, property);
                        return;
                    }
                }
            }

            // Check if visible
            bool visible = PropertyUtility.IsVisible(property);
            if (!visible)
            {
                return;
            }

            // Validate
            ValidatorAttribute[] validatorAttributes = PropertyUtility.GetAttributes<ValidatorAttribute>(property);
            foreach (var validatorAttribute in validatorAttributes)
            {
                validatorAttribute.GetValidator().ValidateProperty(property);
            }

            // Check if enabled and draw
            EditorGUI.BeginChangeCheck();
            bool enabled = PropertyUtility.IsEnabled(property);

            using (new EditorGUI.DisabledScope(disabled: !enabled))
            using (new FieldButtonScope(specialCaseAttributes, property))
            {
                propertyFieldFunction.Invoke(rect, property, PropertyUtility.GetLabel(property), includeChildren);
            }

            // Call OnValueChanged callbacks
            if (EditorGUI.EndChangeCheck())
            {
                PropertyUtility.CallOnValueChangedCallbacks(property);
            }
        }

        public static float GetIndentLength(Rect sourceRect)
        {
            Rect indentRect = EditorGUI.IndentedRect(sourceRect);
            float indentLength = indentRect.x - sourceRect.x;

            return indentLength;
        }

        public static void BeginBoxGroup_Layout(string label = "")
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            if (!string.IsNullOrEmpty(label))
            {
                EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
            }
        }

        public static void EndBoxGroup_Layout()
        {
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Creates a dropdown
        /// </summary>
        /// <param name="rect">The rect the defines the position and size of the dropdown in the inspector</param>
        /// <param name="serializedObject">The serialized object that is being updated</param>
        /// <param name="target">The target object that contains the dropdown</param>
        /// <param name="dropdownField">The field of the target object that holds the currently selected dropdown value</param>
        /// <param name="label">The label of the dropdown</param>
        /// <param name="selectedValueIndex">The index of the value from the values array</param>
        /// <param name="values">The values of the dropdown</param>
        /// <param name="displayOptions">The display options for the values</param>
        public static void Dropdown(
            Rect rect, SerializedObject serializedObject, object target, FieldInfo dropdownField,
            string label, int selectedValueIndex, object[] values, string[] displayOptions)
        {
            EditorGUI.BeginChangeCheck();

            int newIndex = EditorGUI.Popup(rect, label, selectedValueIndex, displayOptions);
            object newValue = values[newIndex];

            object dropdownValue = dropdownField.GetValue(target);
            if (dropdownValue == null || !dropdownValue.Equals(newValue))
            {
                Undo.RecordObject(serializedObject.targetObject, "Dropdown");

                // TODO: Problem with structs, because they are value type.
                // The solution is to make boxing/unboxing but unfortunately I don't know the compile time type of the target object
                dropdownField.SetValue(target, newValue);
            }
        }

        public static void Button(UnityEngine.Object target, MethodInfo methodInfo, DisplayOptions specifiedOption)
        {
            bool visible = ButtonUtility.IsVisible(target, methodInfo);
            if (!visible)
            {
                return;
            }

            var buttons = methodInfo.GetCustomAttributes(typeof(ButtonAttribute), true);
            foreach (ButtonAttribute button in buttons)
            {
                Button(target, methodInfo, specifiedOption, button);
            }
        }

        private static void Button(UnityEngine.Object target, MethodInfo methodInfo, DisplayOptions specifiedOption, ButtonAttribute buttonAttribute)
        {
            if (!buttonAttribute.DisplayOptions.Has(specifiedOption))
            {
                return;
            }

            buttonAttribute.Text = string.IsNullOrEmpty(buttonAttribute.Text) ? ObjectNames.NicifyVariableName(methodInfo.Name) : buttonAttribute.Text;
            bool buttonEnabled = ButtonUtility.IsEnabled(target, methodInfo);

            EvaluateButtonEnable(ref buttonEnabled, buttonAttribute.SelectedEnableMode);

            bool methodIsCoroutine = methodInfo.ReturnType == typeof(IEnumerator);
            if (methodIsCoroutine)
            {
                buttonEnabled &= (Application.isPlaying ? true : false);
            }

            EditorGUI.BeginDisabledGroup(!buttonEnabled);
            if (SizableButton(buttonAttribute))
            {
                ExecuteButtonMethod(target, methodInfo, buttonAttribute.GetUsableParameters(methodInfo));
            }
            EditorGUI.EndDisabledGroup();
        }

        // TODO: Doesn't work with ExpandableAttribute
        public static void FieldButton(ButtonAttribute buttonAttribute, SerializedProperty property)
        {
            if (string.IsNullOrEmpty(buttonAttribute.Method))
            {
                EditorGUILayout.HelpBox($"Failed to display the {buttonAttribute.Text} button! \nTo use the [ButtonAttribute] on a field, you must provide the method name.", MessageType.Error);
            }
            else if (SizableButton(buttonAttribute))
            {
                Type targetObjType = property.serializedObject.targetObject.GetType();
                var methodInfo = targetObjType
                    .GetMethod(buttonAttribute.Method, BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);

                if (methodInfo != null)
                {
                    ExecuteButtonMethod(property.serializedObject.targetObject, methodInfo, buttonAttribute.GetUsableParameters(methodInfo, property));
                }
                else
                {
                    Debug.LogError($"Can't find the method:{buttonAttribute.Method}!");
                }
            }
        }

        public static bool SizableButton(ButtonAttribute button)
        {
            if(button.DisplayOptions.GetPosition() == DisplayOptions.AlongSide)
            {
                return GUILayout.Button(button.Text, _buttonStyle);
            }

            var widthOption = button.DisplayOptions.GetWidthOption();
            var heightOption = button.DisplayOptions.GetHeightOption();

            bool isButtonClicked = false;
            using (new CenterScope(widthOption != null))
            {
                int optionsCount = widthOption != null ? 1 : 0;
                optionsCount += heightOption != null ? 1 : 0;
                isButtonClicked = optionsCount switch
                {
                    1 => GUILayout.Button(button.Text, _buttonStyle, widthOption ?? heightOption),
                    2 => GUILayout.Button(button.Text, _buttonStyle, widthOption, heightOption),
                    _ => GUILayout.Button(button.Text, _buttonStyle),
                }; 
            }
            return isButtonClicked;
        }

        private static object[] GetUsableParameters(this ButtonAttribute buttonAttribute, MethodInfo methodInfo, SerializedProperty property = null)
        {
            object[] args = buttonAttribute.Args;
            var parameters = methodInfo.GetParameters();
            if(args == null || args.Length != parameters.Length)
            {
                int originalLength = buttonAttribute.Args != null ? buttonAttribute.Args.Length : 0;
                args = new object[parameters.Length];
                for(int i = 0; i < parameters.Length;i++)
                {
                    if(i < originalLength)
                    {
                        args[i] = buttonAttribute.Args[i];
                    }
                    else if (parameters[i].HasDefaultValue)
                    {
                        args[i] = parameters[i].DefaultValue;
                    }
                    else if(property != null && i == 0)
                    {
                        args[i] = property.boxedValue;
                    }
                    else
                    {
                        throw new ArgumentException($"Unmatched parameters length! TargetMethod:{parameters?.Length} Button:{buttonAttribute.Args?.Length}");
                    }
                }
            }
            return args;
        }

        private static void ExecuteButtonMethod(UnityEngine.Object target, MethodInfo methodInfo, object[] args)
        {
            IEnumerator methodResult = methodInfo.Invoke(target, args) as IEnumerator;

            if (!Application.isPlaying)
            {
                // Set target object and scene dirty to serialize changes to disk
                EditorUtility.SetDirty(target);

                PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();
                if (stage != null)
                {
                    // Prefab mode
                    EditorSceneManager.MarkSceneDirty(stage.scene);
                }
                else
                {
                    // Normal scene
                    EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                }
            }
            else if (methodResult != null && target is MonoBehaviour behaviour)
            {
                behaviour.StartCoroutine(methodResult);
            }
        }

        private static void EvaluateButtonEnable(ref bool buttonEnabled, EButtonEnableMode mode)
        {
            buttonEnabled &=
                mode == EButtonEnableMode.Always ||
                mode == EButtonEnableMode.Editor && !Application.isPlaying ||
                mode == EButtonEnableMode.Playmode && Application.isPlaying;
        }

        public static void NativeProperty_Layout(UnityEngine.Object target, PropertyInfo property)
        {
            object value = property.GetValue(target, null);

            if (value == null)
            {
                string warning = string.Format("{0} is null. {1} doesn't support reference types with null value", ObjectNames.NicifyVariableName(property.Name), typeof(ShowNativePropertyAttribute).Name);
                HelpBox_Layout(warning, MessageType.Warning, context: target);
            }
            else if (!Field_Layout(value, ObjectNames.NicifyVariableName(property.Name)))
            {
                string warning = string.Format("{0} doesn't support {1} types", typeof(ShowNativePropertyAttribute).Name, property.PropertyType.Name);
                HelpBox_Layout(warning, MessageType.Warning, context: target);
            }
        }

        public static void NonSerializedField_Layout(UnityEngine.Object target, FieldInfo field)
        {
            object value = field.GetValue(target);

            if (value == null)
            {
                string warning = string.Format("{0} is null. {1} doesn't support reference types with null value", ObjectNames.NicifyVariableName(field.Name), typeof(ShowNonSerializedFieldAttribute).Name);
                HelpBox_Layout(warning, MessageType.Warning, context: target);
            }
            else if (!Field_Layout(value, ObjectNames.NicifyVariableName(field.Name)))
            {
                string warning = string.Format("{0} doesn't support {1} types", typeof(ShowNonSerializedFieldAttribute).Name, field.FieldType.Name);
                HelpBox_Layout(warning, MessageType.Warning, context: target);
            }
        }

        public static void HorizontalLine(Rect rect, float height, Color color)
        {
            rect.height = height;
            EditorGUI.DrawRect(rect, color);
        }

        public static void HelpBox(Rect rect, string message, MessageType type, UnityEngine.Object context = null, bool logToConsole = false)
        {
            EditorGUI.HelpBox(rect, message, type);

            if (logToConsole)
            {
                DebugLogMessage(message, type, context);
            }
        }

        public static void HelpBox_Layout(string message, MessageType type, UnityEngine.Object context = null, bool logToConsole = false)
        {
            EditorGUILayout.HelpBox(message, type);

            if (logToConsole)
            {
                DebugLogMessage(message, type, context);
            }
        }

        public static bool Field_Layout(object value, string label)
        {
            using (new EditorGUI.DisabledScope(disabled: true))
            {
                bool isDrawn = true;
                Type valueType = value.GetType();

                if (valueType == typeof(bool))
                {
                    EditorGUILayout.Toggle(label, (bool)value);
                }
                else if (valueType == typeof(short))
                {
                    EditorGUILayout.IntField(label, (short)value);
                }
                else if (valueType == typeof(ushort))
                {
                    EditorGUILayout.IntField(label, (ushort)value);
                }
                else if (valueType == typeof(int))
                {
                    EditorGUILayout.IntField(label, (int)value);
                }
                else if (valueType == typeof(uint))
                {
                    EditorGUILayout.LongField(label, (uint)value);
                }
                else if (valueType == typeof(long))
                {
                    EditorGUILayout.LongField(label, (long)value);
                }
                else if (valueType == typeof(ulong))
                {
                    EditorGUILayout.TextField(label, ((ulong)value).ToString());
                }
                else if (valueType == typeof(float))
                {
                    EditorGUILayout.FloatField(label, (float)value);
                }
                else if (valueType == typeof(double))
                {
                    EditorGUILayout.DoubleField(label, (double)value);
                }
                else if (valueType == typeof(string))
                {
                    EditorGUILayout.TextField(label, (string)value);
                }
                else if (valueType == typeof(Vector2))
                {
                    EditorGUILayout.Vector2Field(label, (Vector2)value);
                }
                else if (valueType == typeof(Vector3))
                {
                    EditorGUILayout.Vector3Field(label, (Vector3)value);
                }
                else if (valueType == typeof(Vector4))
                {
                    EditorGUILayout.Vector4Field(label, (Vector4)value);
                }
                else if (valueType == typeof(Vector2Int))
                {
                    EditorGUILayout.Vector2IntField(label, (Vector2Int)value);
                }
                else if (valueType == typeof(Vector3Int))
                {
                    EditorGUILayout.Vector3IntField(label, (Vector3Int)value);
                }
                else if (valueType == typeof(Color))
                {
                    EditorGUILayout.ColorField(label, (Color)value);
                }
                else if (valueType == typeof(Bounds))
                {
                    EditorGUILayout.BoundsField(label, (Bounds)value);
                }
                else if (valueType == typeof(Rect))
                {
                    EditorGUILayout.RectField(label, (Rect)value);
                }
                else if (valueType == typeof(RectInt))
                {
                    EditorGUILayout.RectIntField(label, (RectInt)value);
                }
                else if (typeof(UnityEngine.Object).IsAssignableFrom(valueType))
                {
                    EditorGUILayout.ObjectField(label, (UnityEngine.Object)value, valueType, true);
                }
                else if (valueType.BaseType == typeof(Enum))
                {
                    EditorGUILayout.EnumPopup(label, (Enum)value);
                }
                else if (valueType.BaseType == typeof(System.Reflection.TypeInfo))
                {
                    EditorGUILayout.TextField(label, value.ToString());
                }
                else
                {
                    isDrawn = false;
                }

                return isDrawn;
            }
        }

        private static void DebugLogMessage(string message, MessageType type, UnityEngine.Object context)
        {
            switch (type)
            {
                case MessageType.None:
                case MessageType.Info:
                    Debug.Log(message, context);
                    break;
                case MessageType.Warning:
                    Debug.LogWarning(message, context);
                    break;
                case MessageType.Error:
                    Debug.LogError(message, context);
                    break;
            }
        }
    }
}
