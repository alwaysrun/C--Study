using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleTest
{
    partial class Program
    {
        class ItemTop { }
        class ItemBase
        {
            public string BaseName { get; set; }
        }
        class ItemSecond : ItemBase
        {
            public string SubName { get; set; }

            public ItemSecond()
            {
                BaseName = "Base-Second";
                SubName = "Sub-Second";
            }
        }
        class ItemThird : ItemBase
        {
            public string SubName { get; set; }

            public ItemThird()
            {
                BaseName = "Base-Third";
                SubName = "Sub-Third";
            }
        }

        //class AllBase<T> where T : ItemBase
        interface AllBase<out T> where T : ItemBase
        {
            int Count { get; }
            T Get();
            //IEnumerable<T> All { get; set; }
        }

        class AllSub<T> : ItemTop, AllBase<T> where T : ItemBase, new()
        {
            public int Count { get; set; }
            public List<T> All { get; set; }

            public T Get()
            {
                return All[0];
            }
            public AllSub()
            {
                All = new List<T>();
                All.Add(new T());
            }
        }

        class AllSecond : AllSub<ItemSecond>
        {
        }

        class AllThird : AllSub<ItemThird>
        {

        }

        private static void TestCovariant()
        {
            List<ItemBase> lstItems = new List<ItemBase>();
            lstItems.Add(new ItemSecond());

            List<AllBase<ItemBase>> lstAll = new List<AllBase<ItemBase>>();
            lstAll.Add(new AllSecond());
            lstAll.Add(new AllThird());

            foreach (var item in lstAll)
            {
                var top = item as ItemTop;
                Console.WriteLine(item.Get().BaseName);
            }
        }
    }
}
