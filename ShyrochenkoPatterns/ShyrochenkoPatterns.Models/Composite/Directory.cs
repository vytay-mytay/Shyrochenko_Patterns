using ShyrochenkoPatterns.Common.Exceptions;
using ShyrochenkoPatterns.Models.Enums;
using ShyrochenkoPatterns.Models.ResponseModels.Composite;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace ShyrochenkoPatterns.Models.Composite
{
    public class Directory : Component
    {

        public List<Component> Children = new List<Component>();

        public Directory(string name) : base(name)
        {
            Children = new List<Component>();
        }

        public override ComponentResponseModel Add(Component component)
        {
            Children.Add(component);

            return component.Display();
        }

        public override ComponentResponseModel Display()
        {
            var response = new ComponentResponseModel();

            response.Id = Id;
            response.Name = Name;
            response.Size = Size();
            response.Type = ComponentType.Directory;

            Children.ForEach(c => response.Children.Add(c.Display()));

            return response;
        }

        public override Component Get(int id)
        {
            if (this.Id == id)
                return this;

            foreach (var item in Children)
            {
                var response = item.Get(id);
                if (response != null)
                    return response;
            }

            throw new CustomException(HttpStatusCode.BadRequest, "id", "You haven't component with such id");
        }

        public override void Remove(int id)
        {
            if (Children.Any(c => c.Id == id))
                Children.Remove(Children.FirstOrDefault(c => c.Id == id));

            Children.ForEach(c => c.Remove(id));
        }

        public override int Size()
        {
            var size = 0;

            Children.ForEach(c => size += c.Size());

            return size;
        }
    }
}
