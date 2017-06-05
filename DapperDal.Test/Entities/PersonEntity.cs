using System;
using System.Collections.Generic;

namespace DapperDal.Test.Entities
{
    public class PersonEntity
    {
        public long PersonId { get; set; }

        public string PersonName { get; set; }

        public int CarId { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public short IsActive { get; set; }
    }

}