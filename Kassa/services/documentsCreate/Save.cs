using MaterialSkin.Controls;
using Microsoft.Data.SqlClient;
using Registrator.repo.models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kassa
{
    
    class Save
    {
        public SqlConnection sqlConnection = null;

        public void SaveData(DataKKT dataKKT, SettingsProgram settings)
        {

            // Заполнение версии программы
            string program_version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            
            try
            {
                string[] zap_znak = { "\"", "\\", "/", ":", "*", "?", "<", ">", "|", "\"" };
                string NameOrganization_save = dataKKT.NameOrganization;
                if (dataKKT.NameOrganization != "")
                {
                    for (int i = 0; i < zap_znak.Length; i++)
                    {
                        NameOrganization_save = NameOrganization_save.Replace(zap_znak[i], "");
                    }
                }

                string Manufacturer_FN = "";
                if (dataKKT.ModelFN != null)
                {
                    if (dataKKT.ModelFN.Length > 5)
                    {
                        Manufacturer_FN = dataKKT.ModelFN.Substring(0, dataKKT.ModelFN.Length - 4);
                    }
                }
                if (NameOrganization_save == "")
                {
                    NameOrganization_save = "Пустой файл";
                }
                if (dataKKT.ID == "")
                {
                    dataKKT.ID = "ID";
                }
                string file = settings.AdressFile;
                FolderBrowserDialog Browserdialog = new FolderBrowserDialog(); //открытие проводника и выбор папки сохраннения
                Browserdialog.RootFolder = Environment.SpecialFolder.Desktop;
                Browserdialog.SelectedPath = settings.AdressFile;

                if (Browserdialog.ShowDialog() == DialogResult.OK)
                {
                    file = Browserdialog.SelectedPath;
                }
                if (settings.CreateFolder == true)
                {
                    //Создание папки
                    string[] arrayFilePath = file.Split('\\'); // Делим полный путь на участки через \
                    bool is_down = false;
                    for (int i = 0; i < arrayFilePath.Length; i++)  // Если в пути файла встречается папка с названием орагнизации, то заново она не создается
                    {
                        if (arrayFilePath[i] == dataKKT.ID + " " + NameOrganization_save)
                        {
                            is_down = true;
                            break;
                        }
                    }
                    if (!is_down)
                    {
                        file += "\\" + dataKKT.ID + " " + NameOrganization_save;
                        Directory.CreateDirectory(file);
                    }
                }
                StreamWriter sw = new StreamWriter(file + "\\" + dataKKT.ID + " " + NameOrganization_save + ".txt");
                
                sw.WriteLine("ЗН ККТ #" + dataKKT.ZN_KKT + " #");
                sw.WriteLine("Модель ККТ #" + dataKKT.ModelKKT + " #");
                sw.WriteLine("Номер автомата #" + dataKKT.NumberAvtomate + " #");
                sw.WriteLine("Номер ФН #" + dataKKT.NumberFN + " #");
                sw.WriteLine("Модель ФН #" + dataKKT.ModelFN + " #");
                sw.WriteLine();
                sw.WriteLine("ID клиента #" + dataKKT.ID + " #");
                sw.WriteLine("Наименование организации #" + dataKKT.NameOrganization + " #");
                sw.WriteLine("ИНН организации #" + dataKKT.INNOrganization + " #");
                sw.WriteLine("КПП организации #" + dataKKT.KPPOrganization + " #");
                sw.WriteLine("Руководитель организации #" + dataKKT.DirectorOrganization + " #");
                sw.WriteLine("ФИО уполномоченного лица #" + dataKKT.NameCashier + " #");
                sw.WriteLine("Телефон #" + dataKKT.Telephone + " #");
                sw.WriteLine("Почта #" + dataKKT.EmailOrganization + " #");
                sw.WriteLine();
                sw.Write("СНО: ОСН #" + dataKKT.SNO_OSN + " #");
                sw.Write("УСН Доход #" + dataKKT.SNO_USN_D + " #");
                sw.Write("УСН Доход - расход #" + dataKKT.SNO_USN_D_R + " #");
                sw.Write("Патент #" + dataKKT.SNO_PATENT + " #");
                sw.Write("ЕСХН #" + dataKKT.SNO_ESHN + " #");
                sw.WriteLine();
                sw.WriteLine("Адрес расчетов #" + dataKKT.AddressPayment + " #");
                sw.WriteLine("Место расчетов #" + dataKKT.PlacePayment + " #");
                sw.WriteLine();
                sw.WriteLine("ОФД #" + dataKKT.NameOFD + " #");
                sw.WriteLine("ИНН ОФД #" + dataKKT.INNOFD + " #");
                sw.WriteLine("Производитель ФН #" + Manufacturer_FN + " #");
                sw.WriteLine();
                sw.WriteLine("РНМ #" + dataKKT.RNM + " #");
                sw.WriteLine("Дата, время #" + dataKKT.DataTimeFD + " #");
                sw.WriteLine("Номер ФД #" + dataKKT.NumberFD + " #");
                sw.WriteLine("ФП #" + dataKKT.FP + " #");
                sw.WriteLine();
                sw.WriteLine("Признак проведения лотереи #" + dataKKT.PrLotereya + " #");
                sw.WriteLine("Признак проведения азартных игр #" + dataKKT.PrAzart + " #");
                sw.WriteLine("Признак деятельности платежного агента #" + dataKKT.PrPlatAgent + " #");
                sw.WriteLine("Применение только в Интернет #" + dataKKT.PrInternet + " #");
                sw.WriteLine("Применение в сфере услуг #" + dataKKT.PrDelivery + " #");
                sw.WriteLine("Признак работы с подакцизными товарами #" + dataKKT.PrAkxiz + " #");
                sw.WriteLine("Признак работы с маркированными товарами #" + dataKKT.PrMark + " #");
                sw.WriteLine();
                sw.WriteLine();
                sw.WriteLine("Версия файла# " + program_version + " #");
                //Close the file
                sw.Close();

               
                MaterialMessageBox.Show(
        "Файл сохранен",
        "Сообщение");
                return;
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show(
        "Ошибка при формировании файла TXT: " + ex,
        "Ошибка");
            }
            finally
            {

            }
        }        
    }
}
