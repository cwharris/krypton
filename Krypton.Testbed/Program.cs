namespace Krypton.Testbed
{
    public static class Program
    {
        #if WINDOWS || XBOX

        public static void Main()
        {
            using (var game = new Game2())
            {
                game.Run();
            }
        }
        
        #endif
    }
}
