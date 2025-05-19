using System.Collections;
using UnityEngine;

namespace NaughtyAttributes.Test
{
    public class ButtonTest : MonoBehaviour
    {
        [Button("Field <color=yellow>MiniTop</color>", nameof(IncrementMyInt), DisplayOptions = DisplayOptions.MiniTop)]
        [Button("Field <color=yellow>FatBottom</color>", nameof(IncrementMyInt), DisplayOptions = DisplayOptions.FatBottom)]
        public int myInt;

        [Button("Field <color=yellow>AlongSide</color>", nameof(DecrementMyInt), DisplayOptions = DisplayOptions.AlongSide)]
        public int myInt2;

        [Button("Method <color=yellow>OnTop</color>", SelectedEnableMode = EButtonEnableMode.Editor, DisplayOptions = DisplayOptions.OnTop)]
        private void IncrementMyInt()
        {
            myInt++;
        }

        [Button("Method <color=yellow>Default</color>", SelectedEnableMode = EButtonEnableMode.Editor)]
        private void DecrementMyInt()
        {
            myInt--;
        }

        [Button("Method <color=yellow>FatBottom</color>", "", "zzz", DisplayOptions = DisplayOptions.FatBottom)]
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
