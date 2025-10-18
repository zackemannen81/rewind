using System;
using UnityEngine;

namespace Chapter1
{
    public class Chapter1Generator : MonoBehaviour
    {
        public event Action OnGeneratorActivated;

        [SerializeField]
        private Chapter1FuseBox fuseBox;

        public bool IsGeneratorOnline { get; private set; }
        public bool HasCompletedGoldenPath { get; private set; }

        public void OnLoopStart()
        {
            IsGeneratorOnline = false;
        }

        public bool TryActivate()
        {
            if (IsGeneratorOnline)
            {
                return false;
            }

            if (fuseBox != null && !fuseBox.IsPowerToCourtyard)
            {
                return false;
            }

            IsGeneratorOnline = true;
            HasCompletedGoldenPath = true;
            OnGeneratorActivated?.Invoke();
            return true;
        }

        public void SetFuseBox(Chapter1FuseBox target)
        {
            fuseBox = target;
        }
    }
}
