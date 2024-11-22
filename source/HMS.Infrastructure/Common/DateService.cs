using HMS.Application.Common;

namespace HMS.Infrastructure.Common;

public class DateService : IDateService
{
    public DateTime NowDate => DateTime.Now.Date;
}