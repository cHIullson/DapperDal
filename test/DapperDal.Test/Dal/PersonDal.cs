using DapperDal.Test.Entities;

namespace DapperDal.Test.Dal
{
    public class PersonDal : DalBase<PersonEntity, long>
    {
        public PersonDal() : base(ConnectionNames.Default)
        {
        }
    }
}
