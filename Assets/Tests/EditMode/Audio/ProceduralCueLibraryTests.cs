using System;
using Audio;
using NUnit.Framework;

namespace Tests.Audio
{
    public class ProceduralCueLibraryTests
    {
        [Test]
        public void BuildLibrary_ContainsAllCueIdsWithClips()
        {
            var library = ProceduralCueLibrary.BuildLibrary();
            foreach (AudioCueId id in Enum.GetValues(typeof(AudioCueId)))
            {
                Assert.That(library.ContainsKey(id), $"Missing cue for {id}");
                var cue = library[id];
                Assert.That(cue.Clip, Is.Not.Null, $"Cue {id} has null clip");
                Assert.That(cue.Clip.samples, Is.GreaterThan(0), $"Cue {id} has zero samples");
            }
        }
    }
}
