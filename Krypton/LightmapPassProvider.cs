using System.Collections.Generic;
using Krypton.Design;

namespace Krypton
{
    public class LightmapPassProvider : ILightmapPassProvider
    {
        public LightmapPassProvider(IEnumerable<ILightmapPass> passes)
        {
            Passes = passes;
        }

        public IEnumerable<ILightmapPass> Passes { get; }
    }
}
