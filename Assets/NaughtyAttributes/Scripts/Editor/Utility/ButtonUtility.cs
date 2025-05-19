using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;

namespace NaughtyAttributes.Editor
{
    public static class ButtonUtility
    {
        public static bool IsEnabled(Object target, MethodInfo method)
        {
            EnableIfAttributeBase enableIfAttribute = method.GetCustomAttribute<EnableIfAttributeBase>();
            if (enableIfAttribute == null)
            {
                return true;
            }

            List<bool> conditionValues = PropertyUtility.GetConditionValues(target, enableIfAttribute.Conditions);
            if (conditionValues.Count > 0)
            {
                bool enabled = PropertyUtility.GetConditionsFlag(conditionValues, enableIfAttribute.ConditionOperator, enableIfAttribute.Inverted);
                return enabled;
            }
            else
            {
                string message = enableIfAttribute.GetType().Name + " needs a valid boolean condition field, property or method name to work";
                Debug.LogWarning(message, target);

                return false;
            }
        }

        public static bool IsVisible(Object target, MethodInfo method)
        {
            ShowIfAttributeBase showIfAttribute = method.GetCustomAttribute<ShowIfAttributeBase>();
            if (showIfAttribute == null)
            {
                return true;
            }

            List<bool> conditionValues = PropertyUtility.GetConditionValues(target, showIfAttribute.Conditions);
            if (conditionValues.Count > 0)
            {
                bool enabled = PropertyUtility.GetConditionsFlag(conditionValues, showIfAttribute.ConditionOperator, showIfAttribute.Inverted);
                return enabled;
            }
            else
            {
                string message = showIfAttribute.GetType().Name + " needs a valid boolean condition field, property or method name to work";
                Debug.LogWarning(message, target);

                return false;
            }
        }

        public static bool Has(this DisplayOptions options, DisplayOptions target)
        {
            return ((int)options & (int)target) != 0;
        }

        public static DisplayOptions GetPosition(this DisplayOptions options) => options switch
        {
            var x when x.Has(DisplayOptions.OnTop) => DisplayOptions.OnTop,
            var x when x.Has(DisplayOptions.AtBottom) => DisplayOptions.AtBottom,
            var x when x.Has(DisplayOptions.AlongSide) => DisplayOptions.AlongSide,
            _ => DisplayOptions.AtBottom,
        };

        public static GUILayoutOption GetHeightOption(this DisplayOptions options) => options switch
        {
            var x when x.Has(DisplayOptions.DoubleLineHeight) => GUILayout.Height(EditorGUIUtility.singleLineHeight * 2),
            var x when x.Has(DisplayOptions.TripleLineHeight) => GUILayout.Height(EditorGUIUtility.singleLineHeight * 3),
            _ => null,
        };

        public static GUILayoutOption GetWidthOption(this DisplayOptions options) => options switch
        {
            var x when x.Has(DisplayOptions.HalfWidth) => GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.5f),
            _ => null,
        };
    }
}
