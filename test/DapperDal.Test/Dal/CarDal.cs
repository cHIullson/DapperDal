using DapperDal.Test.Entities;

namespace DapperDal.Test.Dal
{
    public class CarDal : DalBase<CarEntity>
    {
        public CarDal() : base(ConnectionNames.Default)
        {
        }
    }
}
