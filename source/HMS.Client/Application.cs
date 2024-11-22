namespace HMS.Client;

public class Application
{
    private readonly QueryExecutor _queryExecutor;

    public Application(QueryExecutor queryExecutor)
    {
        _queryExecutor = queryExecutor;
    }

    public void Run()
    {
        try
        {
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Exiting program.");
                    break;
                }

                var result = _queryExecutor.Execute(input);

                if (result.IsError)
                    Console.WriteLine($"Error: {result.FirstError}");
                else
                    Console.WriteLine(result.Value);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}