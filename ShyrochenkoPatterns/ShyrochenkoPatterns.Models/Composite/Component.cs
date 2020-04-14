using ShyrochenkoPatterns.Models.ResponseModels.Composite;

namespace ShyrochenkoPatterns.Models.Composite
{
    public abstract class Component
    {
        public int Id;

        public string Name;

        public Component(string name)
        {
            Name = name;
            Id = Composite.Id;
        }

        public abstract ComponentResponseModel Display();

        public abstract ComponentResponseModel Add(Component component);

        public abstract void Remove(int id);

        public abstract int Size();

        public abstract Component Get(int id);
    }
}
