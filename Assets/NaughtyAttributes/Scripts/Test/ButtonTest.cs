using System.Collections;
using UnityEngine;

namespace NaughtyAttributes.Test
{
    public class ButtonTest : MonoBehaviour
    {
        [Button(method: nameof(IncrementMyInt))]
        public int myInt;

        [Button(method: nameof(DecrementMyInt), DisplayedArea = DisplayedArea.Top)]
        public int myInt2;

        private void IncrementMyInt()
        {
            myInt++;
        }

        [Button("Decrement My Int", SelectedEnableMode = EButtonEnableMode.Editor)]
        private void DecrementMyInt()
        {
            myInt--;
        }

        [Button(SelectedEnableMode = EButtonEnableMode.Playmode)]
        private void LogMyInt(string prefix = "MyInt = ")
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
