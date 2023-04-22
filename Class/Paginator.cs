using IISAutoParts.DBcontext;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace IISAutoParts.Class
{
    public class Paginator
    {
        IISAutoPartsEntities _dbContext;
        private List<object> table = new List<object>();
        private int page { get; set; }
        private int countPage { get; set; }
        private int countElements  { get; set; }



        public Paginator(List<object> table, int page, int countElements) {
            this.page = page;
            this.countPage = (int)Math.Ceiling((double)table.Count() / countElements);
            this.countElements = countElements;
            this.table = table;
        }


        public void NextPage()
        {
            page = page + 1 < countPage ? (page+=1) : (page=(countPage));
        }

        public void PreviousPage()
        {
            page = page - 1 > 0 ? (page -= 1) : (page = 1);
        }

        public List<object> GetTable() {
            return table.Skip((page - 1) * countElements).Take(countElements).ToList();
        }

        public int GetPage()
        {
            return page;
        }

        public void SetPage(int page)
        {
            this.page = page;
        }

        public int GetCountpage() {
        return countPage;
        }

    }
}
