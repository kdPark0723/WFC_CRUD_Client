using System;
using System.Data;
using System.Windows.Forms;
using System.ServiceModel;

namespace WCF_Clinet
{
    public partial class MainForm : Form
    {
        IDataAccessor dataAccessor;

        public MainForm()
        {
            InitializeComponent();

            try
            {
                ChannelFactory<IDataAccessor> factory = new ChannelFactory<IDataAccessor>();

                string address = "net.tcp://localhost:8080/wcf";
                factory.Endpoint.Address = new EndpointAddress(address);

                factory.Endpoint.Binding = new NetTcpBinding();

                factory.Endpoint.Contract.ContractType = typeof(IDataAccessor);

                dataAccessor = factory.CreateChannel();

                SyncData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database link fail!");
                this.Close();
            }
        }

        private void ReadButton_Click(object sender, EventArgs e)
        {
            SyncData();
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCellAddress.Y >= 0)
                dataAccessor.InsertData(GetColNames(), GetSelectColData());

            SyncData();
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            dataAccessor.UpdateData(GetSelectColNamesAndData(), GetSelectPrimaryKeyColNamesAndData());

            SyncData();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            dataAccessor.DeleteData(GetSelectPrimaryKeyColNamesAndData());

            SyncData();
        }

        private void SyncData()
        {
            DataSet ds = dataAccessor.GetData();

            dataGridView1.DataSource = ds.Tables[0];
        }

        private string GetColNames()
        {
            int size = dataGridView1.Columns.Count;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < size; ++i)
            {
                if (i != 0)
                    sb.Append(", ");

                sb.Append(dataGridView1.Columns[i].Name);
            }

            return sb.ToString();
        }

        private string GetSelectColData()
        {
            int size = dataGridView1.Columns.Count;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < size; ++i)
            {
                if (i != 0)
                    sb.Append(", ");

                string value = ParseValue(dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[i].Value.ToString());

                sb.Append(value);
            }

            return sb.ToString();
        }

        private string GetSelectColNamesAndData()
        {
            int size = dataGridView1.Columns.Count;


            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            bool isFirst = true;
            for (int i = 0; i < size; ++i)
            {
                string name = dataGridView1.Columns[i].Name;
                string value = ParseValue(dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[i].Value.ToString());

                if (IsPrimaryKey(name))
                    continue;

                if (!isFirst)
                    sb.Append(", ");


                sb.Append(name + "=" + value);

                isFirst = false;
            }

            return sb.ToString();
        }

        private string GetSelectPrimaryKeyColNamesAndData()
        {
            int size = dataGridView1.Columns.Count;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            bool isFirst = true;
            for (int i = 0; i < size; ++i)
            {
                string name = dataGridView1.Columns[i].Name;
                string value = ParseValue(dataGridView1.Rows[dataGridView1.CurrentCellAddress.Y].Cells[i].Value.ToString());

                if (!IsPrimaryKey(name))
                    continue;

                if (!isFirst)
                    sb.Append(", ");


                sb.Append(name + "=" + value);

                isFirst = false;
            }

            return sb.ToString();
        }

        private bool IsPrimaryKey(string value)
        {
            return value.Equals("id");
        }

        private string ParseValue(string value)
        {
            if (value.Equals(""))
                value = "NULL";

            if (!int.TryParse(value, out int tmp1) && !long.TryParse(value, out long tmp2))
                value = "'" + value + "'";

            return value;
        }
    }
}
