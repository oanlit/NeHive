using NeHive.Reactive;

namespace A
{
    public class Foo {}
}

namespace C
{
    using A;
    public static class FooExtensions
    {
        public static string ToText(this Foo foo) => "ok";
    }
}

namespace B
{
    using A;
    using C;
    
    [Store]
    public class FooTest
    {
        public Foo Value { get; set; } =  new();
        public List<Foo> Values { get; set; } = [];
        public string Text { get; set; } = new Foo().ToText();
        
        public List<string> BuildTexts(List<Foo> input)
        {
            var result = input
                .Select(x => x.ToText())
                .ToList();

            return result;
        }
    }

    public class NamespaceResolutionTest
    {
        public static FooTestStore FTest = new FooTestStore();
    }
}
