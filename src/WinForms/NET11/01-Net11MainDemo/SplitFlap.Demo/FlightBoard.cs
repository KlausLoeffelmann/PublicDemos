namespace SplitFlap.Demo;

/// <summary>
///  Generates plausible departure rows. Any resemblance to actual delays is coincidental.
/// </summary>
internal sealed class FlightBoard(int columns)
{
    private static readonly string[] s_airlines = ["LH", "BA", "AF", "KL", "SK", "AZ", "IB", "OS", "LX", "SN"];
    private static readonly string[] s_cities =
    [
        "FRANKFURT", "LONDON LHR", "PARIS CDG", "AMSTERDAM", "COPENHAGEN", "ROMA FCO",
        "MADRID", "WIEN", "ZUERICH", "BRUXELLES", "MUENCHEN", "OSLO", "LISBOA", "ATHINAI"
    ];
    private static readonly string[] s_status =
    [
        "ON TIME", "ON TIME", "ON TIME", "BOARDING", "GATE OPEN", "DELAYED", "LAST CALL", "GO TO GATE"
    ];

    private readonly List<Flight> _flights = [];

    public string Header
        => Fit("FLIGHT  DESTINATION      TIME   GATE  REMARKS");

    /// <summary>
    ///  Produces the next board state: existing flights advance, the top one departs, a new one arrives.
    /// </summary>
    public string Next(int rows)
    {
        if (_flights.Count == 0)
        {
            TimeOnly time = new(DateTime.Now.Hour, DateTime.Now.Minute / 5 * 5);

            for (int i = 0; i < rows - 1; i++)
            {
                _flights.Add(Flight.Random(time.AddMinutes(10 * (i + 1))));
            }
        }
        else
        {
            _flights.RemoveAt(0);
            _flights.Add(Flight.Random(_flights[^1].Time.AddMinutes(Random.Shared.Next(5, 20))));

            foreach (Flight flight in _flights.Take(3))
            {
                flight.Advance();
            }
        }

        IEnumerable<string> lines = _flights.Select(f => Fit(f.ToString()));

        return string.Join(Environment.NewLine, lines.Prepend(Header));
    }

    private string Fit(string text)
        => text.Length >= columns ? text[..columns] : text.PadRight(columns);

    private sealed class Flight(string code, string city, TimeOnly time, string gate, int status)
    {
        public TimeOnly Time { get; } = time;

        private int _status = status;

        public static Flight Random(TimeOnly time)
            => new(
                $"{s_airlines[System.Random.Shared.Next(s_airlines.Length)]} {System.Random.Shared.Next(100, 999)}",
                s_cities[System.Random.Shared.Next(s_cities.Length)],
                time,
                $"{(char)('A' + System.Random.Shared.Next(3))}{System.Random.Shared.Next(1, 40):00}",
                System.Random.Shared.Next(3));

        public void Advance()
            => _status = Math.Min(_status + 1, s_status.Length - 1);

        public override string ToString()
            => $"{code,-7} {city,-16} {Time:HH:mm}  {gate,-5} {s_status[_status]}";
    }
}
