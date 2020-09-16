using AutoBogus;
using AutoBogus.Conventions;
using Bogus.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

class Program
{
    private static readonly JsonSerializerOptions s_serializerOptions = new JsonSerializerOptions
    {
        IgnoreNullValues = true // works if set to false
    };

    static async Task Main(string[] args)
    {
        AutoFaker.Configure(builder =>
        {
            builder.WithConventions();
        });

        string runId = DateTime.UtcNow.ToString("yyyy-MM-dd-HHmmss");
        string outputDir = Path.Combine(AppContext.BaseDirectory, "../../../examples", runId);
        string outputFile = Path.Combine(outputDir, runId + ".txt");

        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        using (var output = File.OpenWrite(outputFile))
        using (var writer = new StreamWriter(output))
        {
            for (int i = 0; i < 1000; i++)
            {
                var stream = new MemoryStream();

                try
                {
                    var customers = new CustomerCollectionResponse
                    {
                        Customers = Enumerable.Range(0, 50).Select(i => CustomerDataGenerator.CreateCustomer()).ToList()
                    };

                    await JsonSerializer.SerializeAsync(stream, customers);

                    stream.Position = 0;

                    var deserialized = await JsonSerializer.DeserializeAsync<CustomerCollectionResponse>(stream, s_serializerOptions);

                    writer.WriteLine($"{i:0000}: Reading Json from stream - deserialized {deserialized.Customers.Count} customer records.");
                    Console.WriteLine($"{i:0000}: Reading Json from stream - deserialized {deserialized.Customers.Count} customer records.");
                }
                catch (Exception e)
                {
                    writer.WriteLine($"{i:0000}: Error:{Environment.NewLine}{e}");
                    Console.WriteLine($"{i:0000}: Error:{Environment.NewLine}{e}");

                    using (var file = File.OpenWrite(Path.Combine(outputDir, $"{runId}-{i:0000}.json")))
                    {
                        await file.WriteAsync(stream.ToArray());
                    }
                }
            }
        }
    }
}

public static class CustomerDataGenerator
{
    public static Customer CreateCustomer()
    {
        var addressFaker = new AutoFaker<Address>()
            .RuleFor(a => a.Address1, f => f.Address.StreetName())
            .RuleFor(a => a.Address2, f => f.Address.StreetName().OrNull(f, 0.9f))
            .RuleFor(a => a.City, f => f.Address.City())
            .RuleFor(a => a.Country, f => f.Address.Country())
            .RuleFor(a => a.CountryCode, f => f.Address.CountryCode())
            .RuleFor(a => a.Province, f => f.Address.State().OrNull(f, 0.7f));
        var customerFaker = new AutoFaker<Customer>()
            .RuleFor(c => c.Email, f => f.Person.Email);

        var customer = customerFaker.Generate();
        customer.DefaultAddress = addressFaker.Generate();
        customer.Addresses.Add(customer.DefaultAddress);

        return customer;
    }
}

public class CustomerCollectionResponse
{
    [JsonPropertyName("customers")]
    public List<Customer>? Customers { get; set; }
}

public class Address
{
    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("address1")]
    public string? Address1 { get; set; }

    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("zip")]
    public string? Zip { get; set; }

    [JsonPropertyName("province")]
    public string? Province { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("address2")]
    public string? Address2 { get; set; }

    [JsonPropertyName("company")]
    public string? Company { get; set; }

    [JsonPropertyName("latitude")]
    public float? Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public float? Longitude { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }

    [JsonPropertyName("province_code")]
    public string? ProvinceCode { get; set; }
}

public class Customer
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("accepts_marketing")]
    public bool AcceptsMarketing { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; set; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("orders_count")]
    public int OrdersCount { get; set; }

    [JsonPropertyName("state")]
    public string? State { get; set; }

    [JsonPropertyName("total_spent")]
    public string? TotalSpent { get; set; }

    [JsonPropertyName("last_order_id")]
    public long? LastOrderId { get; set; }

    [JsonPropertyName("note")]
    public string? Note { get; set; }

    [JsonPropertyName("verified_email")]
    public bool VerifiedEmail { get; set; }

    [JsonPropertyName("multipass_identifier")]
    public string? MultipassIdentifier { get; set; }

    [JsonPropertyName("tax_exempt")]
    public bool TaxExempt { get; set; }

    [JsonPropertyName("tags")]
    public string? Tags { get; set; }

    [JsonPropertyName("last_order_name")]
    public string? LastOrderName { get; set; }

    [JsonPropertyName("default_address")]
    public Address? DefaultAddress { get; set; }

    [JsonPropertyName("addresses")]
    public IList<Address>? Addresses { get; set; }
}
