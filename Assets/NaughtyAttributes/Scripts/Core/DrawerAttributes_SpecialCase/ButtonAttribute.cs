using System;

namespace NaughtyAttributes
{
    public enum EButtonEnableMode
    {
        /// <summary>
        /// Button should be active always
        /// </summary>
        Always,
        /// <summary>
        /// Button should be active only in editor
        /// </summary>
        Editor,
        /// <summary>
        /// Button should be active only in playmode
        /// </summary>
        Playmode
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class ButtonAttribute : SpecialCaseDrawerAttribute
    {
        public string Text;
        public EButtonEnableMode SelectedEnableMode = EButtonEnableMode.Always;
        public object[] Args;
        public string Method;
        public DisplayOptions DisplayOptions = DisplayOptions.Default;

        public ButtonAttribute(params object[] args)
        {
            Args = args;
        }

        public ButtonAttribute(string textAndMethod = null, params object[] args)
        {
            Text = textAndMethod;
            Method = textAndMethod;
            Args = args;
        }

        public ButtonAttribute(string textAndMethod = null, DisplayOptions displayOptions = DisplayOptions.Default, params object[] args)
        {
            Text = textAndMethod;
            Method = textAndMethod;
            Args = args;
            DisplayOptions = displayOptions;
        }

        public ButtonAttribute(string text = null, string method = null, params object[] args)
        {
            Text = text ?? method;
            Args = args;
            Method = method;
        }

        public ButtonAttribute(string text = null, string method = null, DisplayOptions displayOptions = DisplayOptions.Default, params object[] args)
        {
            Text = text ?? method;
            Args = args;
            Method = method;
            DisplayOptions = displayOptions;
        }

        public ButtonAttribute(string text = null, string method = null, DisplayOptions displayOptions = DisplayOptions.Default, EButtonEnableMode enableMode = EButtonEnableMode.Always, params object[] args)
        {
            Text = text ?? method;
            Args = args;
            Method = method;
            DisplayOptions = displayOptions;
            SelectedEnableMode = enableMode;
        }
    }
}