using System;
using System.Windows.Forms;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Sql;

namespace WindowsFormsApplication1 {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();

            //System.Data.DataTable t = new System.Data.DataTable();
            //using(BigQueryConnection connection = new BigQueryConnection("PrivateKeyFileName=p12key.p12;ProjectID=zymosimeter;DataSetId=testdata;ServiceAccountEmail=227277881286-l0fodnq2h35m58b80up9vi4g83p1ogus@developer.gserviceaccount.com")) {
            //    connection.Open();
            //    connection.ChangeDatabase("testdata");
            //    using(BigQueryDataAdapter adapter = new BigQueryDataAdapter("SELECT * FROM [testdata.natality2]", connection)) {
            //        adapter.Fill(t);
            //    }
            //}
            //dataGridView1.DataSource = t;

            sqlDataSource1.ConnectionName = "bigqueryConnectionString";
            var query = new CustomSqlQuery("Natality",
                "SELECT * FROM [testdata.natality2]");
            query.Parameters.Add(new QueryParameter("age", typeof(int), 50));
            sqlDataSource1.Queries.Add(query);
        }

        private void button1_Click(object sender, EventArgs e) {
            sqlDataSource1.Fill();

            this.dataGridView1.DataSource = this.sqlDataSource1;
            this.dataGridView1.DataMember = "Natality";
        }

        private void button2_Click(object sender, EventArgs e) {
            sqlDataSource1.Queries[0].EditQuery();
        }

        private void button3_Click(object sender, EventArgs e) {
            //new Report().ShowDesignerDialog();
        }
    }
}
