using MaterialSkin.Controls;
using Registrator.models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.repo
{
    public class OFDandFN
    {
        private readonly string connectionString;

        public OFDandFN()
        {
            connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;
        }

        private SQLiteConnection GetConnection()
        {
            var conn = new SQLiteConnection(connectionString);
            conn.Open();
            return conn;
        }

        // Получение настроек OFD по имени
        public OptionsOFD GetOptionsOFDByName(string nameOFD)
        {
            using (var conn = GetConnection())
            {
                string query = @"
                SELECT 
                    inn_OFD, 
                    email_OFD, 
                    adress_OFD, 
                    IP_OFD, 
                    TCP_OFD, 
                    DNS_OFD, 
                    adress_OISM_OFD,
                    port_OFD 
                FROM options_OFD 
                WHERE name_OFD = @nameOFD";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nameOFD", nameOFD);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new OptionsOFD
                            {
                                INN = reader["inn_OFD"].ToString(),
                                Email = reader["email_OFD"].ToString(),
                                URL = reader["adress_OFD"].ToString(),
                                IP = reader["IP_OFD"].ToString(),
                                TCP = reader["TCP_OFD"].ToString(),
                                DNS = reader["DNS_OFD"].ToString(),
                                URL_OISM = reader["adress_OISM_OFD"].ToString(),
                                TCP_OISM = reader["port_OFD"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        // Получение настроек FN по имени
        public OptionsFN GetOptionsFNByName(string nameFN)
        {
            using (var conn = GetConnection())
            {
                string query = @"
                SELECT adress_FN, port_FN FROM options_FN WHERE name_FN = @nameFN";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nameFN", nameFN);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new OptionsFN
                            {
                                URL = reader["adress_FN"].ToString(),
                                TCP = reader["port_FN"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        // Получение списка моделей FN
        public List<string> GetModelFNs()
        {
            var list = new List<string>();
            using (var conn = GetConnection())
            {
                string query = "SELECT model_FN FROM table_model_FN";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(reader["model_FN"].ToString());
                        }
                    }
                }
            }
            return list;
        }

        // Получение списка имен OFD
        public List<string> GetNamesOfd()
        {
            var list = new List<string>();
            using (var conn = GetConnection())
            {
                string query = "SELECT name_OFD FROM options_OFD ORDER BY id_ofd";
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(reader["name_OFD"].ToString());
                        }
                    }
                }
            }
            return list;
        }

        // Обновление параметра по имени
        public bool UpdateParameter(string parameterName, string value)
        {
            using (var conn = GetConnection())
            {
                string query = @"
                UPDATE options_program
                SET meaning = @value
                WHERE parameter = @parameterName";

                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@value", value);
                    cmd.Parameters.AddWithValue("@parameterName", parameterName);
                    try
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }
        public bool InsertOfdRecord(OptionsOFD record)
        {
            // SQL-запрос для вставки
            string query = @"
        INSERT INTO options_OFD 
        (name_OFD, inn_OFD, email_OFD, adress_OFD, IP, TCP, DNS, port_OFD, adress_OISM_OFD)
        VALUES 
        (@NameOFD, @InnOFD, @EmailOFD, @Adress, @IpOFD, @TcpOFD, @DnsOFD, @PortOFD, @AdressOISM)";

            try
            {
                using (SQLiteConnection sqliteConnection = new SQLiteConnection(connectionString))
                {
                    sqliteConnection.Open();
                    using (SQLiteCommand cmd = new SQLiteCommand(query, sqliteConnection))
                    {
                        // Добавляем параметры из модели
                        cmd.Parameters.AddWithValue("@NameOFD", record.Name ?? string.Empty);
                        cmd.Parameters.AddWithValue("@InnOFD", record.INN ?? string.Empty);
                        cmd.Parameters.AddWithValue("@EmailOFD", record.Email ?? string.Empty);
                        cmd.Parameters.AddWithValue("@Adress", record.URL ?? string.Empty);
                        cmd.Parameters.AddWithValue("@IpOFD", record.IP ?? string.Empty);
                        cmd.Parameters.AddWithValue("@TcpOFD", record.TCP ?? string.Empty);
                        cmd.Parameters.AddWithValue("@DnsOFD", record.DNS ?? string.Empty);
                        cmd.Parameters.AddWithValue("@PortOFD", record.TCP_OISM ?? string.Empty);
                        cmd.Parameters.AddWithValue("@AdressOISM", record.URL_OISM ?? string.Empty);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show("Ошибка при обновлении:", ex.Message);
                return false;
            }
        }
    }
}
