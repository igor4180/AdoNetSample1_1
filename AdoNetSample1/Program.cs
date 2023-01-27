using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoNetSample1
{
    class Program
    {
        SqlConnection conn = null;

        public Program()
        {
            conn = new SqlConnection();
            //conn.ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=Library; Integrated Security=SSPI;";
            string cs = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
            conn.ConnectionString = cs;
        }

        static void Main(string[] args)
        {
            Program pr = new Program();
            //pr.InsertQuery();
            //pr.ReadData();
            pr.ReadData2();
            //pr.ExecStoredProcedure();
        }

        public void InsertQuery()
        {
            try
            {
                // Open the connection
                conn.Open();

                // prepare command string
                string insertString = @"
                 insert into Sales Manager
                 (FirstName, LastName) 
                 values ('Roger', 'Zelazny')";

                // 1. Instantiate a new command with a query and connection
                SqlCommand cmd = new SqlCommand(insertString, conn);

                // 2. Call ExecuteNonQuery to send command
                cmd.ExecuteNonQuery();
            }
            finally
            {
                // Close the connection
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public void ReadData()
        {
            SqlDataReader rdr = null;

            try
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("select * from Sales Manager", conn);

                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Console.WriteLine(rdr[1] +" "+ rdr[2]);
                }
            }
            finally
            {
                // close the reader
                if (rdr != null)
                {
                    rdr.Close();
                }

                // Close the connection
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public void ReadData2()
        {
            SqlDataReader rdr = null;

            try
            {
                // Open the connection
                conn.Open();

                SqlCommand cmd = new SqlCommand("select * from Sales Manager; select * from Type;", conn);

                rdr = cmd.ExecuteReader();
                int line = 0;

                // извлечь полученные строки
                do
                {
                    while (rdr.Read())
                    {
                        if (line == 0)  // формируем шапку таблицы перед выводом первой строки  
                        {
                            // цикл по числу прочитанных полей
                            for (int i = 0; i < rdr.FieldCount; i++)
                            {
                                // вывести в консольное окно имена полей	
                                Console.Write(rdr.GetName(i).ToString() + "\t");
                            }
                            Console.WriteLine();
                        }
                        //Console.WriteLine();
                        line++;
                        Console.WriteLine(rdr[0] + "\t" + rdr[1] + "\t" + rdr[2]);
                    }
                    Console.WriteLine("Total records processed: " + line.ToString());
                } while (rdr.NextResult());

            }
            finally
            {
                // close the reader
                if (rdr != null)
                {
                    rdr.Close();
                }

                // Close the connection
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        public void ExecStoredProcedure()
        {
            conn.Open();
            SqlCommand cmd = new SqlCommand("getTypeNumber", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@Sales ManagerId", System.Data.SqlDbType.Int).Value = 1;

            SqlParameter outputParam = new SqlParameter("@TypeCount", System.Data.SqlDbType.Int);
            outputParam.Direction = ParameterDirection.Output;
            //outputParam.Value = 0;
            cmd.Parameters.Add(outputParam);

            cmd.ExecuteNonQuery();
            Console.WriteLine(cmd.Parameters["@TypeCount"].Value.ToString());

        }
    }
}
