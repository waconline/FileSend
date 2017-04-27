using System.ServiceModel;

namespace DataBaseService
{
    [ServiceContract]
    public interface IDataBaseOperator
    {
        [OperationContract]
         bool Login(string Nickname, string Password);
        [OperationContract]
         bool Registration(string Nickname, string Password, string Fname, string Sname, string Mail);
    }
}