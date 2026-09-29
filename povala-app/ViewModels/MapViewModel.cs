using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using povala_app.Services;
using Povala.Shared.DTOs.Ascent;
using Povala.Shared.DTOs.Boulder;
using Povala.Shared.DTOs.Project;

namespace povala_app.ViewModels;

public partial class MapViewModel : ObservableObject
{
    private readonly BoulderApiService _boulderApiService;
    private readonly AscentsApiService _ascentsApiService;
    private readonly ProjectsApiService _projectsApiService;

    [ObservableProperty] 
    public partial List<BoulderDto> AvailableBoulders { get; set; } = new();

    [ObservableProperty]
    public partial List<AscentDto> MyAscents { get; set; } = new();

    [ObservableProperty]
    public partial List<ProjectDto> MyProjects { get; set; } = new();

    public MapViewModel(
        BoulderApiService boulderApiService, 
        AscentsApiService ascentsApiService, 
        ProjectsApiService projectsApiService)
    {
        _boulderApiService = boulderApiService;
        _ascentsApiService = ascentsApiService;
        _projectsApiService = projectsApiService;
    }

    public async Task LoadDataAsync()
    {
        try
        {
            var bouldersTask = _boulderApiService.GetAllBouldersAsync();
            var ascentsTask = _ascentsApiService.GetMyAsync();
            var projectsTask = _projectsApiService.GetMyAsync();

            await Task.WhenAll(bouldersTask, ascentsTask, projectsTask);

            AvailableBoulders = await bouldersTask;
            MyAscents = await ascentsTask;
            MyProjects = await projectsTask;
        }
        catch (Exception ex)
        {
            // TODO: Handle error properly later, e.g. show alert
            System.Diagnostics.Debug.WriteLine($"Error loading map data: {ex.Message}");
        }
    }

    public async Task DeleteBoulderAsync(int boulderId)
    {
        var success = await _boulderApiService.DeleteAsync(boulderId);
        if (success) await LoadDataAsync();
    }

    public async Task CreateBoulderAsync(CreateBoulderDto newBoulder)
    {
        var result = await _boulderApiService.CreateAsync(newBoulder);
        if (result != null) await LoadDataAsync();
    }

    public async Task MarkAscentAsync(int boulderId)
    {
        var success = await _ascentsApiService.CreateAsync(new CreateAscentDto { BoulderId = boulderId });
        if (success != null) await LoadDataAsync();
    }

    public async Task RemoveAscentAsync(int ascentId)
    {
        var success = await _ascentsApiService.DeleteAsync(ascentId);
        if (success) await LoadDataAsync();
    }

    public async Task MarkProjectAsync(int boulderId)
    {
        var success = await _projectsApiService.CreateAsync(new CreateProjectDto { BoulderId = boulderId });
        if (success != null) await LoadDataAsync();
    }

    public async Task RemoveProjectAsync(int projectId)
    {
        var success = await _projectsApiService.DeleteAsync(projectId);
        if (success) await LoadDataAsync();
    }
}
