using Registrator.repo.models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Registrator.repo
{
    // Класс для загрузки настроек
    public class SettingsLoader
    {
        private readonly SQLiteConnection connection;
        public string connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;
        SettingsProgram settings = new SettingsProgram();

        public SettingsLoader()
        {
            // Инициализация и открытие соединения
            connection = new SQLiteConnection(connectionString);
            connection.Open();
        }

        public SettingsProgram GetSettings()
        {            
            string GetParameterValue(string parameterName)
            {
                string query = "SELECT * FROM options_program WHERE parameter = @param";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@param", parameterName);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (string)reader["meaning"];
                        }
                    }
                }
                return null;
            }

            // Получение и присвоение значений
            settings.AdressFile = GetParameterValue("adr_file");
            settings.StandartOFD = GetParameterValue("standart_OFD");
            settings.StandartModelFN = GetParameterValue("standart_FN");
            settings.NameOperator = GetParameterValue("name_operator");
            settings.PortName = GetParameterValue("port_name");

            // Обработка булевых значений
            string delXmlStr = GetParameterValue("del_xml");
            settings.DeleteXML = delXmlStr == "true";

            string printAktStr = GetParameterValue("print_akt");
            settings.PrintAkt = printAktStr == "true";

            string createFolderStr = GetParameterValue("create_folder");
            settings.CreateFolder = createFolderStr == "true";

            return settings;
        }
        public void Dispose()
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }
        }
    }
}
