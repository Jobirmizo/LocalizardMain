﻿using System.Text.Json.Serialization;
using AutoMapper.Configuration.Annotations;
using Localizard.Domain.Entites;

namespace Localizard.Domain.ViewModel;

public class CreateProjectDetailView
{
    public string Key { get; set; }
    public int[] TranslationIds { get; set; } = null!;
    public string Description { get; set; }
    public string Tag { get; set; }
}