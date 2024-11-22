using ErrorOr;
using HMS.Client.Queries;

namespace HMS.Client;

public class QueryExecutor
{
    private readonly Dictionary<string, Func<IServiceProvider, IQuery>> _queriesDictionary;
    private readonly IServiceProvider _serviceProvider;

    public QueryExecutor(Dictionary<string, Func<IServiceProvider, IQuery>> queriesDictionary,
        IServiceProvider serviceProvider)
    {
        _queriesDictionary = queriesDictionary;
        _serviceProvider = serviceProvider;
    }

    public ErrorOr<string> Execute(string input)
    {
        if (input.Trim() == "help")
            return $"Possible queries:{Environment.NewLine}{string.Join(Environment.NewLine, _queriesDictionary.Keys)}";

        var commandName = input.Split('(').FirstOrDefault()?.Trim();

        if (string.IsNullOrWhiteSpace(commandName))
            return Error.Validation("Invalid command format.");

        if (!_queriesDictionary.TryGetValue(commandName, out var factory))
            return Error.Validation($"Unknown query type: {commandName}. Use help to see all possible queries.");

        return factory(_serviceProvider).Execute(input);
    }
}