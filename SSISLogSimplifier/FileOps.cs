using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSISLogSimplifier
{
   public class FileOps
    {
       public List<LogEntity> readFromLog(string path)
       {
           string line = string.Empty;
           string WARNING = "Warning:";
           string ERROR = "Error:";
           List<LogEntity> entityList = new List<LogEntity>();
           using (System.IO.StreamReader reader = new System.IO.StreamReader(path))
           {
               while ((line = reader.ReadLine()) != null)
               {

                   if (line.Contains(WARNING))
                   {
                      LogEntity le = new LogEntity();
                      le.EventType = LogEnum.Warning;
                      le.EventTime = line.Remove(0, 8);
                      le.Code = reader.ReadLine().Remove(0, 9);
                      le.Source = reader.ReadLine().Remove(0, 11);
                      le.Description = reader.ReadLine().Remove(0, 16);
                      entityList.Add(le);
                   }
                   else if (line.Contains(ERROR))
                   {
                       LogEntity le = new LogEntity();
                       le.EventType = LogEnum.Error;
                       le.EventTime = line.Remove(0,6);
                       le.Code = reader.ReadLine().Remove(0,9);
                       le.Source = reader.ReadLine().Remove(0,11);
                       le.Description = reader.ReadLine().Remove(0,16);
                       entityList.Add(le);
                   }

               }
               reader.Close();
           }
           return entityList;
           
       }
    }
}
