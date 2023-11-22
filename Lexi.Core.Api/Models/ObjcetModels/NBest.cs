//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System.Collections.Generic;

namespace Lexi.Core.Api.Models.ObjcetModels
{
    public class NBest
    {
        public PronunciationAssessment PronunciationAssessment { get; set; }
        public List<Words> Words { get; set; }
    }
}
