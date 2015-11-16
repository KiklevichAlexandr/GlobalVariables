using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GlobalVariables
{
    class METRICS
    {
        public string Source { get; set; }
        public List<string> Functions { get; set; }
        public List<string> Variables { get; set; }
      //  public int IndexOfLastFunction { get; set; }

        public METRICS (string src)
        {
            Source = src;
            Functions = new List<string>();
            Variables = new List<string>();
            // IndexOfLastFunction = 0;
            DeleteComments(src);
            ReplaceBeginEnd();
            FillVariables();
            FillFunctions(Source);
            CalculateMetrix();
        }
       
        public void DeleteComments(string src)
        {
            Regex find = new Regex(@"((//.*|{([^}])+})|(\'.*\'))");
            src = find.Replace(src, "");
        }

        public void FillFunctions(string src)
        {
            Regex procPattern = new Regex("procedure ", RegexOptions.IgnoreCase);
            Regex funcPattern = new Regex("function ", RegexOptions.IgnoreCase);
            src = src.Remove(0, src.IndexOf("};", StringComparison.CurrentCultureIgnoreCase) + 4);
            List<int> indexes = new List<int>(); // индексы слов function
            for (int i = 0; i < procPattern.Matches(src).Count; i++)
            {
                indexes.Add(procPattern.Matches(src)[i].Index);
            }
            for (int i = 0; i < funcPattern.Matches(src).Count; i++)
            {
                indexes.Add(funcPattern.Matches(src)[i].Index);
            }
            for (int i = 0; i < indexes.Count(); i++)
            {
                Functions.Add(NextTextInBrakes(src.Remove(0, indexes[i])));
            }
        }
        private int EndBrakePosition(string src)
        {
            int a = 0;
            int b = 0;
            for (int i = 0; (i < src.Length); i++)
            {
                if (src[i] == '{')
                {
                    a++;
                }
                if (src[i] == '}')
                {
                    a--;
                    if (a == 0)
                    {
                        b = i;
                        break;
                    }
                }
            }
            return b;
        }
        private string NextTextInBrakes(string src)
        {
            string result = src.Remove(0, src.IndexOf('{') + 1).Remove(1 + EndBrakePosition(src.Remove(0, src.IndexOf('{'))));

            return result;
        }

        public void ReplaceBeginEnd ()
        {
            string src = Source;
            Regex begin = new Regex(@"begin\W", RegexOptions.IgnoreCase);
            src =  Regex.Replace(src, @"begin\W", "{");
            Regex tryRegExp = new Regex(@"try\W", RegexOptions.IgnoreCase);
            src =  Regex.Replace(src, @"try\W", "{");
            Regex end = new Regex(@"\Wend", RegexOptions.IgnoreCase);
            src =  Regex.Replace(src, @"\Wend", "}");
            Source = src;
        }

        public void FillVariables ()
        {
            string buf = Source;
            buf = Source.Remove(0, buf.IndexOf("};", StringComparison.CurrentCultureIgnoreCase)+4);
            if (buf.IndexOf("procedure ", StringComparison.CurrentCultureIgnoreCase)>0 &&
                buf.IndexOf("function ", StringComparison.CurrentCultureIgnoreCase)> 0)
            {
                if (buf.IndexOf("procedure ", StringComparison.CurrentCultureIgnoreCase) >
                    buf.IndexOf("function ", StringComparison.CurrentCultureIgnoreCase))
                {
                    buf = buf.Remove(buf.IndexOf("function ", StringComparison.CurrentCultureIgnoreCase));
                }
                else
                {
                    buf = buf.Remove(buf.IndexOf("procedure ", StringComparison.CurrentCultureIgnoreCase));
                }
            }
            if (buf.IndexOf("function ", StringComparison.CurrentCultureIgnoreCase )< 0 && 
                buf.IndexOf("procedure ", StringComparison.CurrentCultureIgnoreCase) > 0)
            {
                buf = buf.Remove(buf.IndexOf("procedure ", StringComparison.CurrentCultureIgnoreCase));
            }
            if (buf.IndexOf("function ", StringComparison.CurrentCultureIgnoreCase) > 0 &&
                buf.IndexOf("procedure ", StringComparison.CurrentCultureIgnoreCase) < 0)
            {
                buf = buf.Remove(buf.IndexOf("function ", StringComparison.CurrentCultureIgnoreCase));
            }
            List<string> arr1 = buf.Split(';').ToList();
            for (int i = 0; i < arr1.Count - 1;i++)
            {
                string vars  = arr1[i].Split(':')[0];
                List<string> arr2 = vars.Split(',').ToList();
                foreach ( var a in arr2)
                {
                    var s = a.Split(' ');
                    s = s.OrderByDescending(x => x.Length).ToArray();
                    Variables.Add(s[0]);            
                }
            }
        }
        

        public string CalculateMetrix()
        {
            int aup = 0;
            foreach (var f in Functions )
            {
                bool exists = false;
                foreach (var v in Variables )
                {
                    if (f.IndexOf(v,StringComparison.CurrentCultureIgnoreCase) > 0)
                    {
                        exists = true;
                    }
                }
                if (exists == true)
                {
                    aup++;
                }
            }
            
            int pup = 0;
           
            pup = Functions.Count * Variables.Count;
            float rup = (float)aup / (float)pup;
            return "Rup = " + rup + "   Aup = " + aup + "   Pup = " + pup;
            
        }
        
    }
}
