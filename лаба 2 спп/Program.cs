using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace спп_2_лаба
{
    class Program
    {

        static List<string> Tables = new List<string> { };
        static List<string> Columns = new List<string> { };

        struct ForeignKey
        {
            public string table;
            public string column;

        }
        //static Dictionary<string, string> ForeignKeys = new Dictionary<string, string>();
        static List<ForeignKey> listForeignKeys = new List<ForeignKey>();
        static string PrimaryKey = "";

        static void Main(string[] args)
        {
               string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=Database.accdb;";
           // string connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;";
            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                bool bl = true ;
                string tablename = "";
                // int a;
                while (bl == true)
                {
                    try
                    {
                        connection.Open();
                        setTables(connection);
                        Console.WriteLine("Выберете таблицу для работы");
                        for (int i = 0; i < Tables.Count; i++)
                        {
                            Console.WriteLine((i + 1) + ")" + Tables[i]);
                        }
                        Console.WriteLine((Tables.Count + 1) + ")Выход");
                        int a = Convert.ToInt32(Console.ReadLine());
                        if (a == Tables.Count + 1) { bl = false; break; }
                        if (a < 1 || a > Tables.Count + 1)
                        {
                            Console.WriteLine("Ошибка выбора таблицы");
                            Console.ReadKey();
                            Console.Clear();
                        }
                        else
                        {
                            tablename = Tables[a - 1];
                            switch_menu(connection, tablename);
                        }
                        connection.Close();
                    }
                    catch (InvalidOperationException exp)
                    {
                        Console.WriteLine("Ошибка объекта: " + exp.Message);
                        Console.ReadKey();
                        Console.Clear();
                        connection.Close();
                    }
                    catch (OleDbException ex)
                    {
                        Console.WriteLine("Ошибка источника данных: " + ex.Message);
                        Console.ReadKey();
                        Console.Clear();
                        connection.Close();
                    }
                    catch (FormatException e)
                    {
                        Console.WriteLine("Ошибка формата: " + e.Message);
                        Console.ReadKey();
                        Console.Clear();
                        connection.Close();
                    }
                    catch (Exception er)
                    {
                        Console.WriteLine("Ошибка: " + er.Message);
                        Console.ReadKey();
                        Console.Clear();
                        connection.Close();
                    }
                }
            }
        }

        static int menu(string table)
        {
            Console.Clear();
            Console.WriteLine("Выбрана таблица " + table +". Выберете функцию");
            Console.WriteLine("1) Select");
            Console.WriteLine("2) Insert");
            Console.WriteLine("3) Delete");
            Console.WriteLine("4) Update");
            Console.WriteLine("5) Выбрать таблицу");
            int a = Convert.ToInt32(Console.ReadLine());
            return a;
        }

        static void switch_menu(OleDbConnection connection, string tablename)
        {
            bool ex = true; 
            while (ex)
            {
                switch (menu(tablename))
                {
                    case 1:
                        {
                            Console.Clear();
                            SelectAll(connection, tablename);
                            break;
                        }
                    case 2:
                        {
                            Console.Clear();
                            Insert(connection, tablename);
                            break;
                        }
                    case 3:
                        {
                            Console.Clear();
                            Console.WriteLine("Введите первичный ключ");
                            string id = Console.ReadLine();
                            Delete(connection, tablename, id);
                            break;
                        }
                    case 4:
                        {
                            Console.Clear();
                            Console.WriteLine("Введите первичный ключ");
                            string id = Console.ReadLine();
                            Update(connection, tablename, id);
                            break;
                        }
                    case 5:
                        {
                            ex = false;
                            break;
                        }
                    case 6:
                        {
                            setForeignKey(connection, tablename);
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("Неккоректно выбрана функция");
                            //Console.ReadKey();
                           // Console.Clear();
                            break;
                        }
                }
                Console.WriteLine("Для продолжения нажмите любую кнопку");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static DataTable createDataTable(string tableName, OleDbCommand myOleDbCommand)
        {
                OleDbDataAdapter adapter = new OleDbDataAdapter();
                adapter.SelectCommand = myOleDbCommand;
                adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
                DataSet myDataset = new DataSet();
                adapter.Fill(myDataset, tableName);
                return myDataset.Tables[tableName];
        }

        static void SelectRow(DataTable myDataTable, OleDbConnection connection)
        {
            if (myDataTable.Rows.Count == 0)
            {
                Console.WriteLine("Таблица не имеет строк");
                return;
            }
            /*
            for (int i = 0; i < myDataTable.Columns.Count; i++)
            {
                if (myDataTable.Columns[i].DataType.Name == "Int32")
                    Console.Write("{0,-10}|", myDataTable.Columns[i].ToString());
                else if (myDataTable.Columns[i].DataType.Name == "String")
                    Console.Write("{0,-25}|", myDataTable.Columns[i].ToString());
                else Console.Write("{0,-10}|", myDataTable.Columns[i].ToString());
            }
            */
            setColumns(connection, myDataTable.TableName);
            setPrimaryKey(connection, myDataTable);
          //  Console.Write("{0,-7} PK |", PrimaryKey);
            for (int i=0; i< myDataTable.Columns.Count; i++)
            {
                if (myDataTable.Columns[i].ToString() != PrimaryKey)
                {
                    if (myDataTable.Columns[i].DataType.Name == "Int32")
                        Console.Write("{0,-11}|", myDataTable.Columns[i].ToString());
                    else if (myDataTable.Columns[i].DataType.Name == "String")
                        Console.Write("{0,-25}|", myDataTable.Columns[i].ToString());
                    else Console.Write("{0,-11}|", myDataTable.Columns[i].ToString());
                } else
                {
                    if (myDataTable.Columns[i].DataType.Name == "Int32")
                        Console.Write("{0,-5} (PK) |", myDataTable.Columns[i].ToString());
                    else if (myDataTable.Columns[i].DataType.Name == "String")
                        Console.Write("{0,-20} (PK) |", myDataTable.Columns[i].ToString());
                    else Console.Write("{0,-5} (PK) |", myDataTable.Columns[i].ToString());
                }
            }
            //
            Console.WriteLine("\n---------------------------------------------------------------------------");
            foreach (DataRow dr in myDataTable.Rows)
            {
                //dr.
                //  List<string> row = dr.ItemArray.ToList<string[]>();
                 
                for (int i=0; i< dr.Table.Columns.Count; i++)
                {
                  if (dr[i].GetType().Name == "Int32") 
                    Console.Write("{0,-11}|", dr[i]) ;
                  else if (dr[i].GetType().Name == "String")
                    Console.Write("{0,-25}|", dr[i]);
                  else Console.Write("{0,-11}|", dr[i]);
                }
                Console.Write("\n");
                //   Console.WriteLine("{0,-4}|{1,-24}|{2,-24}|{3,-17}", dr[0], dr[1], dr[2], dr[3]);
                //    Console.WriteLine();
            }
        }


        static void SelectAll(OleDbConnection connection,string table)
        {
            setColumns(connection, table);
            // создаем объект OleDbCommand
            OleDbCommand myOleDbCommand = connection.CreateCommand();
            myOleDbCommand.CommandText =
                    "SELECT " + getColumnsText() + 
                        "FROM [" + table + "]" ;
        //    setPrimaryKey(connection, table);
            SelectRow(createDataTable(table, myOleDbCommand),connection);
        }


        static void Insert(OleDbConnection connection, String table)
        {
            //  if (SearchUserById(connection, id_u).Rows.Count == 0)
            //   {
            //      Console.WriteLine("Ошибка. Пользователя с id=" + id_u + " не найдено");
            //       return;
            //  }
            // setPrimaryKey(connection)
            // DataTable data = GetDataTable(connection, table);
            setColumns(connection, table);
            setPrimaryKey(connection, GetDataTable(connection, table));
            if (Columns.Count == 0)
            {
                Console.WriteLine("Ошибка. Отсуствуют столбцы кроме первичного");
                return;
            }
            string[] param = new string[Columns.Count] ;
            for (int i=0; i< Columns.Count; i++)
            {

                Console.WriteLine("Введите " + Columns[i]);
                param[i] = Console.ReadLine();
            }
            string str = "";
            OleDbCommand myOleDbCommand = connection.CreateCommand();
            for (int i=0; i< param.Length; i++)
            {
                if (i == param.Length - 1) str += "@param" + i  + " ";
                else  str += "@param" + i + ",";
                myOleDbCommand.Parameters.AddWithValue("@param" + i, param[i]);
            }
            myOleDbCommand.CommandText =
                    @" INSERT INTO [" + table + "] ( " + getColumnsText() + " ) VALUES ( " +
                    str + " ) ";
            myOleDbCommand.ExecuteNonQuery();
            Console.WriteLine("Данные успешно добавлены в " + table);
        }

        static void Update(OleDbConnection connection, string table, string id)
        {
            if (SearchById(connection, table, id).Rows.Count == 0)
            {
                Console.WriteLine("Ошибка. Строки с первичным ключом =" + id + " не найдено");
                return;
            }
            //
            string column = "";
            bool bl = true;
            setColumns(connection, table);
            setPrimaryKey(connection, GetDataTable(connection, table));
            while (bl)
            {
                Console.WriteLine("Выберете столбец для работы");
                for (int i = 0; i < Columns.Count; i++)
                {
                    Console.WriteLine((i + 1) + ")" + Columns[i]);
                }
                Console.WriteLine((Columns.Count + 1) + ") Назад к выбору функций");
                int a = Convert.ToInt32(Console.ReadLine());
                if (a == Columns.Count + 1) { bl = false; return; }
                if (a < 1 || a > Tables.Count + 1)
                {
                    Console.WriteLine("Ошибка выбора таблицы");
                }
                else
                {
                    column = Columns[a - 1];
                }

                //
                //setColumns(connection, table);
                //setPrimaryKey(connection, GetDataTable(connection, table));
                /*if (Columns.Count == 0)
                {
                    Console.WriteLine("Ошибка. Отсуствуют столбцы кроме первичного");
                    return;
                }
                string[] param = new string[Columns.Count];
                for (int i = 0; i < Columns.Count; i++)
                {

                    Console.WriteLine("Введите " + Columns[i]);
                    param[i] = Console.ReadLine();
                }
                string str = "";
                */
                OleDbCommand myOleDbCommand = connection.CreateCommand();
                /*for (int i = 0; i < param.Length; i++)
                {
                    if (i == param.Length - 1) str += "[" + Columns[i] +  "]=@param" + i + " ";
                    else str += "["  + Columns[i]  +"]=@param" + i + ", ";
                    myOleDbCommand.Parameters.AddWithValue("@param" + i, param[i]);
                } */
                //
                Console.WriteLine("Введите " + column);
                string str = Console.ReadLine();
                myOleDbCommand.Parameters.AddWithValue("@param", str);
                //
                myOleDbCommand.CommandText = "UPDATE [" + table + "] SET [" + column + "]=@param"
                    + " WHERE [" + PrimaryKey + "]=" + id;
                myOleDbCommand.ExecuteNonQuery();
                Console.WriteLine("Данные изменены");
                Console.ReadKey();
                Console.Clear();
            }
        }

        static void Delete(OleDbConnection connection, string table, string id)
        {
            if (SearchById(connection, table, id).Rows.Count == 0)
            {
                Console.WriteLine("Ошибка. Строки с первичным ключом =" + id + " не найдено");
                return;
            }
            setForeignKey(connection, table);
            int a = 1;
            if (listForeignKeys.Count != 0)
            {
                for (int i = 0; i < listForeignKeys.Count; i++) {
                    if (SearchByColumn(connection, listForeignKeys[i].table ,listForeignKeys[i].column , id).Rows.Count !=0)
                    {
                        Console.WriteLine("У этого значения есть вторичный ключ. 1 - удалить все строки, 2 - не удалять");
                        a = Convert.ToInt32(Console.ReadLine());
                        if (a == 1)
                        {
                            DeleteFKTable(connection, listForeignKeys[i].table, listForeignKeys[i].column, id);
                        }
                    }
                }
            }
            if (a == 2)
            {
                //Console.WriteLine("");
                return;
            }
            if (a != 1 && a != 2 )
            {

                Console.WriteLine("Ошибка выбора. Введено неверное значение");
               // Console.ReadKey();
                return;
            }
            setColumns(connection, table);
            setPrimaryKey(connection, GetDataTable(connection, table));
            OleDbCommand myOleDbCommand = connection.CreateCommand();
            myOleDbCommand.CommandText = "DELETE CASCADE FROM [" + table + "] WHERE [" + PrimaryKey + "]=" + id;
            myOleDbCommand.ExecuteNonQuery();
            Console.WriteLine("Строки успешно удалены ");
        }

        static DataTable SearchById(OleDbConnection connection,string table, string id)
        {
            setColumns(connection, table);
            setPrimaryKey(connection, GetDataTable(connection, table));
            OleDbCommand myOleDbCommand = connection.CreateCommand();
            myOleDbCommand.CommandText =
                    "SELECT " + getColumnsText() +
                        "FROM [" + table + "] WHERE [" +PrimaryKey + "]=" + id ;
            return createDataTable(table, myOleDbCommand);
        }

        static DataTable SearchByColumn(OleDbConnection connection, string table, string column, string id)
        {
            setColumns(connection, table);
            setPrimaryKey(connection, GetDataTable(connection, table));
            OleDbCommand myOleDbCommand = connection.CreateCommand();
            myOleDbCommand.CommandText =
                    "SELECT " + column +
                        " FROM [" + table + "] WHERE [" + column + "]=" + id;
            return createDataTable(table, myOleDbCommand);
        }

        static void DeleteFKTable(OleDbConnection connection, string table, string column, string id)
        {
            OleDbCommand myOleDbCommand = connection.CreateCommand();
            myOleDbCommand.CommandText = "DELETE FROM [" + table + "] WHERE [" + column + "]=" + id;
            myOleDbCommand.ExecuteNonQuery();
        }


        static DataTable GetDataTable(OleDbConnection connection, string table)
        {
         //   setColumns(connection, table);
            OleDbCommand myOleDbCommand = connection.CreateCommand();
            myOleDbCommand.CommandText =
                    "SELECT " + getColumnsText() +
                        "FROM [" + table + "]";
            return createDataTable(table, myOleDbCommand);
        }

        static void setTables(OleDbConnection connection)
        {
            Tables.Clear();
            DataTable dt = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
            foreach (DataRow item in dt.Rows)
            {
                Tables.Add((string)item["TABLE_NAME"]);
            }
        }

        static void setColumns(OleDbConnection connection, string table)
        {
            Columns.Clear();
            DataTable schemaTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Columns,
               new object[] { null, null, table, null });
            foreach (DataRow column in schemaTable.Rows)
            {
                Columns.Add((string)column["COLUMN_NAME"]);
            }
        }

        static void setForeignKey(OleDbConnection connection, string table)
        {
            Columns.Clear();
            DataTable schemaTable = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys,
               new object[] { null, null, table, null });
            foreach (DataRow column in schemaTable.Rows)
            {
                listForeignKeys.Clear();
                ForeignKey foreignKey = new ForeignKey();
                foreignKey.table = column["FK_TABLE_NAME"].ToString();
                foreignKey.column = column["FK_COLUMN_NAME"].ToString();
                //Columns.Add((string)column["COLUMN_NAME"]);
                // Console.WriteLine(column["FK_COLUMN_NAME"]);
                listForeignKeys.Add(foreignKey);
            }
        }

        static string getColumnsText()
        {
            string str = " ";
            for (int i = 0; i < Columns.Count; i++)
            {
                if (i == Columns.Count - 1) str +=  " [" + Columns[i] + "] ";
                else str += " [" + Columns[i] + "] ,";
            }
            return str;
        }

        static void setPrimaryKey(OleDbConnection connection, DataTable table)
        {
          foreach(DataColumn dc in table.PrimaryKey)
            {
                if (Columns.IndexOf(dc.ColumnName) != -1)
                {
                    Columns.Remove(dc.ColumnName);
                    PrimaryKey = dc.ColumnName;
                }
            }
        }

    }
}
