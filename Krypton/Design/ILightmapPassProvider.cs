using System.Collections.Generic;
using Krypton.Design;

namespace Krypton
{
    public interface ILightmapPassProvider
    {
        IEnumerable<ILightmapPass> Passes { get; }
    }
}
