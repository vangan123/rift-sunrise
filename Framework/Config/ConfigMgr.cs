﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace FrameWork
{
    static public class ConfigMgr
    {
        static public List<aConfig> _Configs = new List<aConfig>();

        static public void LoadConfigs()
        {
            if (_Configs.Count > 0)
                return;

            Log.Debug("ConfigMgr", "Loading Config files");

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                bool MustRestart = false;
                foreach (Type type in assembly.GetTypes())
                {
                    // Pick up a class
                    if (type.IsClass != true)
                        continue;

                    object[] attrib = type.GetCustomAttributes(typeof(aConfigAttributes), true);
                    if (attrib.Length <= 0)
                        continue;

                    aConfigAttributes[] ConfigAttribs = (aConfigAttributes[])type.GetCustomAttributes(typeof(aConfigAttributes), true);

                    if (ConfigAttribs.Length > 0)
                    {
                        Log.Debug("ConfigMgr", "Deserializing : " + type.Name);

                        aConfig Obj = null;
                        XmlSerializer Xml = new XmlSerializer(type);

                        try
                        {
                            FileInfo FInfo = new FileInfo(ConfigAttribs[0].FileName);
                            Directory.CreateDirectory(FInfo.DirectoryName);
                        }
                        catch (Exception)
                        {
                        }

                        List<ConfigMethod> OnLoad = new List<ConfigMethod>();
                        foreach (MethodInfo m in type.GetMethods())
                        {
                            aConfigMethod[] Mets = m.GetCustomAttributes(typeof(aConfigMethod), true) as aConfigMethod[];
                            if (Mets.Length > 0)
                            {
                                try
                                {
                                    OnLoad.Add((ConfigMethod)Delegate.CreateDelegate(typeof(ConfigMethod), m));
                                }
                                catch (Exception e)
                                {
                                    Log.Error("ConfigMgr", "ConfigMethod Error : " + e.ToString());
                                }
                            }
                        }

                        FileStream fs = new FileStream(ConfigAttribs[0].FileName, FileMode.OpenOrCreate);
                        bool FirstLoad = false;

                        if (fs.Length <= 0)
                        {
                            FirstLoad = true;
                            Obj = Activator.CreateInstance(type) as aConfig;
                            Log.Info("Config", "A configuration file was created : " + ConfigAttribs[0].FileName);
                        }
                        else
                        {
                            Obj = Xml.Deserialize(fs) as aConfig;
                            if (!Obj.IConfiguredTheFile)
                                Log.Info("Config", ConfigAttribs[0].FileName + " : IConfiguredTheFile value is false.");
                        }

                        fs.SetLength(0);
                        Xml.Serialize(fs, Obj);
                        fs.Close();

                        if (FirstLoad || !Obj.IConfiguredTheFile)
                            MustRestart = true;

                        OnLoad.ForEach(info => { info.Invoke(ConfigAttribs[0], Obj, FirstLoad); });

                        Log.Debug("ConfigMgr", "Registering config : " + ConfigAttribs[0].FileName);
                        _Configs.Add(Obj);
                    }
                }

                if (MustRestart)
                {
                    Log.Info("Config", "You must configure the server before continuing.");
                    Log.Info("Config", "Press any key to exit");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
        }

        static public T GetConfig<T>()
        {
            aConfig Conf = null;
            Conf = _Configs.Find(info => info.GetType() == typeof(T));

            if(Conf != null)
                return (T)Convert.ChangeType(Conf, typeof(T));
            else
                return (T)Convert.ChangeType(null, typeof(T));
        }
    }
}
