﻿//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.IO;
using System.Threading.Tasks;
using Lexi.Core.Api.Models.ObjcetModels;
using Microsoft.AspNetCore.Http;

namespace Lexi.Core.Api.Services.Orchestrations
{
    public interface IOrchestrationService
    {
        Task<ResponseCognitive> GetOggFile(Stream stream);
    }
}
