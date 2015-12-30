using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
//using System.Runtime.Serialization.Json;

namespace DataService
{
    public class DatabaseService
    {
        string dbFingerTable = System.Configuration.ConfigurationManager.AppSettings["dbFingerTable"];
        string dbFingerColumn = System.Configuration.ConfigurationManager.AppSettings["dbFingerColumn"];

        //public override byte[] GetImage(IMAGE_TYPE imageType, int id)
        public override byte[] GetImage(IMAGE_TYPE imageType, System.Int32 id)
        {
            //throw new Exception(getConnectionString());

            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            byte[] buffer = new byte[0];

            try
            {
                //conn = buildConnectionString();
                conn = new SqlConnection(getConnectionString());
                conn.Open();

                cmd = new SqlCommand();
                cmd.Connection = conn;

                cmd.CommandText = "SELECT " + dbFingerColumn + " FROM " + dbFingerTable;

                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    //if (!reader.IsDBNull(0))
                    //    id = reader.GetInt32(0);
                    if (!reader.IsDBNull(0))
                    {
                        //binary = reader.GetSqlBinary(1);
                        //if (id != 20031448)
                        //{
                        //    if (IMAGE_TYPE.wsq == imageType)
                        //        buffer = (byte[])reader["wsq"];
                        //    //buffer = (byte[])reader["AppWsq"];
                        //    else
                        //        buffer = (byte[])reader["picture"];
                        //}
                        //else
                        /*
                                                {
                                                    if (IMAGE_TYPE.wsq == imageType)
                                                        buffer = (byte[])reader["AppWsq"];
                                                    else
                                                        buffer = (byte[])reader["AppImage"];
                                                }
                        */
                        if (IMAGE_TYPE.wsq == imageType)
                            buffer = (byte[])reader[dbFingerColumn];  //(byte[])reader["AppWsq"];
                        else
                            buffer = (byte[])reader[dbPictureColumn]; //(byte[])reader["AppImage"];

                        //buffer = (byte[])reader["AppImage"];
                        //int maxSize = 200000;
                        //buffer = new byte[maxSize];
                        //reader.GetBytes(1, 0L, buffer, 0, maxSize);
                    }
                    //else
                    //{
                    //    buffer = new byte[1];
                    //}
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

                    if (conn != null && conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                        conn = null;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return buffer;

        }

        public override void SendImage(IMAGE_TYPE imageType, int id, ref byte[] buffer)
        {
            SqlConnection conn = null;
            SqlCommand cmd = null;
            //object incrementId = null;
            try
            {
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
                                update {0} with (serializable) SET {1} = @image where {2} = @id
                                if @@rowcount = 0 
                                begin
                                    insert into {0} ({2}, {1}) values (@id, @image) 
                                end
                            commit tran ", dbImageTable, dbImageColumn, dbIdColumn);

                cmd.Parameters.Add("@image", SqlDbType.VarBinary);
                cmd.Parameters["@image"].Value = buffer;

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
                    if (conn != null && conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                        conn = null;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }
/*
        private SqlConnection buildConnectionString()
        {
            String serverName = ConfigurationManager.AppSettings["ServerName"];
            if (serverName.Length == 0)
                serverName = "Data Source=" + decrypt(PSCBioVerification.Credentials.Default.ServerName, "PSC");
            else
                serverName = "Data Source=" + serverName;

            if (PSCBioVerification.Credentials.Default.IntegratedSecurity == true) {
                return new SqlConnection(
                    serverName +
                    //"Data Source=." +
                    //"Data Source=" + decrypt(PSCBioVerification.Credentials.Default.ServerName, "PSC") +
                    ";Database=" + decrypt(PSCBioVerification.Credentials.Default.DataBaseName, "PSC") +
                    ";Integrated Security=True");
            } else {
                return new SqlConnection(
                    serverName +
                    //"Data Source=" + decrypt(PSCBioVerification.Credentials.Default.ServerName, "PSC") +
                    ";Database=" + decrypt(PSCBioVerification.Credentials.Default.DataBaseName, "PSC") +
                    ";User ID=" + decrypt(PSCBioVerification.Credentials.Default.DBUser, "PSC") +
                    ";Password=" + decrypt(PSCBioVerification.Credentials.Default.DBPass, "PSC"));
            }
        }
*/
        private String getConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        }
/*
        private static String decrypt(String strEncrypted, String strKey) {
            try
            {
                var objDESCrypto = new TripleDESCryptoServiceProvider();
                var objHashMD5 = new MD5CryptoServiceProvider();

                byte[] byteHash, byteBuff;
                String strTempKey = strKey;

                byteHash = objHashMD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(strTempKey));
                objHashMD5 = null;
                objDESCrypto.Key = byteHash;
                objDESCrypto.Mode = CipherMode.ECB; //CBC, CFB

                byteBuff = Convert.FromBase64String(strEncrypted);
                String strDecrypted = ASCIIEncoding.ASCII.GetString(objDESCrypto.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length));
                objDESCrypto = null;

                return strDecrypted;
            }
            catch (Exception ex)
            {
                return "Wrong Input. " + ex.Message;
            }
        }
*/
    }
}
