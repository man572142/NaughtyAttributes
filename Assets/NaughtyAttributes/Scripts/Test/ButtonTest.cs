using System.Collections;
using UnityEngine;

namespace NaughtyAttributes.Test
{
    public class ButtonTest : MonoBehaviour
    {
        [Button(text: "Field <color=yellow>FatBottom</color>", method: nameof(IncrementMyInt), DisplayOptions = DisplayOptions.FatBottom)]
        public int myInt;

        [Button(text: "Field <color=yellow>Head</color>", method: nameof(DecrementMyInt), DisplayOptions = DisplayOptions.MiniTop)]
        public int myInt2;

        [Button(text: "Field <color=yellow>AlongSide</color>", method: nameof(DecrementMyInt), DisplayOptions = DisplayOptions.AlongSide)]
        public int myInt3;

        [Button("Method <color=yellow>OnTop</color>", SelectedEnableMode = EButtonEnableMode.Editor, DisplayOptions = DisplayOptions.OnTop)]
        private void IncrementMyInt()
        {
            myInt++;
        }

        [Button("Method <color=yellow>FatBottom</color>", SelectedEnableMode = EButtonEnableMode.Editor, DisplayOptions = DisplayOptions.FatBottom)]
        private void DecrementMyInt()
        {
            myInt--;
        }

        [Button("Method <color=yellow>Default</color>")]
        private void LogMyInt(string prefix)
        {
            Debug.Log(prefix + myInt);
        }

        [Button("StartCoroutine")]
        private IEnumerator IncrementMyIntCoroutine()
        {
            int seconds = 5;
            for (int i = 0; i < seconds; i++)
            {
                myInt++;
                yield return new WaitForSeconds(1.0f);
            }
        }
    }
}
