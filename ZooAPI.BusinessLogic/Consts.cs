namespace ZooAPI
{
    public class Consts
    {
        public const string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        public const string ErrorNameLength = "The name is required and should not exceed 255 characters.";
        public const string ErrorNameUnique = "The name must be unique.";
        public const string ErrorSpeciesLength = "The species is required and should not exceed 255 characters.";
        public const string ErrorWeightNumber = "The weight must be a positive number.";
        public const string ErrorValidClass = "The class is required and should be one of the valid options.";
        public const string ErrorValidType =  "The type is required and should be one of the valid options.";

        public const string Lion = "Lion";
        public const string Lioness = "Lioness";
        public const string Elephant = "Elephant";
        public const string LoxodontaAfricana = "Loxodonta africana";
        public const string Giraffe = "Giraffe";
        public const string GiraffaCamelopardalis = "Giraffa camelopardalis";
        public const string PantheraLeo = "Panthera leo";
    }
}
