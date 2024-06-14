namespace PlanillaPM.Models
{
    public class OrganizationalDetails
    {
        public string Id { get; set; }
        public string Role { get; set; }
        public string Manager { get; set; }
        public string Color { get; set; }

        public static List<OrganizationalDetails> GetAllRecords()
        {
            return new List<OrganizationalDetails>
        {
            new OrganizationalDetails { Id = "1", Role = "General Manager", Manager = "", Color = "#71AF17" },
            new OrganizationalDetails { Id = "2", Role = "Assistant Manager", Manager = "1", Color = "#1859B7" },
            new OrganizationalDetails { Id = "3", Role = "Human Resource Manager", Manager = "1", Color = "#2E95D8" },
            new OrganizationalDetails { Id = "4", Role = "Customer Service Manager", Manager = "1", Color = "#9A9A9A" },
            new OrganizationalDetails { Id = "5", Role = "Marketing Manager", Manager = "1", Color = "#228B22" },
            new OrganizationalDetails { Id = "6", Role = "Assistant Manager", Manager = "2", Color = "#1859B7" },
            new OrganizationalDetails { Id = "7", Role = "Human Resource Manager", Manager = "2", Color = "#2E95D8" },
            new OrganizationalDetails { Id = "8", Role = "Customer Service Manager", Manager = "3", Color = "#9A9A9A" },
            new OrganizationalDetails { Id = "9", Role = "Marketing Manager", Manager = "4", Color = "#228B22" },
            
        };
        }
    }
}
