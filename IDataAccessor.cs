using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Data;

namespace WCF_Clinet
{
    [ServiceContract]
    public interface IDataAccessor
    {
        [OperationContract]
        DataSet GetData();

        [OperationContract]
        void InsertData(string colNames, string colData);

        [OperationContract]
        void UpdateData(string cols, string keys);

        [OperationContract]
        void DeleteData(string keys);
    }
}
