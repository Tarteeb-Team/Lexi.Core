//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.Collections.Generic;

namespace Lexi.Core.Api.Models.ObjcetModels
{
    public class ResponseCognitive
    {
        public string Id { get; set; }
        public string RecognitionStatus { get; set; }
        public string DisplayText { get; set; }
        public List<NBest> NBest { get; set; }

    }
}
