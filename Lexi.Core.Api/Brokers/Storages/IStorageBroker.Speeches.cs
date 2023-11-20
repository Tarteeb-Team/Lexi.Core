//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.Linq;
using Lexi.Core.Api.Models.Foundations.Speeches;

namespace Lexi.Core.Api.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        IQueryable<Speech> SelectAllSpeeches();
    }
}
