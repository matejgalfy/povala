using CommunityToolkit.Mvvm.ComponentModel;
using povala_app.Services;

namespace povala_app.ViewModels.User;

public partial class ProfileViewModel : ObservableObject
{
    private readonly StatsApiService _statsApiService;

    [ObservableProperty]
    public partial int TotalAscents { get; set; }

    [ObservableProperty]
    public partial int ActiveProjects { get; set; }

    [ObservableProperty]
    public partial string HighestGrade { get; set; } = "-";

    public ProfileViewModel(StatsApiService statsApiService)
    {
        _statsApiService = statsApiService;
    }

    public async Task LoadStatsAsync()
    {
        try
        {
            var stats = await _statsApiService.GetMyAsync();

            TotalAscents = stats.TotalAscents;
            ActiveProjects = stats.ActiveProjects;
            HighestGrade = stats.HighestGrade?.ToString() ?? "-";
        }
        catch (Exception ex)
        {
            // TODO: Handle error properly later, e.g. show alert
            System.Diagnostics.Debug.WriteLine($"Error loading profile stats: {ex.Message}");
        }
    }
}
