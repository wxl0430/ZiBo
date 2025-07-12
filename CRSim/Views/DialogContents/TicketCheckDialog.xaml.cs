using System.Text.RegularExpressions;

namespace CRSim.Views;
public sealed partial class TicketCheckDialog : Page
{
    private readonly Action<bool> _onValidityChanged;
    public List<string> GeneratedTicketChecks = [];
    public string SelectedWaitingArea => WaitingAreasComboBox.SelectedValue.ToString();
    private List<string> WaitingAreaNames { get; set; } = [];
    private void Validate(object sender, object e)
    {
        bool isValid = !string.IsNullOrWhiteSpace(TicketCheckTextBox.Text) && !string.IsNullOrWhiteSpace(WaitingAreasComboBox.SelectedValue?.ToString());
        _onValidityChanged?.Invoke(isValid);
    }
    public TicketCheckDialog(List<string> waitingAreaNames, Action<bool> onValidityChanged)
    {
        _onValidityChanged = onValidityChanged;
        WaitingAreaNames = waitingAreaNames;
        InitializeComponent();
    }
    public void GenerateTicketChecks()
    {
        var input = TicketCheckTextBox.Text.Trim();
        if (!input.Contains('-'))
        {
            GeneratedTicketChecks = [input];
            return;
        }

        string[] parts = input.Split(['-'], 2);
        if (parts.Length != 2)
        {
            GeneratedTicketChecks = [input];
            return;
        }

        var startMatch = MyRegex().Match(parts[0]);
        var endMatch = MyRegex().Match(parts[1]);

        if (!startMatch.Success || !endMatch.Success)
        {
            GeneratedTicketChecks = [input];
            return;
        }

        int startNum = int.Parse(startMatch.Groups[1].Value);
        string sLetter = startMatch.Groups[2].Value;
        int endNum = int.Parse(endMatch.Groups[1].Value);
        string eLetter = endMatch.Groups[2].Value;

        bool startHasLetter = !string.IsNullOrEmpty(sLetter);
        bool endHasLetter = !string.IsNullOrEmpty(eLetter);

        if (startHasLetter != endHasLetter)
        {
            GeneratedTicketChecks = [input];
        }

        int min = Math.Min(startNum, endNum);
        int max = Math.Max(startNum, endNum);
        List<int> numbers = [.. Enumerable.Range(min, max - min + 1)];

        List<string> result = [];

        if (startHasLetter)
        {
            if (sLetter == eLetter)
            {
                numbers.ForEach(n => result.Add($"{n}{sLetter}"));
            }
            else
            {
                numbers.ForEach(n =>
                {
                    result.Add($"{n}A");
                    result.Add($"{n}B");
                });
            }
        }
        else
        {
            numbers.ForEach(n => result.Add(n.ToString()));
        }
        GeneratedTicketChecks = result;
        return;
    }

    [GeneratedRegex(@"^(\d+)([AB]?)$")]
    private static partial Regex MyRegex();
}
