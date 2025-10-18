using System;
using UnityEngine;

namespace Chapter1
{
    public class Chapter1FuseBox : MonoBehaviour
    {
        public event Action<bool> OnPowerRouteChanged;

        [SerializeField]
        private bool hasFuse;

        public bool IsPowerToCourtyard { get; private set; }

        public void OnLoopStart()
        {
            hasFuse = true;
            IsPowerToCourtyard = false;
        }

        public bool TryRoutePower(bool toCourtyard)
        {
            if (!hasFuse)
            {
                return false;
            }

            IsPowerToCourtyard = toCourtyard;
            OnPowerRouteChanged?.Invoke(toCourtyard);
            return true;
        }

        public void ConsumeFuse()
        {
            hasFuse = false;
        }
    }
}
