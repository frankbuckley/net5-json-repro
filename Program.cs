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

        for (int i = 0; i < 1000; i++)
        {

            var customers = new CustomerCollectionResponse
            {
                Customers = Enumerable.Range(0, 500).Select(i => CustomerDataGenerator.CreateCustomer()).ToList()
            };

            var buffer = new MemoryStream();

            await JsonSerializer.SerializeAsync(buffer, customers);

            //await buffer.FlushAsync();
            buffer.Position = 0;

            //var reader = new StreamReader(buffer);
            //Console.WriteLine();
            //Console.WriteLine(await reader.ReadToEndAsync());

            //buffer.Position = 0;

            var deserialized = await JsonSerializer.DeserializeAsync<CustomerCollectionResponse>(buffer, s_serializerOptions);

            Console.WriteLine($"{i:0000}: Reading Json from stream - deserialized {deserialized.Customers.Count} customer records.");

        }
    }
}

public static class CustomerDataGenerator
{
    public static Customer CreateCustomer()
    {
        var addressFaker = new AutoFaker<Address>().RuleFor(a => a.Province, f => f.Address.State().OrNull(f, 0.7f));
        var customerFaker = new AutoFaker<Customer>();

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
