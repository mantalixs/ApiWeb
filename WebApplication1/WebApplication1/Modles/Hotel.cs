namespace WebApplication1.Modles
{
    public class AddressObj
    {
        public string address_string;
        public string city;
        public string country;
        public string postalcode;
        public string street1;
        public string street2;
    }

    public class Datum
    {
        public AddressObj address_obj;
        public string location_id;
        public string name;
    }

    public class Root
    {
        public List<Datum> data;
    }
}
