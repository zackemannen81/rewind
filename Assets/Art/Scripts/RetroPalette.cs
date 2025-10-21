using UnityEngine;

namespace Art
{
    [CreateAssetMenu(fileName = "RetroPalette", menuName = "Art/Retro Palette", order = 0)]
    public class RetroPalette : ScriptableObject
    {
        [Header("Primary Palette")]
        [SerializeField]
        private Color primaryBase = new(0.105f, 0.121f, 0.165f); // #1B1F2A

        [SerializeField]
        private Color primaryConcrete = new(0.196f, 0.204f, 0.227f); // #32343A

        [Header("Secondary Accents")]
        [SerializeField]
        private Color accentMagenta = new(0.996f, 0.176f, 0.584f); // #FE2D95

        [SerializeField]
        private Color accentCyan = new(0.0f, 0.819f, 0.996f); // #00D1FE

        [Header("Tertiary Details")]
        [SerializeField]
        private Color tertiaryOxide = new(0.655f, 0.341f, 0.235f); // #A8563C

        [SerializeField]
        private Color tertiaryWarmGrey = new(0.482f, 0.446f, 0.423f); // #7B716C

        [Header("Loop States")]
        [SerializeField]
        private Color loopNormalTint = Color.white;

        [SerializeField]
        private Color loopAlertTint = new(0.502f, 0.070f, 0.070f); // Slight crimson push

        [SerializeField]
        private Color loopEndDesaturate = new(0.298f, 0.298f, 0.298f);

        public Color PrimaryBase => primaryBase;
        public Color PrimaryConcrete => primaryConcrete;
        public Color AccentMagenta => accentMagenta;
        public Color AccentCyan => accentCyan;
        public Color TertiaryOxide => tertiaryOxide;
        public Color TertiaryWarmGrey => tertiaryWarmGrey;
        public Color LoopNormalTint => loopNormalTint;
        public Color LoopAlertTint => loopAlertTint;
        public Color LoopEndDesaturate => loopEndDesaturate;

        public Color GetColor(PaletteSwatch swatch)
        {
            return swatch switch
            {
                PaletteSwatch.PrimaryBase => primaryBase,
                PaletteSwatch.PrimaryConcrete => primaryConcrete,
                PaletteSwatch.AccentMagenta => accentMagenta,
                PaletteSwatch.AccentCyan => accentCyan,
                PaletteSwatch.TertiaryOxide => tertiaryOxide,
                PaletteSwatch.TertiaryWarmGrey => tertiaryWarmGrey,
                PaletteSwatch.LoopNormal => loopNormalTint,
                PaletteSwatch.LoopAlert => loopAlertTint,
                PaletteSwatch.LoopEnd => loopEndDesaturate,
                _ => primaryBase,
            };
        }
    }

    public enum PaletteSwatch
    {
        PrimaryBase,
        PrimaryConcrete,
        AccentMagenta,
        AccentCyan,
        TertiaryOxide,
        TertiaryWarmGrey,
        LoopNormal,
        LoopAlert,
        LoopEnd,
    }
}
