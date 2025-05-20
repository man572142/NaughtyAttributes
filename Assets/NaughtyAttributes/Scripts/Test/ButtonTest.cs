using System.Collections;
using UnityEngine;

namespace NaughtyAttributes.Test
{
    public class ButtonTest : MonoBehaviour
    {
        [Button("Field <color=yellow>MiniTop</color>", nameof(IncrementMyInt), displayOptions: DisplayOptions.MiniTop)]
        [Button("Field <color=yellow>FatBottom</color>", nameof(IncrementMyInt), displayOptions: DisplayOptions.FatBottom)]
        public int myInt;

        [Button("Field <color=yellow>AlongSide</color>", nameof(DecrementMyInt), displayOptions: DisplayOptions.AlongSide)]
        public int myInt2;

        [Space]
        [Button(nameof(LogMyInt))]
        public string log = "MyInt is : ";

        [Button("Method <color=yellow>OnTop</color>", enableMode: EButtonEnableMode.Editor, displayOptions: DisplayOptions.OnTop)]
        private void IncrementMyInt()
        {
            myInt++;
        }

        [Button("Method <color=yellow>Default</color>", enableMode: EButtonEnableMode.Editor)]
        private void DecrementMyInt()
        {
            myInt--;
        }

        [Button(textAndMethod:"Method <color=yellow>Big</color>", displayOptions: DisplayOptions.Big)]
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
