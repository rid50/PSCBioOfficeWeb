using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;
using System.ServiceModel.Activation;
using DAO;

namespace BiometricsService
{
    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DbDataService : IDbDataService
    {
        public byte[] GetImage(IMAGE_TYPE imageType, int id)
        {
            DataSource ds = new Database();
            return ds.GetImage(imageType, id)[0];
        }

        public void SendImage(IMAGE_TYPE imageType, int id, ref byte[] buffer)
        {
            DataSource ds = new Database();
            ds.SendImage(imageType, id, ref buffer);
        }
/*        
        
        
        
        string dbPictureTable;
        string dbFingerTable;

        string dbIdColumn = System.Configuration.ConfigurationManager.AppSettings["dbIdColumn"];
        string dbPictureColumn = System.Configuration.ConfigurationManager.AppSettings["dbPictureColumn"];
        string dbFingerColumn = System.Configuration.ConfigurationManager.AppSettings["dbFingerColumn"];

        internal DbDataService()
        {
            dbPictureTable = getAppSetting("dbPictureTable");
            dbFingerTable = getAppSetting("dbFingerTable");
        }

        //[System.ServiceModel.OperationBehavior(Impersonation = System.ServiceModel.ImpersonationOption.Required)]
        public byte[] GetImage(IMAGE_TYPE imageType, int id)
        {
            //throw new Exception(
            //    "Windows Identity: " + System.Security.Principal.WindowsIdentity.GetCurrent().Name + '\n' +
            //    "Thread Impersonation level : " + System.Security.Principal.WindowsIdentity.GetCurrent().ImpersonationLevel + '\n' +
            //    "Token: " + System.Security.Principal.WindowsIdentity.GetCurrent().Token.ToString() + '\n' +
            //    "Thread Identity: " + System.Threading.Thread.CurrentPrincipal.Identity.Name
            //);

            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            byte[] buffer = null;

            try
            {
                conn = new SqlConnection(getConnectionString());

                conn.Open();

                cmd = new SqlCommand();
                cmd.Connection = conn;

                if (imageType == IMAGE_TYPE.picture)
                    cmd.CommandText = "SELECT " + dbPictureColumn + " FROM " + dbPictureTable + " WHERE " + dbIdColumn + " = @id";
                else
                    cmd.CommandText = "SELECT " + dbFingerColumn + " FROM " + dbFingerTable + " WHERE " + dbIdColumn + " = @id";

                //cmd.Parameters.Add(new SqlCeParameter("@id", SqlDbType.Int));   // doesn't work
                cmd.Parameters.AddWithValue("@id", id);

                reader = cmd.ExecuteReader();
                //reader.Read();

                //SqlBinary binary;
                //SqlBytes bytes;

                //                if (reader.HasRows)   //Does not work for CE
                if (reader.Read())
                {
                    //if (!reader.IsDBNull(0))
                    //    id = reader.GetInt32(0);
                    if (!reader.IsDBNull(0))
                    {
                        //binary = reader.GetSqlBinary(1);
                        if (imageType == IMAGE_TYPE.picture)
                            buffer = (byte[])reader[dbPictureColumn]; //(byte[])reader["AppImage"];
                        else
                            buffer = (byte[])reader[dbFingerColumn]; //(byte[])reader["AppImage"];

                        //int maxSize = 200000;
                        //buffer = new byte[maxSize];
                        //reader.GetBytes(1, 0L, buffer, 0, maxSize);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                try
                {
                    if (reader != null)
                        reader.Close();

                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                    if (conn != null)
                        conn = null;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return buffer;

        }

        //[System.ServiceModel.OperationBehavior(Impersonation = System.ServiceModel.ImpersonationOption.Required)]
        public void SendImage(IMAGE_TYPE imageType, int id, ref byte[] buffer)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            //object incrementId = null;
            try
            {
                //System.Windows.Forms.MessageBox.Show("connect str1: " + getConnectionString());
                conn = new SqlConnection(getConnectionString());

                conn.Open();

                cmd = new SqlCommand();
                cmd.Connection = conn;

                string dbImageTable, dbImageColumn;
                if (imageType == IMAGE_TYPE.picture)
                {
                    dbImageTable = dbPictureTable;
                    dbImageColumn = dbPictureColumn;
                }
                else
                {
                    dbImageTable = dbFingerTable;
                    dbImageColumn = dbFingerColumn;
                }

                cmd.CommandText = String.Format(@"
                            begin tran
                                update {0} with (serializable) SET {1} = @picture where {2} = @id
                                if @@rowcount = 0 
                                begin
                                    insert into {0} ({2}, {1}) values (@id, @picture) 
                                end
                            commit tran ", dbImageTable, dbImageColumn, dbIdColumn);

                cmd.Parameters.Add("@picture", SqlDbType.VarBinary);
                cmd.Parameters["@picture"].Value = buffer;

                cmd.Parameters.Add("@id", SqlDbType.Int);
                cmd.Parameters["@id"].Value = id;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
                throw new Exception(ex.Message);
            }
            finally
            {
                try
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                    if (conn != null)
                        conn = null;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
*/
/*
        private String getConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        }

        private String getAppSetting(string key)
        {
            var setting = ConfigurationManager.AppSettings[key];
            // If we didn't find setting, try to load it from current dll's config file
            if (string.IsNullOrEmpty(setting))
            {
                var filename = System.Reflection.Assembly.GetExecutingAssembly().Location;
                var configuration = ConfigurationManager.OpenExeConfiguration(filename);
                if (configuration != null)
                    setting = configuration.AppSettings.Settings[key].Value;
            }

            return setting;
        }
*/
    }
}
