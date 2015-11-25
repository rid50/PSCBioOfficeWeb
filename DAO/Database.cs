using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using DAO.ConfigurationService;
using System.Text;
//using System.Runtime.Serialization.Json;

namespace DAO
{
    public class Database : DataSource
    {
        static ConfigurationServiceClient configurationServiceClient;

        static string dbPictureTable;
        static string dbFingerTable;
        static string dbIdColumn;
        static string dbPictureColumn;
        static string dbFingerColumn;
        string fingerFields = "li,lm,lr,ll,ri,rm,rr,rl,lt,rt";

        static Database()
        {
            //System.Diagnostics.Debug.Listeners.Add(new System.Diagnostics.TextWriterTraceListener(@"c:\temp\debug.log"));

            //BasicHttpBinding basicHttpbinding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            //basicHttpbinding.Name = "BasicHttpBinding_IConfigurationService";
            //basicHttpbinding.MessageEncoding = WSMessageEncoding.Mtom;
            //basicHttpbinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            //basicHttpbinding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

            //EndpointAddress endpointAddress = new EndpointAddress("http://biooffice/WcfSiteConfigurationService/ConfigurationService.svc");
            //configurationServiceClient = new ConfigurationServiceClient(basicHttpbinding, endpointAddress);

            //System.Diagnostics.Debug.WriteLine("kuku", "DEBUG2::");
            //System.Diagnostics.Debug.Close();

            configurationServiceClient = new ConfigurationServiceClient();
            //System.Diagnostics.Debug.WriteLine("kuku2", "DEBUG2::");
            //System.Diagnostics.Debug.Close();

            dbPictureTable = configurationServiceClient.getAppSetting("dbPictureTable");
            dbFingerTable = configurationServiceClient.getAppSetting("dbFingerTable");
            dbIdColumn = configurationServiceClient.getAppSetting("dbIdColumn");
            dbPictureColumn = configurationServiceClient.getAppSetting("dbPictureColumn");
            dbFingerColumn = configurationServiceClient.getAppSetting("dbFingerColumn");
        }

        //string dbPictureTable = System.Configuration.ConfigurationManager.AppSettings["dbPictureTable"];
        //string dbFingerTable = System.Configuration.ConfigurationManager.AppSettings["dbFingerTable"];
        //string dbIdColumn = System.Configuration.ConfigurationManager.AppSettings["dbIdColumn"];
        //string dbPictureColumn = System.Configuration.ConfigurationManager.AppSettings["dbPictureColumn"];
        //string dbFingerColumn = System.Configuration.ConfigurationManager.AppSettings["dbFingerColumn"];

