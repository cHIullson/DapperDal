using System;

namespace DapperDal.Test.Entities
{
    public class CarEntity
    {
        public int CarId { get; set; }

        public string CarName { get; set; }

        public int BrandId { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public bool IsActive { get; set; }
    }
}
