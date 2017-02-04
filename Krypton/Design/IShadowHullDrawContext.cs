namespace Krypton.Design
{
    public interface IShadowHullDrawContext
    {
        void AddShadowHullVertex(HullVertex hullVertex);
        void AddShadowHullIndex(int index);
    }
}
