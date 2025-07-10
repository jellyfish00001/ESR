using System;

namespace ERS.DTO.BDExpenseSenario
{
    public class ExtraStepsDto
    {
        public Guid Id { get; set; }
        public string SenarioId { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public string ApproverEmplid { get; set; }
        public string ApproverName { get; set; }
        public string ApproverNameA { get; set; }
    }
}
