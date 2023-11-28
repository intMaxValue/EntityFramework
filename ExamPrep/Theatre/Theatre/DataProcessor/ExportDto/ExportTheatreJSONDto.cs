namespace Theatre.DataProcessor.ExportDto
{
    public class ExportTheatreJSONDto
    {
        public string Name { get; set; }

        public int Halls { get; set; }

        public decimal TotalIncome { get; set; }

        public ExportTicketsJSONDto[] Tickets { get; set; }
    }
}
