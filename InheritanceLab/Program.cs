
public class Employee
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Sin { get; set; }

    public Employee(int id, string name, string sin ) 
    { 
        this.ID = id;
        this.Name = name;
        this.Sin = sin;
    }

    public virtual decimal calculateWeeklyPay()
    {
        return 0;
    }
}

//DERIVED CLASS STARTS HERE ------------------------------------------------------------------
public class Salaried : Employee
{
    public decimal WeeklySalary { get; set; }

    public Salaried(int id, string name, string sin, decimal weeklysalary) : base(id, name, sin)
    {
        WeeklySalary = weeklysalary;
    }

    public override decimal calculateWeeklyPay()
    {
        return WeeklySalary;
    }
}

public class Wage : Employee
{
    public decimal hourlyRate { get; set; }
    public int workedHours { get; set; }

    public Wage(int id, string name, string sin, decimal hourlyrate, int workedhours) : base(id, name, sin)
    {
        hourlyRate = hourlyrate;
        workedHours = workedhours;
    }

    public override decimal calculateWeeklyPay()
    {
        decimal normalPay = hourlyRate * workedHours;
        decimal overTimePay = 0;

        if (workedHours > 40) 
        {
            int overTimeHours = workedHours - 40;
            overTimePay = hourlyRate * 1.5m * overTimeHours;
        }

        return normalPay + overTimePay;
    }
}

public class PartTime : Employee
{
    public decimal hourlyRate { get; set; }
    public int workedHours { get; set; }

    public PartTime(int id, string name, string sin, decimal hourlyrate, int workedhours) : base(id, name, sin)
    {
        hourlyRate = hourlyrate;
        workedHours = workedhours;
    }

    public override decimal calculateWeeklyPay()
    {
        return hourlyRate * workedHours;
    }
}
//PROGRAM -------------------------------------------------------------------------------
public class Program
{
    static void Main(string[] args)
    {
        List<Employee> employees = ReadEmployeesFromFile("C:\\Users\\Darrel Nguyen\\Documents\\Object Programming\\Object-Programming2-DarrelN\\InheritanceLab\\res\\employees.txt");

        decimal averageWeeklyPay = CalculateAverageWeeklyPay(employees);
        Console.WriteLine($"Average Weekly Pay: {averageWeeklyPay}");

        decimal highestWeeklyPay = CalculateHighestWeeklyPay(employees);
        Console.WriteLine($"Highest Weekly Pay: {highestWeeklyPay}");

        decimal lowestSalary = CalculateLowestSalary(employees);
        Console.WriteLine($"Lowest Salary: {lowestSalary}");

        Dictionary<string, double> categoryPercentages = CalculateCategoryPercentages(employees);
        Console.WriteLine("Employee Category Percentages:");
        foreach (var category in categoryPercentages)
        {
            Console.WriteLine($"{category.Key}: {category.Value * 100}%");
        }
    }




    static List<Employee> ReadEmployeesFromFile(string filePath)
    {
        List<Employee> employees = new List<Employee>();

        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string[] empolyeeTxTLine = line.Split(':');
            if (empolyeeTxTLine.Length >= 8)
            {
                int employeeID = int.Parse(empolyeeTxTLine[0]);
                string name = empolyeeTxTLine[1];
                string sin = empolyeeTxTLine[5];

                //First number of employeeID
                string employeeIDString = empolyeeTxTLine[0]; 
                char firstDigit = employeeIDString[0];  
                int employeeIDfirst = int.Parse(firstDigit.ToString());  

                if (empolyeeTxTLine.Length == 8)
                {
                    decimal weeklySalary = decimal.Parse(empolyeeTxTLine[7]);
                    employees.Add(new Salaried(employeeID, name, sin, weeklySalary));
                }
                else if (empolyeeTxTLine.Length == 9)
                {
                    decimal hourlyRate = decimal.Parse(empolyeeTxTLine[7]);
                    int workHours = int.Parse(empolyeeTxTLine[8]);

                    if (employeeIDfirst >= 5 && employeeIDfirst <= 7)
                    {
                        employees.Add(new Wage(employeeID, name, sin, hourlyRate, workHours));
                    }
                    else if (employeeIDfirst >= 8 && employeeIDfirst <= 9)
                    {
                        employees.Add(new PartTime(employeeID, name, sin, hourlyRate, workHours));
                    }
                }
            }
        }

        return employees;
    }

    static decimal CalculateAverageWeeklyPay(List<Employee> employees)
    {
        if (employees.Count == 0)
        {
            return 0;
        }

        decimal totalPay = 0;
        foreach (Employee employee in employees)
        {
            totalPay += employee.calculateWeeklyPay();
        }

        return totalPay / employees.Count;
    }

    static decimal CalculateHighestWeeklyPay(List<Employee> employees)
    {
        decimal highestPay = 0;

        foreach (Employee employee in employees)
        {
            if (employee is Wage wageEmployee)
            {
                decimal weeklyPay = wageEmployee.calculateWeeklyPay();
                if (weeklyPay > highestPay)
                {
                    highestPay = weeklyPay;
                }
            }
        }

        return highestPay;
    }

    static decimal CalculateLowestSalary(List<Employee> employees)
    {
        decimal lowestSalary = decimal.MaxValue;

        foreach (Employee employee in employees)
        {
            if (employee is Salaried salariedEmployee)
            {
                decimal weeklySalary = salariedEmployee.WeeklySalary;
                if (weeklySalary < lowestSalary)
                {
                    lowestSalary = weeklySalary;
                }
            }
        }

        return lowestSalary;
    }

    static Dictionary<string, double> CalculateCategoryPercentages(List<Employee> employees)
    {
        int totalEmployees = employees.Count;
        Dictionary<string, int> categoryCounts = new Dictionary<string, int>()
        {
            { "Salaried", 0 },
            { "Wage", 0 },
            { "Part-Time", 0 }
        };

        foreach (Employee employee in employees)
        {
            if (employee is Salaried)
            {
                categoryCounts["Salaried"]++;
            }
            else if (employee is Wage)
            {
                categoryCounts["Wage"]++;
            }
            else if (employee is PartTime)
            {
                categoryCounts["Part-Time"]++;
            }
        }

        Dictionary<string, double> categoryPercentages = new Dictionary<string, double>();
        foreach (var category in categoryCounts)
        {
            double percentage = (double)category.Value / totalEmployees;
            categoryPercentages.Add(category.Key, Math.Round(percentage,2));
        }

        return categoryPercentages;
    }
}