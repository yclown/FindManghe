using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    class 盲盒分析
    {
        public static List<string> prize2 = new List<string> {
            "喷射", "喵星人","飞碟",   //, "探测"
            "花卉", "火箭", "航空","地球",
              "电气", "魔怪", "纸盒",  "宇航员"};
        //public static int[] prize = { 1, 2, 3, 4, 5, 6, 7, 8, 9,10 };
        public static List<string[]> tips = new List<string[]>()
        {
           new string[] {"飞碟","喷射","火箭"},       //1
           new string[] {"纸盒", "宇航员", "火箭"},         //2
           new string[] {"航空","喵星人","魔怪"},       //3
           new string[] {"火箭","宇航员","探测"},       //4
           new string[] { "地球","飞碟","航空"},       //5
           new string[] { "探测", "火箭","纸盒"},       //6
           new string[] { "喷射", "喵星人","纸盒"},       //7
           new string[] {"喷射","喵星人","魔怪"},       //8
           //new string[] {"电气","纸盒","魔怪"},       //9
           new string[] {"花卉","魔怪","地球"},       //10
           new string[] { "纸盒","宇航员","喵星人"},       //11
           new string[] {"宇航员","花卉","地球"},       //12

        };
        public static int count = 0;

        static List<List<int>> res = new List<List<int>>();
        static void init()
        {
            for (var i = 0; i < prize2.Count; i++)
            {
                res.Add(new List<int> { });
                for (var j = 0; j < prize2.Count; j++)
                {
                    res[i].Add(0);
                }
            }
        }

        public static void pailie2(int postion, List<string> used)
        {
            for (int i = 0; i < prize2.Count; i++)
            {
                var temp = used.ToList();
                if (!temp.Contains(prize2[i]) && !tips[postion].Contains(prize2[i]))
                {
                    temp.Add(prize2[i]);
                    if (temp.Count == prize2.Count)
                    {
                        //Console.WriteLine(String.Join(",", temp));
                        count++;
                        for (var j = 0; j < temp.Count; j++)
                        {
                            var p = prize2.IndexOf(temp[j]);
                            res[j][p]++;
                        }
                        return;
                    }

                    pailie2(postion + 1, temp);
                }


            }
        }

        public static void fenxi()
        {

            init();
            DateTime dt1 = System.DateTime.Now;
            pailie2(0, new List<string> { });
            DateTime dt2 = System.DateTime.Now;
            TimeSpan ts = dt2.Subtract(dt1);
            Console.WriteLine(count);

            for (var i = 0; i < res.Count; i++)
            {
                Console.WriteLine("------------------");
                Console.WriteLine("盒子" + (i + 1));
                for (var j = 0; j < res[i].Count; j++)
                {

                    Console.WriteLine(prize2[j].PadRight(5, ' ') + " 次数：" + res[i][j] + " 概率:" + (res[i][j] * 1.0f / count * 100));
                }
                Console.WriteLine("------------------");
            }
        }

    }
}
