using System;
using System.Collections.Generic;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using povala_app.Services;
using Povala.Shared.DTOs.Boulder;

namespace povala_app.ViewModels;

public partial class MapViewModel : ObservableObject
{
    private readonly BoulderApiService _boulderApiService;

    [ObservableProperty] public partial List<BoulderDto> AvailableBoulders = [];



}

