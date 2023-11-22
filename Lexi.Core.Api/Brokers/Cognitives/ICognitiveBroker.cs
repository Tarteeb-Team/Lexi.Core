//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.IO;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Brokers.Cognitives
{
    public interface ICognitiveBroker
    {
        Task<string> GetOggFile(Stream stream);
    }
}
