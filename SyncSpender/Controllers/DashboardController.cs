using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SyncSpender.Data;
using SyncSpender.Models;
using System.Globalization;
using System.Threading.Tasks;
using System.Transactions;
using Transaction = SyncSpender.Models.Transaction;

namespace SyncSpender.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {

            //Last 7 days transaction
            DateTime StartDate = DateTime.Today.AddDays(-6);
            DateTime EndDate = DateTime.Today;

            List<Transaction> SelectedTransactions = await _context.Transactions
                .Include(x => x.Category)
                .Where(y => y.Date >= StartDate && y.Date <= EndDate)
                .ToListAsync();

            //Total Income
            int TotalIncome = (int)SelectedTransactions
                .Where(i => i.Category.Type == "Income")
                .Sum(j => j.Amount);
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-BD");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.TotalIncome = String.Format(culture, "{0:C0}", TotalIncome);
            //Total Expense
            int TotalExpense = (int)SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .Sum(j => j.Amount);
            CultureInfo culture2 = CultureInfo.CreateSpecificCulture("en-BD");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.TotalExpense = String.Format(culture, "{0:C0}", TotalExpense);

            //Balance
            int Balance = TotalIncome - TotalExpense;
            CultureInfo culture3 = CultureInfo.CreateSpecificCulture("en-BD");
            culture.NumberFormat.CurrencyNegativePattern = 1;
            ViewBag.Balance = String.Format(culture,"{0:C0}", Balance);

            //Doughnut Chart - Expense By Category
            ViewBag.DoughnutChartData = SelectedTransactions
                .Where(i => i.Category.Type == "Expense")
                .GroupBy(j => j.Category.CategoryId)
                .Select(k => new
                {
                    categoryTitleWithIcon = k.First().Category.Icon+ "" + k.First().Category.Title,
                    amount = k.Sum(j => j.Amount),
                    formattedAmount = k.Sum(j => j.Amount).ToString("C0"),
                }
                )
                .OrderByDescending(l => l.amount)
                .ToList();

            //Spline Chart - Income & Expense
            //Income
            List<SplineChartData> SplineChartData = SelectedTransactions
                .GroupBy(i => i.Date.Date)
                .Select(j => new SplineChartData
                {
                    day = j.Key.ToString("dd MMM"),
                    income = (int)j.Where(k => k.Category.Type == "Income").Sum(l => l.Amount),
                    expense = (int)j.Where(k => k.Category.Type == "Expense").Sum(l => l.Amount)
                })
                .ToList();

            //Combine Income & Expense
            string[] Last7Days = Enumerable.Range(0, 7)
                .Select(i => StartDate.AddDays(i).ToString("dd MMM"))
                .ToArray();
            ViewBag.SplineChartData = from day in Last7Days
                                      join income in SplineChartData on day equals income.day
                                      into dayIncomeJoined
                                      from income in dayIncomeJoined.DefaultIfEmpty()
                                      join expense in SplineChartData on day equals expense.day
                                      into dayExpenseJoined
                                      from expense in dayExpenseJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          day = day,
                                          income = income == null ? 0 : income.income,
                                          expense = expense == null ? 0 : expense.expense
                                      };

            return View();
        }
    }
       public class SplineChartData
       {
           public string day { get; set; }
           public int income { get; set; }
           public int expense { get; set; }
    }
}
