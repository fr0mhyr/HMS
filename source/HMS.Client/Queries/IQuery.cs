using ErrorOr;

namespace HMS.Client.Queries;

public interface IQuery
{
    ErrorOr<string> Execute(string input);
}