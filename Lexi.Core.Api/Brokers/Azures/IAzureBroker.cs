//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Microsoft.AspNetCore.Http;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Brokers.Azures
{
    public interface IAzureBroker
    {
        ValueTask<string> TakeFeedbackAsync(string formfile);
    }
}