        //public override byte[] GetImage(IMAGE_TYPE imageType, int id)
        public override byte[][] GetImage(IMAGE_TYPE imageType, System.Int32 id)
        {
            //throw new Exception(getConnectionString());

            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            byte[][] buffer = new byte[11][];
            //byte[] buffer = new byte[0];

            try
            {
                //conn = buildConnectionString();
                conn = new SqlConnection(configurationServiceClient.getConnectionString("ConnectionString"));
                conn.Open();

                cmd = new SqlCommand();
                cmd.Connection = conn;

                //if (id != 20031448)
                //{
                //    if (IMAGE_TYPE.wsq == imageType)
                //        cmd.CommandText = "SELECT wsq FROM visitors WHERE id = @id";
                //    else
                //        cmd.CommandText = "SELECT picture FROM visitors WHERE id = @id";
                //}
                //else
/*
                {
                    if (IMAGE_TYPE.wsq == imageType)
                        //cmd.CommandText = "SELECT AppId, AppWsq FROM v_fingerprintverify WHERE ChkId = @id";
                        cmd.CommandText = "SELECT AppWsq FROM T_AppPers WHERE AppId = @id";
                    else
                        //cmd.CommandText = "SELECT AppId, AppImage FROM v_fingerprintverify WHERE ChkId = @id";
                        cmd.CommandText = "SELECT AppImage FROM T_AppPers WHERE AppId = @id";
                }
*/
                if (IMAGE_TYPE.picture == imageType)
                    cmd.CommandText = "SELECT " + dbPictureColumn + " FROM " + dbPictureTable + " WHERE " + dbIdColumn + " = @id";
                else if (IMAGE_TYPE.wsq == imageType)
                    cmd.CommandText = "SELECT " + dbFingerColumn + "," + fingerFields + " FROM " + dbFingerTable + " WHERE " + dbIdColumn + " = @id";
                    //cmd.CommandText = "SELECT " + dbFingerColumn + " FROM " + dbFingerTable + " WHERE " + dbIdColumn + " = @id";
                //else if (IMAGE_TYPE.fingerTemplates == imageType)
                    //cmd.CommandText = "SELECT li,lm,lr,ll,ri,rm,rr,rl,lt,rt FROM " + dbFingerTable + " WHERE " + dbIdColumn + " = @id";
                else
                    throw new Exception("unknown image type");

                //cmd.Parameters.Add(new SqlCeParameter("@id", SqlDbType.Int));   // doesn't work
                cmd.Parameters.AddWithValue("@id", id);

                //cmd.Parameters.Add("@id", SqlDbType.Int);
                //cmd.Parameters[0].Value = id;

                reader = cmd.ExecuteReader();
                //reader.Read();

                //SqlBinary binary;
                //SqlBytes bytes;

                //                if (reader.HasRows)   //Does not work for CE
                if (reader.Read())
                {

                    if (!reader.IsDBNull(0) && ((byte[])reader[0]).Length != 1)
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
                        {
                            buffer[0] = (byte[])reader[dbFingerColumn];  //(byte[])reader["AppWsq"];

                            string[] result = fingerFields.Split(new char[] { ',' });

                            int i = 1;
                            foreach (string s in result)
                            {
                                buffer[i++] = (byte[])reader[s];  //(byte[])reader["li"];
                            }
                        }
                        else
                            buffer[0] = (byte[])reader[dbPictureColumn]; //(byte[])reader["AppImage"];

                        //buffer = (byte[])reader["AppImage"];
                        //int maxSize = 200000;
                        //buffer = new byte[maxSize];
                        //reader.GetBytes(1, 0L, buffer, 0, maxSize);
                    }


                    //if (!reader.IsDBNull(0))
                    //    id = reader.GetInt32(0);
//                    if (!reader.IsDBNull(0))
//                    {
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


                        //if (IMAGE_TYPE.picture == imageType)
                        //{
                        //    if (!reader.IsDBNull(0))
                        //        buffer = (byte[])reader[dbPictureColumn]; //(byte[])reader["AppImage"];
                        //}
                        //else if (IMAGE_TYPE.wsq == imageType)
                        //{
                        //    if (!reader.IsDBNull(0))
                        //        buffer = (byte[])reader[dbFingerColumn];  //(byte[])reader["AppWsq"];
                        //}
                        //else if (IMAGE_TYPE.fingerTemplates == imageType)
                        //{
                        //    StringBuilder sb = new StringBuilder();
                        //    sb.Append(reader["li"]);
                        //    sb.Append(reader["lm"]);
                        //    sb.Append(reader["lr"]);
                        //    sb.Append(reader["ll"]);
                        //    sb.Append(reader["ri"]);
                        //    sb.Append(reader["rm"]);
                        //    sb.Append(reader["rr"]);
                        //    sb.Append(reader["rl"]);
                        //    sb.Append(reader["lt"]);
                        //    sb.Append(reader["rt"]);
                        //    buffer = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                        //}

                        ////buffer = (byte[])reader["AppImage"];
                        //int maxSize = 200000;
                        //buffer = new byte[maxSize];
                        //reader.GetBytes(1, 0L, buffer, 0, maxSize);
//                    }
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
                //conn = new SqlConnection(getConnectionString());
                conn = new SqlConnection(configurationServiceClient.getConnectionString("ConnectionString"));

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

        public string getAppSetting(string key)
        {
            return configurationServiceClient.getAppSetting(key);
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
/*
        private String getConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
        }
*/
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
