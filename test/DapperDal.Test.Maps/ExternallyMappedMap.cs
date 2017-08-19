using DapperDal.Mapper;
using DapperDal.Test.Entities;

namespace DapperDal.Test.Maps
{
    public class ExternallyMappedMap
    {
        public class ExternallyMappedMapper : ClassMapper<ExternallyMapped>
        {
            public ExternallyMappedMapper()
            {
                Table("External");
                Map(x => x.Id).Column("ExternalId");
                AutoMap();
            }
        } 
    }
}