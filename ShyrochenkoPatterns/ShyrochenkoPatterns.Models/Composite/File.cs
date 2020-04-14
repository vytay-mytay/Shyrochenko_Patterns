using ShyrochenkoPatterns.Models.Enums;
using ShyrochenkoPatterns.Models.ResponseModels.Composite;

namespace ShyrochenkoPatterns.Models.Composite
{
    public class File : Component
    {
        private int _size = 0;

        public File(string name, int size) : base(name)
        {
            _size = size;
        }

        public override ComponentResponseModel Add(Component component)
        {
            return Display();
        }

        public override ComponentResponseModel Display()
        {
            return new ComponentResponseModel()
            {
                Id = this.Id,
                Name = Name,
                Size = Size(),
                Type = ComponentType.File
            };
        }

        public override Component Get(int id)
        {
            if (this.Id == id)
                return this;

            return null;
        }

        public override void Remove(int id)
        {
            return;
        }

        public override int Size()
        {
            var size = _size;

            return size;
        }
    }
}
