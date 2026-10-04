namespace Hello;

public class Greeter
{
    public string Greet(string name)
    {
        if (name == null)
        {
            return "Hello, stranger!";
        }

        return $"Hello, {name}!";
    }
}
