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

    [Flags]
    public enum DisplayOptions
    {
        None = 0,

        // Presets
        Default = AtBottom | SingleLineHeight | FullWidth,
        MiniBottom = AtBottom | SingleLineHeight | HalfWidth,
        MiniTop = OnTop | SingleLineHeight | HalfWidth,
        FatBottom = AtBottom | DoubleLineHeight | HalfWidth,
        Big = AtBottom | TripleLineHeight | FullWidth,

        // Position
        OnTop = 1 << 0,
        AtBottom = 1 << 2,
        AlongSide = 1 << 3,

        // Height
        SingleLineHeight = 1 << 4,
        DoubleLineHeight = 1 << 5,
        TripleLineHeight = 1 << 6,

        // Width
        FullWidth = 1 << 7,
        HalfWidth = 1 << 8,
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class ButtonAttribute : SpecialCaseDrawerAttribute
    {
        public string Text;
        public EButtonEnableMode SelectedEnableMode;
        public object[] Args;
        public string Method;
        public DisplayOptions DisplayOptions = DisplayOptions.Default;

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