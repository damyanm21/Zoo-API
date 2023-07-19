using Microsoft.EntityFrameworkCore;

namespace ZooAPI.Data.ModelConfigs
{
    public interface IAnimalConfig
    {
        void Configure(ModelBuilder modelBuilder);
    }
}
