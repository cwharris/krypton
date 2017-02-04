using System.Collections.Generic;

namespace Krypton.Design
{
    public interface ILightmapPassProvider
    {
        IEnumerable<ILightmapPass> Passes { get; }
    }
}
