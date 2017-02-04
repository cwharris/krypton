namespace Krypton.Design
{
    public interface IShadowHullDrawContext
    {
        void AddShadowHullVertex(ShadowHullVertex shadowHullVertex);
        void AddShadowHullIndex(int index);
    }
}
