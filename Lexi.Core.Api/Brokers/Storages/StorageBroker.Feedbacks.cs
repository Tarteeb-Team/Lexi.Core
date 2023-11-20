//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Lexi.Core.Api.Models.Foundations.Feedbacks;
using Microsoft.EntityFrameworkCore;

namespace Lexi.Core.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        DbSet<Feedback> Feedbacks { get; set; }
    }
}
