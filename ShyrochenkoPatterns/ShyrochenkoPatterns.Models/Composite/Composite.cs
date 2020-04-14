namespace ShyrochenkoPatterns.Models.Composite
{
    public static class Composite
    {
        private static int id;

        public static int Id
            => id++;

        public static Component Component = new Directory(":/");
    }
}
