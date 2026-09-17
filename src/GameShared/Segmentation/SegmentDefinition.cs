using System.Collections.Generic;

namespace MetaFramework.Segmentation
{
    public sealed class SegmentDefinition
    {
        public string SegmentId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Version { get; set; } = 1;
        public List<Predicate> Predicates { get; set; } = new();
        public PredicateLogic PredicateLogic { get; set; } = PredicateLogic.And;
        /// <summary>session_start | daily | manual | event:{name}</summary>
        public string EvaluationTrigger { get; set; } = "session_start";
        /// <summary>0 = no TTL (manual only).</summary>
        public int TtlSeconds { get; set; } = 86400;
        public List<string> Tags { get; set; } = new();
    }
}
