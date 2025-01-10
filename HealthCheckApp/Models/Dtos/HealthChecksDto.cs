namespace HealthCheckApp.Models.Dtos;

public class HealthChecksDto
{
    public IEnumerable<URIsDto> URIs { get; set; }
    public IEnumerable<DatabasesDto> Databases { get; set; }
    public IEnumerable<OtherConnectionsDto> OtherConnections { get; set; }

}

public class URIsDto
{
    public string Name { get; set; }
    public string Endpoint { get; set; }
    public bool Enabled { get; set; }
}

public class DatabasesDto
{
    public string Name { get; set; }
    public string Endpoint { get; set; }
    public bool Enabled { get; set; }
}

public class OtherConnectionsDto
{
    public string Name { get; set; }
    public bool Enabled { get; set; }
}
