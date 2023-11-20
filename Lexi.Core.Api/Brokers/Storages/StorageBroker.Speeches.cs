﻿//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.Linq;
using Lexi.Core.Api.Models.Foundations.Speeches;
using Microsoft.EntityFrameworkCore;

namespace Lexi.Core.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Speech> Speeches { get; set; }

        public IQueryable<Speech> SelectAllSpeeches() =>
            SelectAll<Speech>();
    }
}
