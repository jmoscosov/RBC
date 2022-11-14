using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace SupervisorProfileManager
{
    class Program
    {
        /*
         jm185384 - NCR Chile 
        */

        static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            List<cGroup> docXML = new List<cGroup>();
            string strPathFile = string.Empty;
            RegistryKey localKey;
            bool bvalido = false;

            try
            {
                LoggerClass.Log($"args number : {args.Length.ToString()}");
               /* for (int i = 0; i < args.Length; i++)
                {
                    LoggerClass.Log($"args params : {args[i].ToString()}");
                }*/
/*
                if (Environment.Is64BitOperatingSystem)
                {
                    localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                    LoggerClass.Log($"OS 64bits");
                }
                else
                {
                    localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                    LoggerClass.Log($"OS 32bits");
                }
*/
                localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);

                LoggerClass.Log($"LocalKey Value {localKey.ToString()}");
                string strPathFilevalue = localKey.OpenSubKey("SOFTWARE\\NCR\\Advance NDC\\supervisor\\Password").GetValue("XMLFilePath").ToString();
                int intSupervisorPasswordLength = Convert.ToInt32(localKey.OpenSubKey("SOFTWARE\\NCR\\Advance NDC\\supervisor\\Password").GetValue("SupervisorPasswordLength").ToString());
                //LoggerClass.Log($"strPathFilevalue Value {strPathFilevalue.ToString()}");
                if ((localKey != null) && (string.IsNullOrEmpty(strPathFilevalue)== false))
                    {
                        Globals.strPathFile = strPathFilevalue;
                        Globals.intSupervisorPasswordLength = intSupervisorPasswordLength;
                        LoggerClass.Log($"Path file was found in the registry");
                    }
                    else
                    {
                        Globals.strPathFile = @"C:\Program Files\NCR APTRA\Advance NDC\Config\SupvPwd.xml";
                        LoggerClass.Log($"Path file was not found in the registry");
                    }
                if (args.Length == 0)
                {
                    LoggerClass.Log($"JM185384 - command with no arguments");
                    // Console.WriteLine("Invalid format. Please read documentation, press any key for exit");
                    LoggerClass.Log($"Invalid format. Please read the documentation");
                    //Console.Read();
                    return;
                }
                doc = LoadXML(doc, Globals.strPathFile);
                if (args.Length == 1)
                {
                    if (args[0].Substring(1, args[0].Length - 1).ToLower() == "listgroups")
                    {
                        // /listgroup 
                        bvalido = true;
                        LoggerClass.Log($"JM185384 - command listgroups");
                        docXML = xmlToList(doc);
                        ListGroups(docXML);

                    }
                    if (args[0].Substring(1, args[0].Length - 1).ToLower() == "listusers")
                    {
                        // /listusers 
                        bvalido = true;
                        LoggerClass.Log($"JM185384 - command listusers");
                        docXML = xmlToList(doc);
                        ListUsers(docXML);

                    }
                }
                if (args.Length == 4)
                {
                    if (args[3].Substring(1, args[3].Length - 1).ToLower() == "updategroup")
                    {
                        LoggerClass.Log($"JM185384 - command updategroup");
                        // idgroup groupname(optional) deniedupdate(optional) /updategroup 
                        bvalido = true;
                        string strNameGroup = string.Empty;
                        string strIdGroup = string.Empty;
                        string strDeniedGroup = string.Empty;
                        //String pattern = "^[A-Za-z]+$";
                        String pattern = "^[A-Za-z0-9\\s]+$"; //EXPRESION REGULAR PARA ESPACIOS EN BLANCO LETRAS Y NUMEROS
                        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);

                       if (regex.IsMatch(args[1]))
                            LoggerClass.Log($"JM185384 - Namegroup OK");
                        else
                        {
                            LoggerClass.Log($"JM185384 - Incorrect characters, please check input namegroup parameters");
                            return;
                        }
                    
                        strIdGroup = args[0].Trim();
                        strNameGroup = args[1].Trim();
                        strDeniedGroup = args[2].Trim();
                        bool cc = UpdateGroup(doc, strIdGroup, strNameGroup, strDeniedGroup);
                    }
                }
                if (args.Length == 3)
                {



                    if (args[2].Substring(1, args[2].Length - 1).ToLower() == "adduser")
                    {
                        LoggerClass.Log($"JM185384 - command adduser");
                        // password idgroup /adduser 
                        bvalido = true;
                        string password = string.Empty;
                        string idGroup = string.Empty;

                        password = args[0].Trim();
                        bool verifica = VerificarPassword(password);
                        if (!verifica)
                        {
                            LoggerClass.Log($"JM185384 - User ID wasn't created, because the password is not numeric");
                            return;
                        }
                        idGroup = args[1].Trim();
                        XmlElement objNewNode = doc.CreateElement("user");
                        if (password.Length <= Globals.intSupervisorPasswordLength)
                        {
                            bool resp = AddUser(password, objNewNode, doc, idGroup);
                        }
                        else
                        {
                            LoggerClass.Log($"JM185384 - User ID wasn't created, because the password exceeds the maximum. The password must have max.{Globals.intSupervisorPasswordLength.ToString()} numbers");
                        }

                    }

                    if (args[2].Substring(1, args[2].Length - 1).ToLower() == "updatepass")
                    {
                        /*
                          userID newpass /updatepass 
                         */
                        bvalido = true;
                        LoggerClass.Log($"JM185384 - command updatepass");
                        string password = string.Empty;
                        string idUser = string.Empty;
                        password = args[1].Trim();
                        bool verifica = VerificarPassword(password);
                        if (!verifica)
                        {
                            LoggerClass.Log($"JM185384 - User ID wasn't updated, because the password is not numeric");
                            return;
                        }

                        idUser = args[0].Trim();
                        XmlElement objNewNode = doc.CreateElement("user");
                        if (password.Length <= Globals.intSupervisorPasswordLength)
                        {
                            LoggerClass.Log($"JM185384 - Password length OK");
                            bool resp3 = UpdatePassword(doc, objNewNode, idUser, password);

                        }
                        else
                        {
                            LoggerClass.Log($"JM185384 - New Password for User ID {idUser} wasn't updated, because the password exceeds the maximum. The password must have max.{Globals.intSupervisorPasswordLength.ToString()} numbers");
                        }

                    }
                    if (args[2].Substring(1, args[2].Length - 1).ToLower() == "addgroup")
                    {
                        /*
                          groupName denied /addgroup 
                         */
                        bvalido = true;
                                               LoggerClass.Log($"JM185384 - command addgroup");
                        string strgroupName = string.Empty;
                        string strdenied = string.Empty;
                      //  String pattern = "^[A-Za-z]+$";
                        String pattern = "^[A-Za-z0-9\\s]+$";
                        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(pattern);

                        if (regex.IsMatch(args[0]))
                            LoggerClass.Log($"JM185384 - NameGroup OK");
                        else
                        {
                            LoggerClass.Log($"JM185384 - Incorrect characters, please check input namegroup parameters");
                            return;
                        }
                            
                        strgroupName = args[0].Trim();
                        strdenied = args[1].Trim();
                        bool aa = CreateGroup(doc, strgroupName, strdenied);
                    }
                }
                if (args.Length == 2)
                {

                    if (args[1].Substring(1, args[1].Length - 1).ToLower() == "deluser")
                    {
                        /*
                            userID /deluser 
                        */
                        bvalido = true;
                        LoggerClass.Log($"JM185384 - command deluser");
                        string idUser = string.Empty;
                        idUser = args[0].Trim();
                        XmlElement objNewNode = doc.CreateElement("user");
                        bool resp = DeleteUser(doc, objNewNode, idUser); ;
                    }
                    if (args[1].Substring(1, args[1].Length - 1).ToLower() == "delgroup")
                    {
                        /*
                            userID /delgroup 
                        */

                        string strUserID = string.Empty;
                        strUserID = args[0].Trim();
                        bool bb = DeleteGroup(doc, strUserID);
                    }

                }
                if (bvalido == false)
                {
                    LoggerClass.Log($"JM185384 - Command not executed, please check params");
                }

            }
            catch (Exception ex)
            {
                LoggerClass.Log($"JM185384 - Error : {ex.ToString()}");

            }
            // XmlElement objNewNode = doc.CreateElement("user");
            // XmlText text = doc.CreateTextNode("");
           // docXML = xmlToList(doc);
            // bool aa = CreateGroup(doc, "Antonia", "5;6;4;5,4");
            // bool bb = DeleteGroup(doc, "6");
         //   bool cc = UpdateGroup(doc, "2","","5;4,5");
           // bool resp = AddUser("12","1111", objNewNode,doc,"2");
           //bool resp2 = DeleteUser(doc, objNewNode, "11");
           // bool resp3 = UpdatePassword(doc, objNewNode, "02", "1111");
           // ListGroups(docXML);
           // ListUsers(docXML);
           //Console.ReadLine();
           // string pass = EncryptOrDecrypt("1111","1");
           // Encriptor("1111",10);
        }
        static public void ListGroups(List<cGroup> Groups)
        {
            LoggerClass.Log($"JM185384 - List of Groups        \n");
            LoggerClass.Log($"ID Grupo        Group Name                    Denied         \n");
            foreach (var nodo in Groups)
            {
                //Console.Write(nodo.ID.ToString() + "           " + nodo.Name + "           " + nodo.Denied + "\n");
                LoggerClass.Log($"{nodo.ID.ToString().PadRight(8,' ')}           {nodo.Name.PadRight(20, ' ')}           {nodo.Denied.PadRight(70, ' ')} \n");
            }
            Groups.Clear();
        }
        static public void ListUsers(List<cGroup> Groups)
        {
            LoggerClass.Log($"JM185384 - List of Users        \n");
            LoggerClass.Log($"User ID        Group Name \n");
            int grupos = 0;
            foreach (List<cUser> usuario in Groups.Select(x => x.Users).Distinct().ToList())
            {
               
                for (int i = 0; i <= usuario.Count - 1; i++)
                {
                    Console.Write(usuario[i].ID + "        " + Groups[grupos].Name +  "\n");
                    LoggerClass.Log($"{usuario[i].ID.ToString().PadRight(8, ' ')}           {Groups[grupos].Name.PadRight(20, ' ')}  \n");

                }
                grupos++;
            }
            Groups.Clear();
            
        }
        static public XmlDocument LoadXML(XmlDocument doc,string spath)
        {
            //String Path = @"C:\logs\SupvPwd.xml";
            doc.Load(spath);
            return doc;
        }
        static public List<cGroup> xmlToList(XmlDocument doc)
        {
            int groupLength = doc.GetElementsByTagName("group").Count;
            List<cGroup> Grupos = new List<cGroup>();
            for (int i = 0; i <= groupLength - 1; i++)
            {
                var groupNode = doc.GetElementsByTagName("group").Item(i);
                cGroup grupo = new cGroup();
                string EnterID = groupNode.Attributes.GetNamedItem("ID").Value;
                string NameGroup = groupNode.Attributes.GetNamedItem("Name").Value;
                string Denied = groupNode.Attributes.GetNamedItem("Denied").Value;
                grupo.ID = EnterID;
                grupo.Denied = Denied;
                grupo.Name = NameGroup;
                grupo.Users = new List<cUser>();
                int userLength = groupNode.ChildNodes.Count;
                if (userLength > 0)
                {
                    for (int j = 0; j <= userLength - 1; j++)
                    {
                        cUser usuario = new cUser();
                        var userNode = groupNode.ChildNodes.Item(j);
                        string usrID = userNode.Attributes.GetNamedItem("ID").Value;
                        string usrPWD = userNode.Attributes.GetNamedItem("password").Value;
                        usuario.ID = usrID;
                        usuario.Password = usrPWD;
                        grupo.Users.Add(usuario);
                    }

                }
                Grupos.Add(grupo);

                int result = String.Compare(EnterID.ToString(), groupNode.Value, 0);

            }
            return Grupos;
        }
        static public string Encriptor(string pwd , int id)
        {
            string key = "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,Z,Y,X,W,V,U,T,S,R,Q,P,O,N,M,L,K,J,I,H,G,F,E,D,C,B,A,z,y,x,w,v,u,t,s,r,q,p,o,n,m,l,k,j,i,h,g,f,e,d,c,b,a,!,@,#,$,%,^,&,*";
            List<string> s = new List<string>(key.Split(new string[] { "," }, StringSplitOptions.None));
            string cadena = string.Empty;
                for (int p = 0; p < pwd.Length; p++)
                {
                            string temp;
                            temp = pwd.Substring(p, 1);
                            var temp3 = Encoding.ASCII.GetBytes(s[id])[0];
                            var temp4 = Encoding.ASCII.GetBytes(temp)[0];
                            var result = temp3 ^ temp4;
                            cadena += Convert.ToChar(result).ToString();
                            id += 1;
                            
                }
                System.Console.WriteLine($"<{cadena}>");
            return cadena;
        }
        static public bool AddUser(string pwd, XmlElement elemento, XmlDocument doc,string idGrupo)
        {
            int cantidadGrupos = 0;
            int cantidadUsuarios = 0;
            string newChildId = string.Empty;
            int index = 0;
            int temp = 0;
            int largo2 = 0;
            string firstID;
            string IDValue;
            string IdAux;
            int flag1 = 0;
            int num, largo;
            string pwdEncryted = string.Empty;
            cantidadGrupos = doc.GetElementsByTagName("group").Count;
            for (int i = 0; i < cantidadGrupos; i++)
            {
                var groupNode = doc.GetElementsByTagName("group").Item(i);
                IDValue = groupNode.Attributes.GetNamedItem("ID").Value;
                if (string.Compare(IDValue, idGrupo)==0)
                {
                    temp = 1;
                    cantidadUsuarios=groupNode.ChildNodes.Count;

                    if(cantidadUsuarios == 10)
                    {
                        LoggerClass.Log($"JM185384 - User ID wasn't created, because Group ID is Full");
                        return false;
                    }
                    if(cantidadUsuarios == 0)
                    {
                        newChildId = IDValue + "0";
                        index = 0;
                    }
                    else
                    {
                        IdAux = groupNode.FirstChild.Attributes.GetNamedItem("ID").Value.ToString();
                        largo2 = IdAux.Length;
                        if(largo2 == 1)
                        {
                            firstID = IdAux.Substring(0, 1);
                        }
                        else
                        {
                            firstID = IdAux.Substring(1, 1);
                        }
                        if(!firstID.Equals("0"))
                        {
                            newChildId = idGrupo + "0";
                            index = 0;
                        }else if(cantidadUsuarios == 1)
                        {
                            newChildId = (groupNode.LastChild.Attributes.GetNamedItem("ID").Value).ToString();
                            int idUsuario = Convert.ToInt32(newChildId) + 1;
                            newChildId = idUsuario.ToString("00");
                            index = 1;
                        }
                        else
                        {
                            for (int j = 0; j <= cantidadUsuarios - 2; j++)
                            {
                                int oldnode = Convert.ToInt32(groupNode.ChildNodes.Item(j).Attributes.GetNamedItem("ID").Value.ToString());
                                int Newnode = Convert.ToInt32(groupNode.ChildNodes.Item(j+1).Attributes.GetNamedItem("ID").Value.ToString());

                                if(oldnode + 1!= Newnode)
                                {
                                    // Encuentra una diferencia en la secuencia de ID
                                    if(flag1 == 0)
                                    {
                                        // Asigna id nuevo 
                                        newChildId = (oldnode + 1).ToString();
                                        index = j + 1;
                                    }
                                    flag1 = 1;
                                    break;
                                }
                            }
                            if(flag1 == 0)
                            {
                                //newChildId = (groupNode.LastChild.Attributes.GetNamedItem("ID").Value + 1).ToString();
                                newChildId = (groupNode.LastChild.Attributes.GetNamedItem("ID").Value).ToString();
                                int idUsuario = Convert.ToInt32(newChildId) + 1;
                                newChildId = idUsuario.ToString("00");
                                num = Convert.ToInt32(newChildId);
                                largo = newChildId.Length;
                                if (largo == 1)
                                {
                                    newChildId = "0" + newChildId;
                                }
                                else
                                {
                                    newChildId = Convert.ToString(newChildId);
                                }
                                elemento.SetAttribute("ID", newChildId);
                                pwdEncryted = Encriptor(pwd, Convert.ToInt32(newChildId));
                                elemento.SetAttribute("password", pwdEncryted);
                                groupNode.AppendChild(elemento);
                                doc.Save(Globals.strPathFile);
                                LoggerClass.Log($"JM185384 - User ID {newChildId} was created in group {IDValue}");
                                return true;
                            }
                        }
                    }
                    elemento.SetAttribute("ID", newChildId);
                    pwdEncryted = Encriptor(pwd, Convert.ToInt32(newChildId));
                    elemento.SetAttribute("password", pwdEncryted);

                    groupNode.AppendChild(elemento);
                    groupNode.InsertBefore(elemento, groupNode.ChildNodes.Item(index));
                    LoggerClass.Log($"JM185384 - The New User ID {newChildId} was created in group {IDValue}");
                    doc.Save(Globals.strPathFile);
                    return true;
                }
            }
            LoggerClass.Log($"JM185384 - User ID wasn't created, because ID Group not found");
            return false;
        }
        static public bool DeleteUser(XmlDocument doc,XmlElement elemento,string UserId)
        {
            string searchGroup;
            int flag2,flag1 = 0;
            string newChildId = string.Empty;
            int cantidadGrupos = doc.GetElementsByTagName("group").Count;

            if (UserId.Length == 1)
            {
                searchGroup = "0";
            }
            else
            {
                searchGroup = UserId.Substring(0, 1);
            }

            cantidadGrupos = doc.DocumentElement.ChildNodes.Count;
            for (int i = 0; i < cantidadGrupos; i++)
            {
                if (string.Compare(doc.DocumentElement.ChildNodes.Item(i).Attributes.GetNamedItem("ID").Value.ToString(), searchGroup) == 0)
                {
                    flag2 = 1;
                    XmlNode node = doc.DocumentElement.ChildNodes.Item(i);
                    int cantidadHijos = node.ChildNodes.Count;
                    for (int j = 0; j < cantidadHijos; j++)
                    {
                        string nextUserId = node.ChildNodes.Item(j).Attributes.GetNamedItem("ID").Value;
                        if (string.Compare(nextUserId, UserId)==0)
                        {
                            XmlNode nodeDelete = node.RemoveChild(node.ChildNodes.Item(j));
                            flag1 = 0;
                            doc.Save(Globals.strPathFile);
                            LoggerClass.Log($"JM185384 - User ID {UserId} was deleted");
                            return true;
                        }
                    }
                }
            }

            LoggerClass.Log($"JM185384 - User ID {UserId} wasn't deleted, because User ID not exist");
            return false;

        }
        static public bool UpdatePassword(XmlDocument doc, XmlElement elemento,string userID,string newPassword)
        {
            int cantidadUsuarios = doc.GetElementsByTagName("user").Count;
            for (int i = 0; i < cantidadUsuarios; i++)
            {
                if (string.Compare(doc.GetElementsByTagName("user").Item(i).Attributes.GetNamedItem("ID").Value, userID) == 0)
                {
                    newPassword = Encriptor(newPassword, Convert.ToInt32(userID));
                    XmlNode nodo = doc.GetElementsByTagName("user").Item(i);
                    nodo.Attributes.GetNamedItem("password").Value = newPassword;
                    doc.Save(Globals.strPathFile);
                    LoggerClass.Log($"JM185384 - Password was updated for User ID {userID}");
                    return true;
                }
            }
            LoggerClass.Log($"JM185384 - Password wasn't updated, because User ID {userID} not exist");
            return false;
        }
        static public bool CreateGroup(XmlDocument doc, string groupName, string denied)
        {
            XmlElement objNewNode = doc.CreateElement("group");
            XmlText text = doc.CreateTextNode("");
            XmlElement elemento =  doc.DocumentElement;
            int cantidadGrupos = elemento.ChildNodes.Count;
            int flag = 0;
            string newId = string.Empty;
            denied = denied + ";0";
            LoggerClass.Log($"JM185384 - AddGroup Process");
            if (cantidadGrupos > 9)
            {
                LoggerClass.Log($"JM185384 - Group ID wasn't created, because capacity is Full");
                return false;
            }
            if(cantidadGrupos == 0)
            {
                LoggerClass.Log($"JM185384 - cantidadGrupos = 0");
                objNewNode.SetAttribute("ID", "0");
                objNewNode.SetAttribute("Denied", denied);
                objNewNode.SetAttribute("Name", groupName);
                elemento.InsertBefore(objNewNode, elemento.ChildNodes.Item(0));

            }
            else
            {
                string strFirstID = elemento.FirstChild.Attributes.GetNamedItem("ID").Value;
                if(cantidadGrupos == 1)
                {
                    LoggerClass.Log($"JM185384 - cantidadGrupos = 1");
                    newId = (Convert.ToInt32(elemento.LastChild.Attributes.GetNamedItem("ID").Value) + 1).ToString();
                    objNewNode.SetAttribute("ID", newId);
                    objNewNode.SetAttribute("Denied", denied);
                    objNewNode.SetAttribute("Name", groupName);
                    elemento.AppendChild(objNewNode);
                }
                else
                {
                    for (int i = 0; i <= cantidadGrupos - 2; i++)
                    {
                        LoggerClass.Log($"JM185384 - cantidadGrupos = {cantidadGrupos.ToString()}");
                        int nodoAnterior = Convert.ToInt32(elemento.ChildNodes.Item(i).Attributes.GetNamedItem("ID").Value);
                        int nodoNuevo = Convert.ToInt32(elemento.ChildNodes.Item(i+1).Attributes.GetNamedItem("ID").Value);
                        if((nodoAnterior + 1 ) != nodoNuevo)
                        {
                            if (flag == 0)
                            {
                                newId = Convert.ToString(nodoAnterior + 1);
                            }
                            objNewNode.SetAttribute("ID", newId);
                            objNewNode.SetAttribute("Denied", denied);
                            objNewNode.SetAttribute("Name", groupName);
                            elemento.InsertBefore(objNewNode,elemento.ChildNodes.Item(i+1));
                            flag = 1;
                        }
                    }
                    if(flag == 0)
                    {
                        newId = (Convert.ToInt32(elemento.LastChild.Attributes.GetNamedItem("ID").Value) + 1).ToString();
                        objNewNode.SetAttribute("ID", newId);
                        objNewNode.SetAttribute("Denied", denied);
                        objNewNode.SetAttribute("Name", groupName);
                        elemento.AppendChild(objNewNode);
                    }
                }
            }
            doc.Save(Globals.strPathFile);
            LoggerClass.Log($"JM185384 - The Group called {groupName} was created with ID {newId} ");
            return true;
            
        }
        static public bool DeleteGroup(XmlDocument doc , string groupID)
        {
            XmlElement elemento = doc.DocumentElement;
            int cantidadGrupos = elemento.ChildNodes.Count;
            for (int i = 0; i < cantidadGrupos; i++)
            { 
                if (string.Compare(groupID,elemento.ChildNodes.Item(i).Attributes.GetNamedItem("ID").Value) == 0)
                {
                  XmlNode deleteNode = elemento.RemoveChild(elemento.ChildNodes.Item(i));
                  doc.Save(Globals.strPathFile);
                  LoggerClass.Log($"JM185384 - The Group ID {groupID} was deleted success");
                   return true;
                }
            }
            LoggerClass.Log($"JM185384 - The Group ID {groupID} wasn't deleted, because ID not found");
            return false;
        }
        static public bool UpdateGroup(XmlDocument doc,string groupID, string nameGroup, string denied)
        {
            XmlElement elemento = doc.DocumentElement;
            int cantidadGrupos = elemento.ChildNodes.Count;
            for (int i = 0; i < cantidadGrupos; i++)
            {
                if (string.Compare(groupID, elemento.ChildNodes.Item(i).Attributes.GetNamedItem("ID").Value) == 0)
                {

                    if (nameGroup.Length != 0)
                    {
                        elemento.ChildNodes.Item(i).Attributes.GetNamedItem("Name").Value = nameGroup;
                        LoggerClass.Log($"The Name group ID {elemento.ChildNodes.Item(i).Attributes.GetNamedItem("ID").Value.ToString()} was updated");
                    }
                    if (denied.Length != 0)
                    {
                        denied = denied + ";0";
                        elemento.ChildNodes.Item(i).Attributes.GetNamedItem("Denied").Value = denied;
                        LoggerClass.Log($"The denied option group ID {elemento.ChildNodes.Item(i).Attributes.GetNamedItem("ID").Value.ToString()} was updated");
                    }
                            
                    //doc.Save(@"C:\logs\SupvPwd.xml");
                    doc.Save(Globals.strPathFile);
                    LoggerClass.Log($"JM185384 - The Group ID {groupID} was updated success");
                    return true;
                }

            }
            LoggerClass.Log($"JM185384 - The Group ID {groupID} wasn't updated, because ID not found");
            return false;
        }

        static private bool VerificarPassword(String password)
        {
            long newvalue = 0;

               if ((long.TryParse(password, out newvalue))== false)
                {
                    return false;
                }


            return true;
        }
    }
 }
