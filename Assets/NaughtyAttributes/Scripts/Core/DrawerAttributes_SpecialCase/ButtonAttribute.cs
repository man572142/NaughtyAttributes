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

    public enum DisplayedArea
    { 
        Top,
        Bottom,
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ButtonAttribute : SpecialCaseDrawerAttribute
    {
        public string Text { get; private set; }
        public EButtonEnableMode SelectedEnableMode;
        public object[] Args;
        public string Method;
        public DisplayedArea DisplayedArea = DisplayedArea.Bottom;

        public ButtonAttribute()
        {
            this.Text = null;
            this.SelectedEnableMode = EButtonEnableMode.Always;
            this.Args = null;
            this.Method = null;
        }

        public ButtonAttribute(params object[] args)
        {
            this.Text = null;
            this.SelectedEnableMode = EButtonEnableMode.Always;
            this.Args = args;
            this.Method = null;
        }

        public ButtonAttribute(string text = null, params object[] args)
        {
            this.Text = text;
            this.SelectedEnableMode = EButtonEnableMode.Always;
            this.Args = args;
            this.Method = null;
        }

        public ButtonAttribute(string text = null, string method = null, params object[] args)
        {
            this.Text = text ?? method;
            this.SelectedEnableMode = EButtonEnableMode.Always;
            this.Args = args;
            this.Method = method;
        }
    }
}
