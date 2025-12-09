using System.Text.Json.Serialization;

namespace StudHub.SharedDTO.Order
{
    public class BrickLinkOrderDTO
    {
        [JsonPropertyName("order_id")]
        public long OrderId { get; set; }

        [JsonPropertyName("order_date")]
        public DateTime OrderDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } // e.g., "INVOICED", "PAID", "SHIPPED"

        [JsonPropertyName("currency_code")]
        public string CurrencyCode { get; set; } // e.g., "USD", "EUR"

        [JsonPropertyName("total_price")]
        public string TotalPrice { get; set; } // Stored as string to preserve precision

        [JsonPropertyName("buyer")]
        public BuyerDTO Buyer { get; set; }

        [JsonPropertyName("seller")]
        public SellerDTO Seller { get; set; }

        [JsonPropertyName("shipping_address")]
        public AddressDTO ShippingAddress { get; set; }

        [JsonPropertyName("billing_address")]
        public AddressDTO BillingAddress { get; set; }

        [JsonPropertyName("items")]
        public List<OrderItemDTO> Items { get; set; }

        [JsonPropertyName("shipping_cost")]
        public string ShippingCost { get; set; } // string to preserve decimal

        [JsonPropertyName("tax")]
        public string Tax { get; set; }

        [JsonPropertyName("notes")]
        public string Notes { get; set; }

        // Buyer representation
        public class BuyerDTO
        {
            [JsonPropertyName("username")]
            public string Username { get; set; }

            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("phone")]
            public string Phone { get; set; }
        }

        // Seller representation
        public class SellerDTO
        {
            [JsonPropertyName("username")]
            public string Username { get; set; }

            [JsonPropertyName("email")]
            public string Email { get; set; }

            [JsonPropertyName("phone")]
            public string Phone { get; set; }
        }

        // Address representation
        public class AddressDTO
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("street")]
            public string Street { get; set; }

            [JsonPropertyName("city")]
            public string City { get; set; }

            [JsonPropertyName("state")]
            public string State { get; set; }

            [JsonPropertyName("postal_code")]
            public string PostalCode { get; set; }

            [JsonPropertyName("country_code")]
            public string CountryCode { get; set; }
        }

        // Order item representation
        public class OrderItemDTO
        {
            [JsonPropertyName("item_id")]
            public long ItemId { get; set; }

            [JsonPropertyName("item_name")]
            public string ItemName { get; set; }

            [JsonPropertyName("item_type")]
            public string ItemType { get; set; } // e.g., "PART", "SET", "MINIFIGURE"

            [JsonPropertyName("color_id")]
            public int ColorId { get; set; }

            [JsonPropertyName("color_name")]
            public string ColorName { get; set; }

            [JsonPropertyName("quantity")]
            public int Quantity { get; set; }

            [JsonPropertyName("unit_price")]
            public string UnitPrice { get; set; }

            [JsonPropertyName("total_price")]
            public string TotalPrice { get; set; }

            [JsonPropertyName("new_or_used")]
            public string NewOrUsed { get; set; } // "N" or "U"

            [JsonPropertyName("item_weight")]
            public string ItemWeight { get; set; }
        }
    }
}
