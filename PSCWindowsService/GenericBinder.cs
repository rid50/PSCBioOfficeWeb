using System;

[Serializable]
public class WsqImage
{
    public int XSize { get; set; }
    public int YSize { get; set; }
    public int XRes { get; set; }
    public int YRes { get; set; }
    public int PixelFormat { get; set; }
    public byte[] Content { get; set; }
}

namespace WsqSerializationBinder
{
    public class GenericBinder<T> : System.Runtime.Serialization.SerializationBinder
    {
        /// <summary>
        /// Resolve type
        /// </summary>
        /// <param name="assemblyName">eg. App_Code.y4xkvcpq, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null</param>
        /// <param name="typeName">eg. String</param>
        /// <returns>Type for the deserializer to use</returns>
        public override Type BindToType(string assemblyName, string typeName)
        {
            // We're going to ignore the assembly name, and assume it's in the same assembly 
            // that <T> is defined (it's either T or a field/return type within T anyway)

            string[] typeInfo = typeName.Split('.');
            bool isSystem = (typeInfo[0].ToString() == "System");
            string className = typeInfo[typeInfo.Length - 1];

            // noop is the default, returns what was passed in
            Type toReturn = null;
            try
            {
                toReturn = Type.GetType(string.Format("{0}, {1}", typeName, assemblyName));
            }
            catch (System.IO.FileLoadException) { }

            if (!isSystem && (toReturn == null))
            {   // don't bother if system, or if the GetType worked already (must be OK, surely?)
                System.Reflection.Assembly asm = System.Reflection.Assembly.GetAssembly(typeof(T));
                //throw new Exception(asm.GetTypes().Length.ToString());
                //int i = 0;
                //foreach (Type type in asm.GetTypes())
                //{
                //    if (i == 3)
                //        throw new Exception(type.Name + " --- " + type.ToString());

                //    i++;
                //}

                //System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
                //throw new Exception("Assembly for type: " + asm.FullName + " --- Class name: " + className + " --- Type name: " + typeName);
                string assembly = asm.FullName.Split(',')[0];   //FullName example: "App_Code.y4xkvcpq, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"
                if (asm == null)
                {
                    throw new ArgumentException("Assembly for type '" + typeof(T).Name.ToString() + "' could not be loaded.");
                }
                else
                {
                    //Type newtype = asm.GetType("WsqSerializationBinder" + "." + className);
                    Type newtype = asm.GetType(className);
                    //throw new Exception((newtype == null).ToString());

                    if (newtype == null)
                    {
                        throw new ArgumentException("Type '" + typeName + "' could not be loaded from assembly '" + assembly + "'.");
                    }
                    else
                    {
                        toReturn = newtype;
                    }
                }
            }
            return toReturn;
        }
    }
}
