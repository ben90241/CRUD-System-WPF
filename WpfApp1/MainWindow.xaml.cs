using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Data.SqlClient;

using System.Web.Script.Serialization;
using System.Data;
using Microsoft.Win32;
using System.IO;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlCon = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Benson\source\repos\WpfApp1\WpfApp1\Database1.mdf;Integrated Security=True");
        public MainWindow()
        {
            InitializeComponent();
            sqlCon.Open();
            reload();
            sqlCon.Close();
        }

        private void reload()
        {
            SqlCommand cmd = new SqlCommand("select * from Employee", sqlCon);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            //dataGridView1.DataSource = dt;
            dataGrid.ItemsSource = dt.DefaultView;
        }

        private void Button1_Click(object sender, RoutedEventArgs e)
        {
            sqlCon.Open();
            SqlCommand cmd = new SqlCommand("if not exists (select Id from Employee where Id=@Id) begin insert into Employee (Id, Name, Position, Phone, Address) values (@Id, @Name, @Position, @Phone, @Address) end", sqlCon);

            cmd.Parameters.AddWithValue("@Id", int.Parse(textBox1.Text));
            cmd.Parameters.AddWithValue("@Name", textBox2.Text);
            cmd.Parameters.AddWithValue("@Position", textBox3.Text);
            cmd.Parameters.AddWithValue("@Phone", textBox4.Text);
            cmd.Parameters.AddWithValue("@Address", textBox5.Text);

            cmd.ExecuteNonQuery();

            reload();

            sqlCon.Close();
        }

        private void Button7_Click_1(object sender, RoutedEventArgs e)
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox6.Clear();
            reload();
        }

        private void Button2_Click_1(object sender, RoutedEventArgs e)
        {
            sqlCon.Open();
            SqlCommand cmd = new SqlCommand("update Employee set Name=@Name, Position=@Position, Phone=@Phone, Address=@Address where Id=@Id", sqlCon);

            cmd.Parameters.AddWithValue("@Id", int.Parse(textBox1.Text));
            cmd.Parameters.AddWithValue("@Name", textBox2.Text);
            cmd.Parameters.AddWithValue("@Position", textBox3.Text);
            cmd.Parameters.AddWithValue("@Phone", textBox4.Text);
            cmd.Parameters.AddWithValue("@Address", textBox5.Text);

            cmd.ExecuteNonQuery();

            reload();

            sqlCon.Close();
        }

        private void Button3_Click_1(object sender, RoutedEventArgs e)
        {
            sqlCon.Open();
            SqlCommand cmd = new SqlCommand("select * from Employee where Id=@Id or Name=@Name or Position=@Position or Phone=@Phone or Address=@Address", sqlCon);

            cmd.Parameters.AddWithValue("@Id", textBox1.Text);
            cmd.Parameters.AddWithValue("@Name", textBox2.Text);
            cmd.Parameters.AddWithValue("@Position", textBox3.Text);
            cmd.Parameters.AddWithValue("@Phone", textBox4.Text);
            cmd.Parameters.AddWithValue("@Address", textBox5.Text);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            //dataGridView1.DataSource = dt;
            dataGrid.ItemsSource = dt.DefaultView;
            sqlCon.Close();
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            sqlCon.Open();
            SqlCommand cmd = new SqlCommand("delete Employee where Id=@Id", sqlCon);

            cmd.Parameters.AddWithValue("@Id", int.Parse(textBox6.Text));

            cmd.ExecuteNonQuery();

            reload();

            sqlCon.Close();
        }

        private void Button5_Click(object sender, RoutedEventArgs e)
        {
            reload();
        }

        private void Button6_Click(object sender, RoutedEventArgs e)
        {
            if (checkBox1.IsChecked == checkBox2.IsChecked)
            {
                MessageBox.Show("Please select the export data format!");
                return;
            }
            sqlCon.Open();
            SqlCommand cmd = new SqlCommand("select * from Employee", sqlCon);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = @"C:\Users\Benson\Desktop";
            sfd.RestoreDirectory = true;

            if (checkBox1.IsChecked == true)
            {
                sfd.FileName = "*.xml";
                sfd.DefaultExt = "xml";
                sfd.Filter = "xml files (*.xml) | *.xml";

                if (sfd.ShowDialog() == true)
                {


                    Stream fileStream = sfd.OpenFile();
                    StreamWriter sw = new StreamWriter(fileStream);
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    ds.WriteXml(sw, XmlWriteMode.WriteSchema);
                    sw.Close();

                    /*
                    Stream fileStream = sfd.OpenFile();
                    StreamWriter sw = new StreamWriter(fileStream);
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    sw.WriteLine(ds.GetXml());
                    sw.Close();
                    */

                }

                sqlCon.Close();
            }

            if (checkBox2.IsChecked == true)
            {
                sfd.FileName = "*.json";
                sfd.DefaultExt = "json";
                sfd.Filter = "json files (*.json) | *.json";

                if (sfd.ShowDialog() == true)
                {
                    Stream fileStream = sfd.OpenFile();
                    StreamWriter sw = new StreamWriter(fileStream);
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);

                    JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
                    List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
                    Dictionary<string, object> childRow;

                    DataTable table = dt;

                    foreach (DataRow row in table.Rows)
                    {
                        childRow = new Dictionary<string, object>();
                        foreach (DataColumn col in table.Columns)
                        {
                            childRow.Add(col.ColumnName, row[col]);
                        }
                        parentRow.Add(childRow);
                    }

                    sw.WriteLine(jsSerializer.Serialize(parentRow));
                    sw.Close();
                    fileStream.Close();
                }

                sqlCon.Close();
            }
        }
    }

}
